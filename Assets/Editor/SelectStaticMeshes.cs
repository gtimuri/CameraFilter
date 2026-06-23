using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class SelectStaticMeshes
{
    [MenuItem("[Select static meshes]/Select all static meshes")]
    public static void SelectStaticMeshRendererGIContributors() 
    {
        List<GameObject> staticMeshes = new List<GameObject>();
        foreach (MeshRenderer mr in Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None))
        {
            GameObject go = mr.gameObject;
            var staticFlags = GameObjectUtility.GetStaticEditorFlags(go);
            if ((staticFlags & StaticEditorFlags.ContributeGI) != 0)
            {
                staticMeshes.Add(go);
            }
        }

        if (staticMeshes.Count > 0)
        {
            Selection.objects = staticMeshes.ToArray();
            Debug.Log($"Выделено объектов: {staticMeshes.Count}");
        }
        else
        {
            Debug.LogWarning("Не найдено статичных объектов с MeshRenderer");
        }
    }
}
