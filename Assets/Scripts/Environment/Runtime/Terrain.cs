using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Env.Runtime
{
    public abstract class ProcEnvTerrain: MonoBehaviour
    {
        [SerializeField]
        public SectionSpawner Spawner;

        protected virtual void Start()
        {
            if (Spawner == null)
            {
                Spawner = GetComponent<SectionSpawner>();
                if (Spawner == null)
                {
                    Debug.LogError("Section Spawner is missing in", gameObject);
                }
            }
            
        }


        public abstract void Refresh();
        public abstract void UnloadAll();

        public virtual void Clear()
        {
            while (gameObject.transform.childCount > 0)
            {
                var child = gameObject.transform.GetChild(0);
                DestroyImmediate(child.gameObject);
            }
        }
    }
}
