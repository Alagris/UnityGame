using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


namespace Env.Runtime
{

    [ExecuteInEditMode]
    public class InstancedMesh : MonoBehaviour
    {
        [SerializeField]
        Mesh Mesh;

        [SerializeField]
        Material[] Materials;

        [SerializeField]
        List<Matrix4x4> Transforms;

        bool Visible = true;

        public void SetVisible(bool visible)
        {
            Visible = visible && Mesh != null;
        }
        private bool Validate(Mesh mesh, Material[] materials, List<Matrix4x4> transforms)
        {
            if (mesh == null || materials == null || transforms == null) return false;

            foreach (Material m in materials)
            {
                if (m == null || !m.enableInstancing)
                {
                    return false;
                }
            }
            return true;

        }
        public void Set(Mesh mesh, Material[] materials, List<Matrix4x4> transforms)
        {
            if (Validate(mesh, materials, transforms))
            {
                Mesh = mesh;
                Materials = materials;
                Transforms = transforms;
            }
            else
            {
                Mesh = null;
                Materials = null;
                Transforms = null;
                Visible = false;

            }

        }

        private void Update()
        {
            if (Visible)
            {
                int submeshes = Mathf.Min(Mesh.subMeshCount, Materials.Length);
                for (int i = 0; i < submeshes; i++)
                {
                    if (Materials[i].enableInstancing)
                    {
                        Graphics.DrawMeshInstanced(Mesh, i, Materials[i], Transforms);
                    }
                }
            }
        }
    }

}