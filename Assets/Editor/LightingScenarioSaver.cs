using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class LightingScenarioSaver
{
    [MenuItem("[Lighting Scenario]/Save Current Lighting Scenario...")]
    public static void SaveCurrentScenario()
    {
        var path = EditorUtility.SaveFilePanelInProject(
            "Save Lighting Scenario",
            "LightingScenario",
            "asset",
            "Choose location to save lighting scenario asset");

        if (string.IsNullOrEmpty(path)) return;

        var data = ScriptableObject.CreateInstance<LightingScenarioData>();

        // Сохранить лайтмапы
        var lms = LightmapSettings.lightmaps;
        var colors = new List<Texture2D>();
        var dirs = new List<Texture2D>();
        var masks = new List<Texture2D>();

        foreach (var lm in lms)
        {
            if (lm == null) continue;
            colors.Add(lm.lightmapColor);
            dirs.Add(lm.lightmapDir);
            masks.Add(lm.shadowMask);
        }

        data.lightmapsColor = colors.ToArray();
        data.lightmapsDir = dirs.ToArray();
        data.shadowMasks = masks.ToArray();
        data.lightmapsMode = LightmapSettings.lightmapsMode;

        // Сохранить привязки ТОЛЬКО для террейнов (по ссылке на TerrainData)
        var terrains = Object.FindObjectsByType<Terrain>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        var terrainInfos = new List<LightingScenarioData.TerrainLightmapInfo>();
        foreach (var t in terrains)
        {
            if (t == null) continue;
            if (t.terrainData == null) continue;
            if (t.lightmapIndex < 0) continue; // нет baked GI
            terrainInfos.Add(new LightingScenarioData.TerrainLightmapInfo
            {
                terrainData = t.terrainData,
                lightmapIndex = t.lightmapIndex,
                lightmapScaleOffset = t.lightmapScaleOffset
            });
        }
        data.terrainInfos = terrainInfos.ToArray();

        AssetDatabase.CreateAsset(data, path);
        AssetDatabase.SaveAssets();
        EditorUtility.DisplayDialog("Lighting Scenario", $"Saved:\n{path}\n" +
            $"Lightmaps: {data.lightmapsColor?.Length} | Terrains: {data.terrainInfos?.Length}", "OK");
        Debug.Log($"Lighting Scenario saved: {path}");
    }
}
