using UnityEngine;

public class PhysicsButton : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public Transform offset;
    public float offsetLength;
    public Camera shotCamera;
    [Header("Park")] 
    public BoxCollider parkPortal;
    [Header("Day/Night")] 
    public ChangeDayNight changeDayNight;
    
    private Vector3 _initialPosition;
    private RaycastHit[] _hits;

    private int _currentLightIndex;
    
    private void Awake()
    {
        _initialPosition = offset.localPosition;
        _hits = new RaycastHit[5];
    }

    private void OnTriggerEnter(Collider other)
    {
        var currentTransform = _initialPosition;
        currentTransform.y -= offsetLength;
        offset.transform.localPosition = currentTransform;

        _currentLightIndex = _currentLightIndex == 0 ? 1 : 0;
        changeDayNight.ApplyScenario(_currentLightIndex);
        
        if (Physics.RaycastNonAlloc(shotCamera.transform.position, shotCamera.transform.forward, _hits, 100f) > 0)
        {
            Debug.Log("START");
            foreach (var hit in _hits)
            {
                if (hit.collider != null && hit.collider.CompareTag("ParkTrigger"))
                {
                    Debug.Log("TEST");
                    parkPortal.gameObject.SetActive(true);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        offset.transform.localPosition = _initialPosition;
    }
}
