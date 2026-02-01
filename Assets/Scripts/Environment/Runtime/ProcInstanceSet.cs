
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Env.Runtime
{
    public class ProcInstanceSet
    {
        Dictionary<InstanceableObject, ProcInstances> Instances = new Dictionary<InstanceableObject, ProcInstances>();

        public ProcInstanceSet() { }
        public ProcInstanceSet(ProcInstances instances)
        {
            Instances.Add(instances.Object, instances);
        }
        internal ProcInstances join(ProcInstanceSet b)
        {
            ProcInstances output = new ProcInstances();
            foreach (var e in Instances)
            {
                //var myInstances = e.Value;
                // = b.Instances.GetValueOrDefault(e.Key, null);
            }
            return output;
        }
    }
}