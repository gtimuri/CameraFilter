using UnityEngine;

public class SimpleVRButton : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Obj used the button" + other.name);
        
        if (other.name.Contains("Hand") || other.name.Contains("Controller"))
        {
            Debug.Log("BUTTON PRESSED");
            // TRIGGER ANY ACTION FURTHER
        }
    }
}