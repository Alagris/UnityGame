using System;
using System.Collections.Generic;
using System.Text;
using Unity.Mathematics;

namespace Env.Runtime
{

    public enum AttributeType
    {
        FLOAT,
        FLOAT2,
        FLOAT3,
        FLOAT4,
        INT,
        INT2,
        INT3,
        INT4,
        UINT,
        UINT2,
        UINT3,
        UINT4,
    }
    
    public abstract class Attribute
    {
        public bool CopyOnWrite = false;
        public static T[] Copy<T>(T[] array)
        {
            T[] copy = new T[array.Length];
            Array.Copy(array, copy, array.Length);
            return copy;
        }
        public abstract Attribute DeepClone();

        public void SetCopyOnWrite()
        {
            CopyOnWrite = true;
        }
    }

    public class AttributeFloat : Attribute
    {
        float[] value;
        public AttributeFloat(float[] value)
        {
            this.value = value;
        }
        public override Attribute DeepClone()=>new AttributeFloat(Copy(value));
    }
    public class AttributeFloat2 : Attribute
    {
        float2[] value;
        public AttributeFloat2(float2[] value)
        {
            this.value = value;
        }
        public override Attribute DeepClone() => new AttributeFloat2(Copy(value));
    }
    public class AttributeFloat3 : Attribute
    {
        float3[] value;
        public AttributeFloat3(float3[] value)
        {
            this.value = value;
        }
        public override Attribute DeepClone() => new AttributeFloat3(Copy(value));
    }
    public class AttributeFloat4 : Attribute
    {
        float4[] value;
        public AttributeFloat4(float4[] value)
        {
            this.value = value;
        }
        public override Attribute DeepClone() => new AttributeFloat4(Copy(value));
    }
    public class AttributeInt : Attribute
    {
        int[] value;
        public AttributeInt(int[] value)
        {
            this.value = value;
        }
        public override Attribute DeepClone() => new AttributeInt(Copy(value));
    }
    public class AttributeInt2 : Attribute
    {
        int2[] value;
        public AttributeInt2(int2[] value)
        {
            this.value = value;
        }
        public override Attribute DeepClone() => new AttributeInt2(Copy(value));
    }
    public class AttributeInt3 : Attribute
    {
        int3[] value;
        public AttributeInt3(int3[] value)
        {
            this.value = value;
        }
        public override Attribute DeepClone() => new AttributeInt3(Copy(value));
    }
    public class AttributeInt4 : Attribute
    {
        int4[] value;
        public AttributeInt4(int4[] value)
        {
            this.value = value;
        }
        public override Attribute DeepClone() => new AttributeInt4(Copy(value));
    }
    public class AttributeUint : Attribute
    {
        int[] value;
        public AttributeUint(int[] value)
        {
            this.value = value;
        }
        public override Attribute DeepClone() => new AttributeUint(Copy(value));
    }
    public class AttributeUint2 : Attribute
    {
        int2[] value;
        public AttributeUint2(int2[] value)
        {
            this.value = value;
        }
        public override Attribute DeepClone() => new AttributeUint2(Copy(value));
    }
    public class AttributeUint3 : Attribute
    {
        int3[] value;
        public AttributeUint3(int3[] value)
        {
            this.value = value;
        }
        public override Attribute DeepClone() => new AttributeUint3(Copy(value));
    }
    public class AttributeUint4 : Attribute
    {
        int4[] value;
        public AttributeUint4(int4[] value)
        {
            this.value = value;
        }
        public override Attribute DeepClone() => new AttributeUint4(Copy(value));
    }
}
