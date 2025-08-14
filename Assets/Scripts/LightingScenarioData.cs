using System;
using UnityEngine;
// using UnityEngine.Rendering; // не требуется здесь

[CreateAssetMenu(fileName = "LightingScenario", menuName = "ScriptableObject/LightingScenario", order = 0)]
public class LightingScenarioData : ScriptableObject
{
    [Serializable]
    public struct TerrainLightmapInfo
    {
        public TerrainData terrainData;
        public int lightmapIndex;
        public Vector4 lightmapScaleOffset;
    }

    // Настройка окружения
    public Material skybox;
    public Color sunColor;
    public Color fogColor;
    
    // Наборы лайтмапов
    public Texture2D[] lightmapsColor;
    public Texture2D[] lightmapsDir;      // может быть null/пусто, если режим Non-Directional
    public Texture2D[] shadowMasks;       // может быть null/пусто, если без Shadowmask

    // Режим лайтмапов (NonDirectional / CombinedDirectional / SeparateDirectional)
    public UnityEngine.LightmapsMode lightmapsMode;

    // Привязка ТОЛЬКО террейнов к индексам/оффсетам в атласах лайтмапов
    public TerrainLightmapInfo[] terrainInfos;

    // Имя APV Lighting Scenario (URP Adaptive Probe Volumes) для этого набора света
	// Оставьте пустым, если не используете APV-сценарии
	public string apvScenarioName;
}