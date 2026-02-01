using UnityEngine;

[CreateAssetMenu(fileName = "FoliageType", menuName = "Scriptable Objects/FoliageType")]
public class FoliageType : ScriptableObject
{
    [SerializeField]
    Mesh Mesh;
    [SerializeField]
    Mesh AttachableMesh;
    [SerializeField]
    internal Material Material;
    [SerializeField]
    internal Material AttachableMeshMaterial;
    [SerializeField]
    internal bool EnableCollision = false;
    [SerializeField]
    internal uint Seed = 474767;
    [SerializeField]
    internal float MaxTilt = 30;
    [SerializeField]
    internal float MinWidth = 1.0f;
    [SerializeField]
    internal float MaxWidth = 1.0f;
    [SerializeField]
    internal float MinHeight = 1.0f;
    [SerializeField]
    internal float MaxHeight = 1.0f;
    [SerializeField]
    internal Color Bright = new Color(1, 1, 1);
    [SerializeField]
    internal Color Dark = new Color(0.5f, 1, 0.5f);
}
