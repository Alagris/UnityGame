using Unity.Mathematics;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using UnityEngine.UIElements;

public class BMath
{
    public static float3 tangent(float2 gradient)
    {
        return new float3(1, gradient.x + gradient.y, 1);
    }
    public static float3 tangentX(float2 gradient)
    {
        return new float3(1, gradient.x, 0);
    }
    public static float3 tangentZ(float2 gradient)
    {
        return new float3(0, gradient.y, 1);
    }
    public static float3 normal(float2 gradient)
    {
        return math.cross(new float3(1, gradient.x, 0), new float3(0, gradient.y, 1));
    }
    public static float3 cross_tri(float3 v1, float3 v2, float3 v3)
    {
        return math.cross(v1 - v2, v3 - v2);
    }
    public static float area_tri(float3 v1, float3 v2, float3 v3)
    {
        return math.length(cross_tri(v1, v2, v3)) * 0.5f;
    }
    public static float3 interp_v3_v3v3v3(float3 v1, float3 v2, float3 v3, float3 w)
    {
        return v1 * w.x + v2 * w.y + v3 * w.z;
    }

    /**The following holds:
        BMath.normalToYawPitch(Vector3.forward)==(0,PI/2)
        BMath.normalToYawPitch(Vector3.up)==(0,0)
        BMath.normalToYawPitch(Vector3.right)==(PI/2,PI/2)*/
    public static float2 normalToYawPitch(float3 normal)
    {
        float yaw = Mathf.Atan2(normal.x, normal.z);
        float padj = Mathf.Sqrt(normal.x* normal.x + normal.z*normal.z);
        float pitch = Mathf.Atan2(padj, normal.y);
        return new float2(yaw, pitch);
    }
    public static float3 rotateByEulerDegYXY(float3 vector, float spin, float yaw, float pitch) {
        return rotateByEulerYXY(vector, Mathf.Deg2Rad * spin, Mathf.Deg2Rad * yaw, Mathf.Deg2Rad * pitch);
    }
    /**first applies spin (rotation around Y) then pitch (rotation around X) then yaw (again around Y). The following holds:
     Vector3.forward == BMath.rotateByEulerDegYXY(Vector3.up, 0, 0, 90)
     Vector3.up = BMath.rotateByEulerDegYXY(Vector3.up, 0, 0, 0)
     Vector3.right = BMath.rotateByEulerDegYXY(Vector3.up, 0, 90, 90)
    So this function is the inverse of normalToYawPitch
    */
    public static float3 rotateByEulerYXY(float3 vector, float spin, float yaw, float pitch)
    {
        float ss = Mathf.Sin(spin);
        float sc = Mathf.Cos(spin);
        float ys = Mathf.Sin(yaw);
        float yc = Mathf.Cos(yaw);
        float ps = Mathf.Sin(pitch);
        float pc = Mathf.Cos(pitch);
        float x = vector.x;
        float y = vector.y;
        float z = vector.z;
        // apply spin
        //   x axis
        //   ^
        //   |
        //   +-->  z axis
        float x2 = x * sc - z * ss;
        float z2 = x * ss + z * sc;
        // apply pitch
        //   y axis
        //   ^
        //   |
        //   +-->  z axis
        //z' = z*cos(t)-y*sin(t)
        //y' = z*sin(t)+y*cos(t)
        float z3 = y * ps + z2 * pc;
        float y3 = y * pc - z2 * ps;
        // apply yaw
        //   x axis
        //   ^
        //   |
        //   +-->  z axis
        float z4 = z3 * yc - x2 * ys;
        float x4 = z3 * ys + x2 * yc;
        return new float3(x4,y3,z4);
    }
    public static float3x3 rotationByEulerYXY(float spin, float yaw, float pitch)
    {
        float s_s = Mathf.Sin(spin);
        float s_c = Mathf.Cos(spin);
        float y_s = Mathf.Sin(yaw);
        float y_c = Mathf.Cos(yaw);
        float p_s = Mathf.Sin(pitch);
        float p_c = Mathf.Cos(pitch);
        //float x2 = x * sc - z * ss;
        //float z2 = x * ss + z * sc;
        // as a matrix:
        //[x']   [sc,0, - ss]   [x]
        //[y'] = [0 ,1,    0] * [y]
        //[z']   [ss,0,   sc]   [z]
        //
        //float z3 = y * ps + z2 * pc;
        //float y3 = y * pc - z2 * ps;
        // as a matrix:
        //[x']   [1,0 ,    0]   [x]
        //[y'] = [0,pc, - ps] * [y]
        //[z']   [0,ps,   pc]   [z]
        //
        //float z4 = z3 * yc - x2 * ys;
        //float x4 = z3 * ys + x2 * yc;
        // as a matrix:
        //[x']   [ yc, 0, ys]   [x]
        //[y'] = [0  , 1,  0] * [y]
        //[z']   [-ys, 0, yc]   [z]

        // They need to be applied in the order from bottom to top.
        // In wolfram alpha language it becomes:
        // {{ y_c, 0, y_s},{0  , 1,  0},{-y_s, 0, y_c}}
        // {{1,0 ,    0},{0,p_c, - p_s},{0,p_s,   p_c}}
        // {{s_c,0, - s_s},{0 ,1,    0},{s_s,0,   s_c}}

        // which yields
        return new float3x3(p_c * s_s * y_s + s_c * y_c, p_s * y_s, p_c * s_c * y_s - s_s * y_c,
                    -p_s * s_s, p_c, -p_s * s_c,
                    p_c * s_s * y_c - s_c * y_s, p_s * y_c, p_c * s_c * y_c + s_s * y_s);

    }
    public static float3 yawPitchToNormal(float yaw, float pitch)
    {
        return rotateByEulerYXY(Vector3.up, 0, yaw, pitch);
    }
}