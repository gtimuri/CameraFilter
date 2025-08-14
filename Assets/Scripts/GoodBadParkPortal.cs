using System;
using UnityEngine;
using UnityEngine.UI;

public class GoodBadParkPortal : MonoBehaviour
{
    [SerializeField] private RawImage portalImage;
    [SerializeField] private RenderTexture goodTexture;
    [SerializeField] private RenderTexture badTexture;
    [Header("VRCamera")] 
    [SerializeField] private Camera altWorldCamera;
    [SerializeField] private Camera[] changeCameras;
    [SerializeField] private LayerMask goodLayerMask;
    [SerializeField] private LayerMask badLayerMask;

    private bool _isGood = true;

    private void Start()
    {
        SetTexture();
        SetCameras();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Player"))
            return;
        
        _isGood = !_isGood;
        SetCameras();
        SetTexture();
    }

    private void SetTexture()
    {
        portalImage.texture = _isGood ? badTexture : goodTexture;
    }

    private void SetCameras()
    {
        altWorldCamera.cullingMask = _isGood ? badLayerMask : goodLayerMask;
        foreach (var cam in changeCameras)
        {
            cam.cullingMask = _isGood ? goodLayerMask : badLayerMask;
        }
    }
}
