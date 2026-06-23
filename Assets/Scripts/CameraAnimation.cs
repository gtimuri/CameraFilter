using UnityEngine;

public class CameraShooter : MonoBehaviour
{
    public Animator cameraAnimator;
    private bool isZoomed = false;

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            cameraAnimator.SetTrigger("Shoot");
        }
        
        if (OVRInput.GetDown(OVRInput.Button.One)) 
        {
            if (!isZoomed)
            {
                cameraAnimator.SetTrigger("ZoomIn");
            }
            else
            {
                cameraAnimator.SetTrigger("ZoomOut");
            }

            isZoomed = !isZoomed;
        }
    }
}