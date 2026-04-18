using Env.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ProcEnvMenuItems
{

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
                int mipMaps = tex0.mipmapCount;

                Texture2DArray textureArray = new Texture2DArray(width, height, layers.Count, format, mipMaps > 1);
                textureArray.anisoLevel = tex0.anisoLevel;
                textureArray.filterMode = tex0.filterMode;
                textureArray.wrapMode = tex0.wrapMode;
                for (int i = 0; i < layers.Count; i++)
                {

                    TerrainLayer layer = layers[i];
                    Texture2D tex = layerToTex[j](layer);
                    if (width != tex.width || height != tex.height || format != tex.format || mipMaps != tex.mipmapCount)
                    {
                        Debug.LogError("Layer " + layer + " has " + filenames[j] + " texture of different format width:" + width + " == " + tex.width + ", height:" + height + " == " + tex.height + " format:" + format + " == " + tex.format + " mipMaps: " + mipMaps + " == " + tex.mipmapCount);
                        continue;
                    }


                    //bool texHasMM = tex.mipmapCount > 1;
                    /*
                    if (tex.width != width || tex.height != height || tex.format!= format || hasMipMaps!= texHasMM)
                    {
                        tex.Reinitialize(width, height, format, hasMipMaps);
                    }
                    */

                    for (int mip = 0; mip < tex.mipmapCount; mip++)
                    {
                        Graphics.CopyTexture(tex, 0, mip, textureArray, i, mip);
                    }

                    //Graphics.CopyTexture(tex, 0, 0, textureArray, i, 0);
                    //Color[] pixels = tex.GetPixels();
                    //textureArray.SetPixels(pixels, i);
                }
                textureArray.Apply();
                AssetDatabase.CreateAsset(textureArray, texArrayPath + "\\layer " + filenames[j] + ".asset");
                AssetDatabase.SaveAssets();
                saveLayer[j](textureArray, asset);
            }

            ProjectWindowUtil.CreateAsset(asset, texArrayPath + "\\TerrainLayers.asset");
        }


    }


    [MenuItem("Assets/Create/Proc Env/Instanced Mesh From Selected Asset")]
    static void CreateInstancedMeshAssetFile()
    {
        InstanceableObjectAsset asset = ScriptableObject.CreateInstance<InstanceableObjectAsset>();


        if (Selection.activeObject is Mesh)
        {
            Mesh m = (Mesh)Selection.activeObject;
            asset.LODs = new InstanceableObject(m);
        }
        else if (Selection.activeObject is GameObject)
        {
            GameObject go = (GameObject)Selection.activeObject;
            MeshRenderer mesh = go.GetComponent<MeshRenderer>();
            if (mesh != null)
            {
                asset.LODs = new InstanceableObject(mesh);
            }
            else
            {
                LODGroup lods = go.GetComponent<LODGroup>();
                if (lods != null)
                {
                    asset.LODs = new InstanceableObject(lods);
                }
            }

        }

        String path = AssetDatabase.GetAssetPath(Selection.activeObject);
        String assetPath = Path.GetFileNameWithoutExtension(path) + " instanced.asset";

        ProjectWindowUtil.CreateAsset(asset, assetPath);
    }
}
