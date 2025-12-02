// This script creates placeholder PBR materials for the Kochi pack.
// Supports both Standard (Built-in) and HDRP render pipelines.
// Place it under Assets/TimeLoopKochi/Materials/PBR/ and run via the menu.
using UnityEngine;
using UnityEditor;

public static class MaterialPackGenerator
{
    private static readonly string[] materialNames = new[]
    {
        "Road_Asphalt_Wet",
        "Road_Asphalt_Dry",
        "Concrete",
        "Brick",
        "TiledRoof",
        "Glass",
        "Water",
        "Soil",
        "MangroveMud",
        "VegetationGround"
    };

    [MenuItem("Tools/Time Loop City/Generate PBR Material Pack")]
    public static void GenerateMaterials()
    {
        string folderPath = "Assets/TimeLoopKochi/Materials/PBR/Generated";
        if (!AssetDatabase.IsValidFolder(folderPath))
            AssetDatabase.CreateFolder("Assets/TimeLoopKochi/Materials/PBR", "Generated");

        // Determine which render pipeline is active
        Shader shaderToUse = GetAppropriateShader();
        if (shaderToUse == null)
        {
            Debug.LogError("[Kochi Pack] Could not find a suitable shader! Ensure Standard shader is available.");
            return;
        }

        string shaderName = shaderToUse.name;
        bool isHDRP = shaderName.Contains("HDRP");

        foreach (var name in materialNames)
        {
            var mat = new Material(shaderToUse);
            mat.name = name;
            
            // Set properties based on shader type
            if (isHDRP)
            {
                mat.SetColor("_BaseColor", Color.gray);
                mat.SetFloat("_Smoothness", 0.7f);
                mat.SetFloat("_Metallic", 0f);
            }
            else
            {
                // Standard shader properties
                mat.SetColor("_Color", Color.gray);
                mat.SetFloat("_Glossiness", 0.7f);
                mat.SetFloat("_Metallic", 0f);
            }
            
            AssetDatabase.CreateAsset(mat, $"{folderPath}/{name}.mat");
        }
        
        AssetDatabase.SaveAssets();
        Debug.Log($"[Kochi Pack] Generated {materialNames.Length} placeholder PBR materials using {shaderName}.");
    }

    private static Shader GetAppropriateShader()
    {
        // Try HDRP first
        Shader hdrpShader = Shader.Find("HDRP/Lit");
        if (hdrpShader != null)
        {
            Debug.Log("[Kochi Pack] Using HDRP/Lit shader");
            return hdrpShader;
        }

        // Fallback to Standard shader
        Shader standardShader = Shader.Find("Standard");
        if (standardShader != null)
        {
            Debug.Log("[Kochi Pack] Using Standard shader (HDRP not available)");
            return standardShader;
        }

        return null;
    }
}
