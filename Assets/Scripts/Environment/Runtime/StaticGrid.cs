using Unity.Mathematics;
using UnityEngine;

public class StaticGrid : ProcSectionSpawner
{
    [SerializeField]
    internal int Rows = 3;
    [SerializeField]
    internal int Columns = 3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    ProcSection[] sections;
    


    private void Start()
    {
        Refresh();

    }
    public override void Clear()
    {
        if (sections != null)
        {
            for (int i = 0; i < sections.Length; i++)
            {
                if (sections[i] != null)
                {
                    sections[i].DestroyImmediate();
                }
            }
            sections = null;
        }
    }
    public override void UnloadAll()
    {
        if (sections != null)
        {
            for (int i = 0; i < sections.Length; i++)
            {
                sections[i].OnUnload();
            }
        }
    }
    public override void Refresh()
    {

        UnloadAll();
        if (sections == null) {
            sections = new ProcSection[Rows * Columns];
        }
        else
        {
            if(sections.Length != Rows * Columns)
            {
                ProcSection[] old = sections;
                sections = new ProcSection[Rows * Columns];
                int common = Mathf.Min(old.Length, sections.Length);
                for (int i = 0; i < common; i++)
                {
                    sections[i] = old[i];

                }
                
                for (int i = common; i < old.Length; i++)
                {
                    old[i].DestroyImmediate();
                }
            }
        }
        int2 offset = new int2(Columns / 2, Rows / 2);
        for (int x = 0, i = 0; x < Columns; x++)
        {
            for (int y = 0; y < Rows; y++, i++)
            {
                if (sections[i] == null)
                {
                    sections[i] = SpawnSection(i);
                }
                sections[i].OnLoad(0, new int2(x, y) - offset);
            }
        }
    }

    
}
