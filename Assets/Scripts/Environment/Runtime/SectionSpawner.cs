using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Env.Runtime
{

    public abstract class SectionSpawner : MonoBehaviour
    {

        [SerializeField]
        public EnvCompiledGraph ProcEnv;
        [SerializeField]
        public int ResX = 32;
        [SerializeField]
        public int ResZ = 32;
        [SerializeField]
        public float ChunkSize = 100;
        [SerializeField]
        public float OffsetY = 0;
        
       


        public abstract Section SpawnSection(int id);

    }
}
