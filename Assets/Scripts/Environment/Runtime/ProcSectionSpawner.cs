using Env.Runtime;
using Unity.Mathematics;
using UnityEngine;

public abstract class ProcSectionSpawner : MonoBehaviour
{

    [SerializeField]
    public EnvCompiledGraph ProcEnv;
    [SerializeField]
    public int ResX=32;
    [SerializeField]
    public int ResZ=32;
    [SerializeField]
    public float ChunkSize=100;
    [SerializeField]
    public float OffsetY=0;
    [SerializeField]
    public Material LandscapeMaterial;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public ProcSection SpawnSection(int id)
    {
        GameObject sectionObj = new GameObject();
        sectionObj.name = "Section " + id;
        sectionObj.transform.SetParent(transform, true);
        ProcSection section = sectionObj.AddComponent<ProcSection>();
        section.Setup(this, id);
        return section;
    }
    public abstract void Refresh();
}
