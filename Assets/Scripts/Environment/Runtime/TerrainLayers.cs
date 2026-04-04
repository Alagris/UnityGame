using Codice.Client.BaseCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Env.Runtime
{

    public struct TL
    {
        public float[] weight;
        public TerrainLayer layer;

        public TL(float[] weight, TerrainLayer layer)
        {
            this.weight = weight;
            this.layer = layer;
        }
    }
    public class TerrainLayers : List<TL>
    {
        internal void Add(float[] weight, TerrainLayer layer)
        {
            Add(new TL(weight, layer));
        }
        
        public void Normalize()
        {
            if (Count > 0) {
                int len = this[0].weight.Length;
                float[] sum = new float[len] ;
                int firstWithoutWeights = -1;
                for (int j = 0; j < Count; j++)
                {
                    float[] weight = this[j].weight;
                    if (weight == null) {
                        if (firstWithoutWeights == -1)
                        {
                            firstWithoutWeights = j;
                        }
                    } else {
                        for (int i = 0; i < len; i++)
                        {
                            sum[i] += weight[i];
                        }
                    }
                }
                if (firstWithoutWeights >= 0)
                {
                    if (Count == 1)
                    {
                        float[] missingWeight = new float[len];
                        for (int i = 0; i < len; i++)
                        {
                            missingWeight[i] = 1;
                        }
                        this[firstWithoutWeights] = new TL(missingWeight, this[firstWithoutWeights].layer);
                    }
                    else
                    {
                        float[] missingWeight = new float[len];
                        this[firstWithoutWeights] = new TL(missingWeight, this[firstWithoutWeights].layer);
                        float max = sum.Max();
                        for (int i = 0; i < len; i++)
                        {
                            missingWeight[i] = max - sum[i];
                        }
                        for (int j = 0; j < Count; j++)
                        {
                            float[] weight = this[j].weight;
                            if (weight != null)
                            {
                                for (int i = 0; i < len; i++)
                                {
                                    weight[i] /= max;
                                }
                            }
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < Count; j++)
                    {
                        float[] weight = this[j].weight;
                        if (weight != null)
                        {
                            for (int i = 0; i < len; i++)
                            {
                                weight[i] /= sum[i];
                            }
                        }
                    }
                }
                RemoveAll(tl => tl.weight == null);
            }
        }
    }
}
