using UnityEngine;

public class VisualiseNormals : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    Material linesMaterial;

    MeshFilter originalFilter;
    MeshFilter normalsFilter;
    Mesh originalMesh=null;
    void Start()
    {
        
        originalFilter = GetComponent<MeshFilter>();
        GameObject normalsChild = new GameObject("Normals");
        normalsChild.transform.parent = transform;
        MeshRenderer normalsRenderer = normalsChild.AddComponent<MeshRenderer>();
        normalsRenderer.material = linesMaterial;
        normalsFilter = normalsChild.AddComponent<MeshFilter>();
        
    }

    void Refresh()
    {
        if (originalFilter==null)
        {
            originalFilter = GetComponent<MeshFilter>();
            if (originalFilter == null)
            {
                return;
            }
        }
        Mesh currentMesh = originalFilter.sharedMesh;
        if (!Object.ReferenceEquals(originalMesh, currentMesh))
        {
            originalMesh = currentMesh;
            Vector3[] originalVerts = currentMesh.vertices;
            Vector3[] originalNormals = currentMesh.normals;
            Vector3[] normalVerts = new Vector3[originalVerts.Length*2];
            int[] normalIndices = new int[originalVerts.Length * 2];
            for(int i = 0; i < normalIndices.Length; i++)
            {
                normalIndices[i] = i;
            }
            Mesh normalMesh = new Mesh();
            for (int i = 0; i < originalVerts.Length; i++)
            {
                normalVerts[i * 2] = originalVerts[i];
                normalVerts[i * 2+1] = originalVerts[i]+originalNormals[i];
            }
            normalMesh.SetVertices(normalVerts);
            normalMesh.SetIndices(normalIndices, MeshTopology.Lines, 0);
            normalsFilter.mesh = normalMesh;
        }
    }
    // Update is called once per frame
    void Update()
    {
        Refresh();
    }
}
