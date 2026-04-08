using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Environment
{
    internal interface ISpawnedInstance
    {
        void Setup(ProcTerrainSection procTerrainSection);
    }
}
