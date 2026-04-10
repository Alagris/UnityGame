using Env.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.Collections;
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


        //ComputeBuffer TransformsNative;

        int currentLOD = 0;
        bool Visible = true;

        public void SetVisible(bool visible)
        {
            Visible = visible;
        }
        public void Set(List<InstanceableLOD> LODs, List<Trans> transforms)
        {

            this.LODs = LODs;
            this.Transforms = new List<Matrix4x4>(transforms.Count);
            
            //TransformsNative = new ComputeBuffer(, transforms.Count, );
            
            for (int i = 0; i < transforms.Count; i++)
            {
                this.Transforms.Add(transforms[i].toMat4());
                //TransformsNative[i] = transforms[i].toMat4();
            }
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
                        RenderParams p = new RenderParams(mats[i]);
                        
                        //p.material.SetBuffer("Instances", TransformsNative);
                        Graphics.RenderMeshInstanced(p, mesh, i, Transforms);
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