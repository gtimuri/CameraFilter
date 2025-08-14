using UnityEngine;
using UnityEngine.Rendering;

public class ChangeDayNight : MonoBehaviour
{
    [Tooltip("Наборы выпеченного света (день/ночь и т.д.)")]
    public LightingScenarioData[] scenarios;

    public void ApplyScenario(int index)
    {
        if (scenarios == null || scenarios.Length == 0)
        {
            Debug.LogWarning("No scenarios assigned");
            return;
        }
        if (index < 0 || index >= scenarios.Length)
        {
            Debug.LogWarning("Scenario index out of range");
            return;
        }

        var s = scenarios[index];
        if (s == null)
        {
            Debug.LogWarning("Scenario asset is null");
            return;
        }

        RenderSettings.skybox = s.skybox;
        RenderSettings.fogColor = s.fogColor;
        RenderSettings.sun.color = s.sunColor;
        
        // Восстановить режим лайтмапов
        LightmapSettings.lightmapsMode = s.lightmapsMode;

        // Собрать массив LightmapData
        int count = s.lightmapsColor != null ? s.lightmapsColor.Length : 0;
        var newMaps = new LightmapData[count];
        for (int i = 0; i < count; i++)
        {
            var lm = new LightmapData
            {
                lightmapColor = SafeTex(s.lightmapsColor, i)
            };
            // directional и shadowmask могут отсутствовать
            var dir = SafeTex(s.lightmapsDir, i);
            if (dir != null) lm.lightmapDir = dir;
            var mask = SafeTex(s.shadowMasks, i);
            if (mask != null) lm.shadowMask = mask;

            newMaps[i] = lm;
        }

        LightmapSettings.lightmaps = newMaps;

        // Переключить APV Lighting Scenario (если указано имя)
        if (!string.IsNullOrEmpty(s.apvScenarioName))
        {
            var prv = ProbeReferenceVolume.instance;
            if (prv != null)
            {
                prv.lightingScenario = s.apvScenarioName;
            }
            else
            {
                Debug.LogWarning("APV ProbeReferenceVolume.instance is null. Убедитесь, что APV включён в URP Asset.");
            }
        }

        // Восстановить индексы/оффсеты только для террейнов по TerrainData
        if (s.terrainInfos != null)
        {
            var terrains = Terrain.activeTerrains;
            foreach (var ti in s.terrainInfos)
            {
                if (ti.terrainData == null) continue;
                for (int i = 0; i < terrains.Length; i++)
                {
                    var t = terrains[i];
                    if (t != null && t.terrainData == ti.terrainData)
                    {
                        t.lightmapIndex = ti.lightmapIndex;
                        t.lightmapScaleOffset = ti.lightmapScaleOffset;
                    }
                }
            }
        }

        // Обновить окружение для динамического GI/рефлексов
        DynamicGI.UpdateEnvironment();

        Debug.Log($"Applied lighting scenario #{index} ({s.name})");
    }

    private static Texture2D SafeTex(Texture2D[] arr, int i)
    {
        if (arr == null) return null;
        if (i < 0 || i >= arr.Length) return null;
        return arr[i];
    }

	// Плавный блендинг APV между текущим активным сценарием и целью (0..1)
	public void BlendApvTo(string targetScenarioName, float blend01, int cellsPerFrame = 16)
	{
		var prv = ProbeReferenceVolume.instance;
		if (prv == null)
		{
			Debug.LogWarning("APV ProbeReferenceVolume.instance is null");
			return;
		}
		prv.numberOfCellsBlendedPerFrame = Mathf.Max(1, cellsPerFrame);
		prv.BlendLightingScenario(targetScenarioName, Mathf.Clamp01(blend01));
	}
}
