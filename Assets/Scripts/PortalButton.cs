using System;
using UnityEngine;

public class PortalButton : MonoBehaviour
{
    public Camera shotCamera;
    [Header("Park")] public BoxCollider parkPortal;
    [Header("Lake")] public BoxCollider lakePortal;

    private RaycastHit[] _hits;

    private void Awake()
    {
        _hits = new RaycastHit[5];
    }

    public void ShootPortal()
    {
        if (Physics.RaycastNonAlloc(shotCamera.transform.position, 
                shotCamera.transform.forward, _hits, 100f) > 0)
        {
            foreach (var hit in _hits)
            {
                if (hit.collider != null && hit.collider.CompareTag("ParkTrigger"))
                {
                    parkPortal.gameObject.SetActive(true);
                }
                if (hit.collider != null && hit.collider.CompareTag("LakeTrigger"))
                {
                    lakePortal.gameObject.SetActive(true);
                }
            }
        }
    }
}