using System;
using UnityEngine;
using UnityEngine.UI;

public class GoodBadParkPortal : MonoBehaviour
{
    [SerializeField] private RawImage portalImage;
    [SerializeField] private RenderTexture goodTexture;
    [SerializeField] private RenderTexture badTexture;
    [Header("VRCamera")] [SerializeField] private Camera altWorldCamera;
    [SerializeField] private Camera[] changeCameras;
    [SerializeField] private LayerMask goodLayerMask;
    [SerializeField] private LayerMask badLayerMask;

    private static bool _isGood = true;
    private static event Action OnGoodUpdated;

    private static bool IsGood
    {
        get => _isGood;
        set
        {
            _isGood = value;
            OnGoodUpdated?.Invoke();
        }
    }

    private void OnEnable()
    {
        OnGoodUpdated += UpdateStatus;
    }

    private void OnDisable()
    {
        OnGoodUpdated -= UpdateStatus;
    }

    private void Start()
    {
        UpdateStatus();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        IsGood = !IsGood;
    }

    private void UpdateStatus()
    {
        SetTexture();
        SetCameras();
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