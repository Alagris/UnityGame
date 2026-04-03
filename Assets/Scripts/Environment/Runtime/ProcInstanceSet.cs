
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Env.Runtime
{
    public class ProcInstanceSet: List<ProcInstances>
    {
        
        
        public ProcInstanceSet() { }
        public ProcInstanceSet(IEnumerable<ProcInstances> e) :base(e){ }
        public ProcInstanceSet(ProcInstances instances)
        {
            Add(instances);
        }
        internal ProcInstanceSet join(ProcInstanceSet b)
        {
            return new ProcInstanceSet(this.Concat(b));
        }

        internal ProcInstanceSet SetMaterials(List<Material> mats)
        {
            ProcInstanceSet output = new ProcInstanceSet();
            Material[] a = mats.ToArray();
            foreach (ProcInstances p in this){
                ProcInstances newP = new ProcInstances(p);
                newP.SetMaterials(mats);
                output.Add(newP);
            }
            return output;
        }
    }
}