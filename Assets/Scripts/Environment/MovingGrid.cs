using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using Unity.AppUI.Redux;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public interface Section {
	void OnLoad(float distance, int2 absolutePosition);
    void OnDistanceChanged(float distance);
    void OnUnload();
	void OnExit();

	string GetDebugStr();
}

struct Chunk<T> where T : class, Section
{
    public T section;
	public float distance;
	public int2 posFromCenter;

    public T Unload()
	{
		if (section == null)
		{
			return null;
		}
		else
		{
			section.OnUnload();
			T s = section;
			section = null;
			return s;
		}
	}
    public void Load(T t, int2 absolutePosition)
    {
        section = t;
		section.OnLoad(distance, absolutePosition);
    }
    public bool IsLoaded()
	{
		return section != null;
	}
	public void SetSection(T section)
	{
		this.section = section;
		if (section != null)
		{
			section.OnDistanceChanged(distance);
		}
	}

    public string GetDebugStr()
    {
        if (IsLoaded())
        {
			return section.GetDebugStr();
        }
        else
        {
            return "  _";
        }
    }
}

public class MovingGrid<T> where T : class, Section
{

	private readonly Func<int, T> sectionSpawner;
    public float chunkSize = 1000;
	int radius = 0;
	int diameter = 0;
    internal readonly Stack<T> unusedSections = new Stack<T>();
	internal Chunk<T>[] chunks;
	int[] sortedByDistance;
	int2 absCenterChunkOffset;
	
	public MovingGrid(Func<int, T> sectionSpawner, float chunkSize, int renderRadius, float spawnRadius, float3 initialPlayerPosition)
	{
        this.sectionSpawner = sectionSpawner;
        absCenterChunkOffset = getChunkAbsPosFromWorldPos(initialPlayerPosition);
        setGridSize(renderRadius, chunkSize);
		addChunksWithinRadius(spawnRadius);
    }
    public int getRadius() {
		return radius;
	}
    public int getDiameter() {
		return diameter;
	}
	public int getArea() {
		return diameter * diameter;
	}
	
	
	public void reset(int radius, float chunkSize, float3 playerPosition) {
        absCenterChunkOffset = getChunkAbsPosFromWorldPos(playerPosition);
        reset(radius, chunkSize);
	}
    public void reset(int radius, float chunkSize) {
		setGridSize(radius, chunkSize);
	}
	public void UnloadAll()
	{
		if (chunks != null)
		{
			for (int i = 0; i < chunks.Length; i++)
			{
				addToUnused(chunks[i].Unload());
			}
		}
    }
	public void EnsureSectionCount(int area)
	{
		
        while (unusedSections.Count < area)
        {
            unusedSections.Push(sectionSpawner(unusedSections.Count));

        }
        while (unusedSections.Count > area)
        {
            unusedSections.Pop().OnExit();

        }
    }
    public void setGridSize(int radius, float chunkSize) {
		this.chunkSize = chunkSize;
        this.radius = radius;
		diameter = radius * 2 + 1;
		int area = diameter * diameter;
		UnloadAll();
        EnsureSectionCount(area);
        chunks = new Chunk<T>[area];
        sortedByDistance = new int[area];

        for (int y = 0, i = 0; y < diameter; y++) {
			for (int x = 0; x < diameter; x++, i++) {
				int2 posFromCenter = new int2(x - radius, y - radius);
				float distL2 = math.sqrt(math.dot(posFromCenter, posFromCenter));
				//const int distL1 = math::sum(math::abs(posFromCenter));
				//int distLinf = math::max(math::abs(posFromCenter.x), math::abs(posFromCenter.y));
				chunks[i].distance = distL2;
				chunks[i].posFromCenter = posFromCenter;
                sortedByDistance[i] = i;

            }
		}
		Array.Sort(sortedByDistance, (a, b) => chunks[a].distance.CompareTo(chunks[b].distance));
		
	}
	public int getChunkIdx(int2 chunkPos)
	{
		return getChunkIdx(chunkPos.x, chunkPos.y);
	}
    public int getChunkIdx(int x, int y) {
		if (y < 0 || y >= diameter || x < 0 || x >= diameter)return -1;
		int chunkIdx = x + diameter * y;
		return chunkIdx;
	}
	public int2 getChunkPos(int chunkIdx) {
		int chunkX = chunkIdx % diameter;
		int chunkY = chunkIdx / diameter;
		return new int2(chunkX, chunkY);
	}
	public T getSection(int chunkIdx) {
		if (chunkIdx < 0) return null;
		return this.chunks[chunkIdx].section;
	}
	public T getSection(int2 chunkPos) {
		return getSection(getChunkIdx(chunkPos));
	}
	public T setSection(int chunkIdx, T section) {
		if (chunkIdx == -1)return null;
		T prev = this.chunks[chunkIdx].section;
		this.chunks[chunkIdx].section = section;
		return prev;
	}
	private void addToUnused(T section)
	{
		if (section != null) {
			unusedSections.Push(section);
		}
	}
    public T setSection(int2 chunkRelPos, T section) {
		return setSection(getChunkIdx(chunkRelPos), section);
	}
	public void dropChunk(int chunkIdx) {
		addToUnused(chunks[chunkIdx].Unload());
	}
	public void spawnChunk(int chunkIdx, int2 absChunkPos) {
		if (!chunks[chunkIdx].IsLoaded()) {
			chunks[chunkIdx].Load(unusedSections.Pop(), absChunkPos);
		}
	}
	public int2 getChunkAbsPosFromWorldPos(float posX, float posY) {
		return new int2(Mathf.FloorToInt(posX / chunkSize), Mathf.FloorToInt(posY / chunkSize));
	}
	public  int2 getChunkAbsPosFromWorldPos(float3 pos) {
		return getChunkAbsPosFromWorldPos(pos.x, pos.z);
	}

