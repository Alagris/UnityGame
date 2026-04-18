using Env.Runtime;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VectorGraphics;
using UnityEngine;
using static UnityEngine.UI.Image;

namespace Env.Runtime
{
    public class ProcSection : MonoBehaviour, Section
    {
        ProcSectionSpawner spawner;

        internal int idx;
        MeshRenderer terrainMeshRenderer;
        MeshFilter terrainMeshFilter, grassMeshFilter;
        MeshCollider terrainMeshCollision;
        List<InstancedLODGroup> instances = new List<InstancedLODGroup>();
        private int2 sectionPosition;

        public void Setup(ProcSectionSpawner spawner, int idx)
        {

            this.spawner = spawner;
            this.idx = idx;
            terrainMeshRenderer = gameObject.AddComponent<MeshRenderer>();
            terrainMeshFilter = gameObject.AddComponent<MeshFilter>();
            terrainMeshCollision = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
            
        }

        public string GetDebugStr()
        {
            string a = idx.ToString();
            string p = a.PadLeft(3);
            return p;
        }

        public void OnDistanceChanged(float distance)
        {
            foreach (InstancedLODGroup lodGroup in instances)
            {
                lodGroup.SetLOD(distance);
            }
        }

        public void OnExit()
        {
        }

        public void OnLoad(float distance, int2 absolutePosition)
        {
            this.sectionPosition = absolutePosition;
            Refresh();
            OnDistanceChanged(distance);
        }

        public void Refresh()
        {
            //Debug.Log("Refreshing section "+idx);
            float2 xzOffset = (float2)sectionPosition * spawner.ChunkSize;
            float3 offset = new float3(xzOffset.x, spawner.OffsetY, xzOffset.y);
            Blackboard bb = spawner.ProcEnv.Run(spawner.ResX, spawner.ResZ, spawner.ChunkSize, offset);
            ProcMesh procMesh =  bb.GetReturnedMesh();
            Mesh m = procMesh.toMesh();
            terrainMeshFilter.mesh = m;
            terrainMeshCollision.sharedMesh = m;
            gameObject.layer = procMesh.Layer;
            List<Material> mats = new List<Material>();
            mats.Add(bb.returnedTerrainMaterial);
            terrainMeshRenderer.SetSharedMaterials(mats);
            AddInstances(bb.GetReturnedInstances());


        }
        public void AddInstances(ProcInstanceSet instances)
        {
            if (instances != null)
            {
                foreach (ProcInstances i in instances)
                {
                    AddInstances(i);
                }
            }
        }
        public void AddInstances(ProcInstances instances)
        {
            if (instances.Mesh.Count > 0)
            {
                InstancedLODGroup im = gameObject.AddComponent<InstancedLODGroup>();
                im.Set(instances.Mesh, instances.Transforms);
                this.instances.Add(im);
                GameObject prefab = instances.Mesh.Prefab;
                if (prefab != null)
                {
                    foreach (Trans transform in instances.Transforms)
                    {
                        SpawnInstance(prefab, transform);
                    }
                }
            }
        }

        public virtual GameObject SpawnInstance(GameObject prefab , Trans transform)
        {
            GameObject copy = Instantiate(prefab);
            copy.transform.SetParent(gameObject.transform, true);
            transform.AssignTo(copy.transform);
            return copy;
        }

        public void ClearInstances()
        {
            foreach (InstancedLODGroup i in GetComponents<InstancedLODGroup>())
            {
                DestroyImmediate(i);
            }
            this.instances.Clear();
        }
        public void SetInstancesVisible(bool visible)
        {
            foreach (InstancedLODGroup i in this.instances)
            {
                i.SetVisible(visible);
            }
        }
        public void OnUnload()
        {
            if (grassMeshFilter != null)
            {
                grassMeshFilter.mesh = null;
                terrainMeshFilter.mesh = null;
                terrainMeshCollision.sharedMesh = null;

            }
            ClearInstances();
        }

        public void DestroyImmediate()
        {
            if (spawner != null)
            {
                DestroyImmediate(gameObject);
            }
        }
    }
}