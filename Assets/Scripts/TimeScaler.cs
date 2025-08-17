using UnityEngine;

public class TimeScaler : MonoBehaviour
{
    [SerializeField] private float scale = 2f;

    private bool _active;
    
    public void Scale()
    {
        _active = !_active;
        Time.timeScale = _active ? scale : 1;
    }
}