	public  int2 getAbsOffsetToBottomLeftmostChunk() {
		return new int2(this.absCenterChunkOffset.x - radius, this.absCenterChunkOffset.y - radius);
	}
	public  int2 absToRelPos(int2 chunkAbsPos) {
		return chunkAbsPos - getAbsOffsetToBottomLeftmostChunk();
	}
	public  int2 relToAbsPos(int2 chunkRelPos) {
		return chunkRelPos + getAbsOffsetToBottomLeftmostChunk();
	}


	void shiftSurroundingChunks(int2 shift) {
		if (diameter <= 1)return;
		shift.x = math.sign(shift.x);
		shift.y = math.sign(shift.y);
		int offset = shift.x + shift.y * diameter;
		int dX = 1, dY = 1, sX = 0, sY = 0, eX = diameter, eY = diameter;
		if (shift.x != 0) {
			if (shift.x > 0) {
				dX = -1;
				sX = diameter - 1;
				eX = 0;
			}
			else {
				sX = 0;
				eX = diameter - 1;
			}

			for (int y = 0; y < diameter; y++) {
				dropChunk(sX + y * diameter);
			}
		}
		if (shift.y != 0) {
			if (shift.y > 0) {
				dY = -1;
				sY = diameter - 1;
				eY = 0;
			}
			else {
				sY = 0;
				eY = diameter - 1;
			}

			for (int x = 0; x < diameter; x++) {
				dropChunk(x + sY * diameter);
			}
		}

		for (int x = sX; x != eX; x += dX) {
			for (int y = sY; y != eY; y += dY) {
				int toIdx = x + y * diameter;
				int fromIdx = toIdx - offset;
				chunks[toIdx].SetSection(chunks[fromIdx].section);
			}
		}
		if (shift.x != 0) {
			for (int y = 0; y < diameter; y++) {
				chunks[eX + y * diameter].section = null;
			}
		}
		if (shift.y != 0) {
			for (int x = 0; x < diameter; x++) {
				chunks[x + eY * diameter].section = null;
			}
		}
	}

	public void addChunksWithinRadius(float r) {
		for (int i = 0; i < sortedByDistance.Length; i++) {
			int j = sortedByDistance[i];
			if (chunks[j].distance <= r) {
				spawnChunk(j, absCenterChunkOffset+ chunks[j].posFromCenter);
			
			}
			else break; // we iterate in sorted order so we know we can terminate early
		}
	}

	public string GetDebugStr() { 
		string s = "";
		for (int y = diameter - 1; y >= 0; y--) {
			for (int x = 0; x < diameter; x++) {
                int chunkIdx = getChunkIdx(x, y);
                s += chunks[chunkIdx].GetDebugStr();
				s += ' ';
			}
			s += '\n';
		}
		return s;
	}
	public bool update(float3 playerPosition, float spawnRadius)
	{
		int2 newAbsCenterChunkOffset = getChunkAbsPosFromWorldPos(playerPosition);
		if (math.any(absCenterChunkOffset != newAbsCenterChunkOffset))
		{
            Debug.Log("Before=\n"+GetDebugStr());
            int2 shift = absCenterChunkOffset - newAbsCenterChunkOffset;
            shiftSurroundingChunks(shift);
			Debug.Log("Shifted=\n"+GetDebugStr());
            absCenterChunkOffset = newAbsCenterChunkOffset;
            addChunksWithinRadius(spawnRadius);
            Debug.Log("Spawned=\n"+GetDebugStr());
            return true;
        }
		return false;
	}

	public void forEachSection(System.Action<T> f)
	{
        for (int i = 0; i < chunks.Length; i++)
        {
            if (chunks[i].IsLoaded())
            {
                f(chunks[i].section);
            }
        }
    }
}