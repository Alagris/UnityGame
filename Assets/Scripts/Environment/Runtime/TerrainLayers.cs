using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Env.Runtime
{
    
    public class TerrainLayers : ScriptableObject
    {
        [SerializeField]
        public Texture2DArray diffuse;
        [SerializeField]
        public Texture2DArray normal;
        [SerializeField]
        public Texture2DArray mask;
        [SerializeField]
        public string[] names;
        [SerializeField]
        public Material material;

        [MenuItem("Assets/Create/Proc Env/Layers")]
        static void CreateAssetFile()
        {
            



            List<TerrainLayer> layers = new List<TerrainLayer>();
            string texArrayPath = null;
            foreach (object o in Selection.objects)
            {
                if (o is TerrainLayer)
                {
                    TerrainLayer l = (TerrainLayer)o;
                    layers.Add(l);
                    if (texArrayPath == null)
                    {
                        string path = AssetDatabase.GetAssetPath(l);
                        texArrayPath = Path.GetDirectoryName(path);

                    }
                }
            }
            if (layers.Count > 0)
            {
                TerrainLayers asset = ScriptableObject.CreateInstance<TerrainLayers>();
                Func<TerrainLayer, Texture2D>[] layerToTex = new Func<TerrainLayer, Texture2D>[] {
                      l=>l.diffuseTexture,
                      l=>l.normalMapTexture,
                      l=>l.maskMapTexture
                };
                Action<Texture2DArray, TerrainLayers>[] saveLayer = new Action<Texture2DArray, TerrainLayers>[] {
                      (arr,layers)=>layers.diffuse=arr,
                      (arr,layers)=>layers.normal=arr,
                      (arr,layers)=>layers.mask=arr,
                };
                string[] filenames = new string[] { "diffuse", "normal", "mask" };
                for (int j = 0; j < 3; j++)
                {
                    Texture2D tex0 = layerToTex[j](layers[0]);
                    int width = tex0.width;
                    int height = tex0.height;
                    TextureFormat format = tex0.format;
                    bool hasMipMaps = false; //  tex0.mipmapCount > 1;

                    Texture2DArray textureArray = new Texture2DArray(width, height, layers.Count, format, hasMipMaps);
                    textureArray.anisoLevel = tex0.anisoLevel;
                    textureArray.filterMode = tex0.filterMode;
                    textureArray.wrapMode = tex0.wrapMode;
                    for (int i = 0; i < layers.Count; i++)
                    {

                        TerrainLayer layer = layers[i];
                        Texture2D tex = layerToTex[j](layer);
                        if (width != tex.width || height != tex.height || format != tex.format)
                        {
                            Debug.LogError("Layer " + layer + " has " + filenames[j] + " texture of different format width:" + width + " != " + tex.width + ", height:" + height + " != " + tex.height + " format:" + format + " != " + tex.format);
                            continue;
                        }


                        bool texHasMM = tex.mipmapCount > 1;
                        /*
                        if (tex.width != width || tex.height != height || tex.format!= format || hasMipMaps!= texHasMM)
                        {
                            tex.Reinitialize(width, height, format, hasMipMaps);
                        }
                        */
                        /*
                        for (int mip = 0; mip < tex.mipmapCount; mip++)
                        {
                            Graphics.CopyTexture(tex, 0, mip, textureArray, i, mip);
                        }
                        */
                        Graphics.CopyTexture(tex, 0, 0, textureArray, i, 0);
                        //Color[] pixels = tex.GetPixels();
                        //textureArray.SetPixels(pixels, i);
                    }
                    textureArray.Apply();
                    AssetDatabase.CreateAsset(textureArray, texArrayPath + "\\layer " + filenames[j] + ".asset");
                    AssetDatabase.SaveAssets();
                    saveLayer[j](textureArray, asset);
                }
                
                ProjectWindowUtil.CreateAsset(asset, texArrayPath+"\\TerrainLayers.asset");
            }

            
        }
    }
}
