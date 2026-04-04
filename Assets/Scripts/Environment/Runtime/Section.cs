using System;
using System.Collections.Generic;
using System.Text;
using Unity.Mathematics;

namespace Env.Runtime
{
    public interface Section
    {
        void OnLoad(float distance, int2 absolutePosition);
        void OnDistanceChanged(float distance);
        void OnUnload();

        void Refresh();


        string GetDebugStr();
        void DestroyImmediate();
    }
}