using Env.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Env.Runtime
{
    [ExecuteInEditMode]
    public class InstancedLODGroup : MonoBehaviour
    {

        [SerializeField]
        List<InstanceableLOD> LODs;

        [SerializeField]
        List<Matrix4x4> Transforms;

        int currentLOD = 0;
        bool Visible = true;

        public void SetVisible(bool visible)
        {
            Visible = visible;
        }
        public void Set(List<InstanceableLOD> LODs, List<Matrix4x4> transforms)
        {

            this.LODs = LODs;
            this.Transforms = transforms;
        }

        private void Update()
        {
            if (Visible && currentLOD < LODs.Count)
            {
                Mesh mesh = LODs[currentLOD].StaticMesh;
                Material[] mats = LODs[currentLOD].Materials;
                int submeshes = Mathf.Min(mesh.subMeshCount, mats.Length);
                for (int i = 0; i < submeshes; i++)
                {
                    if (mats[i] != null && mats[i].enableInstancing)
                    {
                        Graphics.DrawMeshInstanced(mesh, i, mats[i], Transforms);
                    }
                }
            }
        }

        public void SetLOD(float distance)
        {
            for (int i = 0; i < LODs.Count; i++)
            {
                if (distance < LODs[i].chunkDistance)
                {
                    currentLOD = i;
                    return;
                }
            }
        }
    }


}