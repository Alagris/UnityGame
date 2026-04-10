
using Unity.Mathematics;
using UnityEngine;

namespace Env.Runtime
{
    public struct Trans
    {
        public float3 Location;
        public float3 Scale;
        public quaternion Rotation;
        //public Trans() : this(new float3(0, 0, 0), new float3(1, 1, 1), quaternion.identity) { }
        public Trans(float3 Location, quaternion Rotation) : this(Location, new float3(1, 1, 1), Rotation) { }
        public Trans(float3 Location) : this(Location, new float3(1, 1, 1), quaternion.identity) { }
        public Trans(float3 Location, float3 Scale, quaternion Rotation)
        {
            this.Location = Location;
            this.Scale = Scale;
            this.Rotation = Rotation;
        }
        public Matrix4x4 toMat4() => Matrix4x4.TRS(Location, Rotation, Scale);

        public void AssignTo(UnityEngine.Transform trans)
        {
            trans.position = Location;
            trans.rotation = Rotation;
            trans.localScale = Scale;
        }
    }
}
