using System;
using UnityEngine;

public class PhysicsButton : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public Transform offset;
    public float offsetLength;

    private Vector3 _initialPosition;


    private void Awake()
    {
        _initialPosition = offset.localPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        var currentTransform = _initialPosition;
        currentTransform.y -= offsetLength;
        offset.transform.localPosition = currentTransform;
    }

    private void OnTriggerExit(Collider other)
    {
        offset.transform.localPosition = _initialPosition;
    }
}