using System.Collections.Generic;
using System;
using TMPro;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Env.Runtime;

public class ProcTerrainGenerator : ProcSectionSpawner
{

    public override Section SpawnSection(int id)
    {
        GameObject sectionObj = new GameObject();
        sectionObj.name = "Section " + id;
        sectionObj.transform.SetParent(transform, true);
        ProcSection section = sectionObj.AddComponent<ProcTerrainSection>();
        section.Setup(this, id);

        return section;
    }
}