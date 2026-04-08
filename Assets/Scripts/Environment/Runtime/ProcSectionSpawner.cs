using Env.Runtime;
using Unity.Mathematics;
using UnityEngine;

namespace Env.Runtime
{
    public class ProcSectionSpawner : SectionSpawner
    {
        public override Section SpawnSection(int id)
        {
            GameObject sectionObj = new GameObject();
            sectionObj.name = "Section " + id;
            sectionObj.transform.SetParent(transform, true);
            ProcSection section = sectionObj.AddComponent<ProcSection>();
            section.Setup(this, id);
            
            return section;
        }
    }
}