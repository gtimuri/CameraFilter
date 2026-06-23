using System;
using Oculus.Interaction;
using UnityEngine;
using UnityEngine.Serialization;

public class HideObjectsLever : MonoBehaviour
{
    [SerializeField] private GameObject lever;
    [FormerlySerializedAs("objects")] 
    [SerializeField] private GameObject[] enableObjects;
    [SerializeField] private GameObject[] disableObjects;
    [SerializeField] private LakeBecomingBadSystem lakeBecomingBadSystem;

    private void Update()
    {
        if (lever.transform.localRotation.x <= .75f && !enableObjects[0].activeSelf)
        {
            lakeBecomingBadSystem.ResetTimer();
            foreach (var o in enableObjects) 
                o.SetActive(true);
            foreach (var o in disableObjects) 
                o.SetActive(false);
        }
        if (lever.transform.localRotation.x > .75f && enableObjects[0].activeSelf)
        {
            lakeBecomingBadSystem.ResetTimer();
            foreach (var o in enableObjects) 
                o.SetActive(false);
            foreach (var o in disableObjects) 
                o.SetActive(true);
        }
    }
}