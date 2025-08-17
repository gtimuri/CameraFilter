using System;
using Oculus.Interaction;
using UnityEngine;

public class HideObjectsLever : MonoBehaviour
{
    [SerializeField] private GameObject lever;
    [SerializeField] private GameObject[] objects;
    [SerializeField] private LakeBecomingBadSystem lakeBecomingBadSystem;

    private void Update()
    {
        if (lever.transform.localRotation.x <= .75f && !objects[0].activeSelf)
        {
            lakeBecomingBadSystem.ResetTimer();
            SetVisible(true);
        }
        if (lever.transform.localRotation.x > .75f && objects[0].activeSelf)
        {
            lakeBecomingBadSystem.ResetTimer();
            SetVisible(false);
        }
    }

    private void SetVisible(bool value)
    {
        foreach (var o in objects) 
            o.SetActive(value);
    }
}