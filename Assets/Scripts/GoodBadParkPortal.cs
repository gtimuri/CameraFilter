using System;
using UnityEngine;
using UnityEngine.UI;

public class GoodBadParkPortal : MonoBehaviour
{
    [Header("VRCamera")] [SerializeField] private Camera altWorldCamera;
    [SerializeField] private Camera[] changeCameras;
    [SerializeField] private LayerMask goodLayerMask;
    [SerializeField] private LayerMask badLayerMask;
    [Space]
    [SerializeField] private MuteMixer[] muteMixers;

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
        foreach (var muteMixer in muteMixers)
        {
            muteMixer.audioSource.mute 
                = _isGood ? muteMixer.isMutedWhenGoodSide : !muteMixer.isMutedWhenGoodSide;
        }
        altWorldCamera.cullingMask = _isGood ? badLayerMask : goodLayerMask;
        foreach (var cam in changeCameras)
        {
            cam.cullingMask = _isGood ? goodLayerMask : badLayerMask;
        }
    }

    [Serializable]
    public class MuteMixer
    {
        public AudioSource audioSource;
        public bool isMutedWhenGoodSide;
    }
}