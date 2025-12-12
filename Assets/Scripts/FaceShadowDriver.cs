using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class FaceShadowDriver : MonoBehaviour
{

    
    

    [SerializeField]
    Transform HeadBone;

    [SerializeField]
    int materialIndex=0;
    
    [SerializeField]
    string HeadAnglePropertyName= "_FaceShadowAngle";

    [SerializeField]
    Light Sun;
    private Material mat;

    void Start()
    {
        SkinnedMeshRenderer Mesh = GetComponent<SkinnedMeshRenderer>();
        mat = Mesh.materials[materialIndex];
        if (Sun == null)
        {
            GameObject sun = GameObject.Find("Sun");
            Sun = sun.GetComponent<Light>();
        }
    }

    Vector2 xz(Vector3 v) => new Vector2(v.x,v.z);
    // Update is called once per frame
    void Update()
    {
        float angle = 90.0f-Vector2.SignedAngle(xz(Sun.transform.forward), xz(HeadBone.forward));
        float angleNormalised = angle / 360.0f;
        mat.SetFloat(HeadAnglePropertyName, angleNormalised);
        
    }
}
