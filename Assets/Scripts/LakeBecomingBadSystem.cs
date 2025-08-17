using System;
using UnityEngine;

public class LakeBecomingBadSystem : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private TimeSlot[] timeSlots;
    [SerializeField] private float maxTime;
    
    private float _timer;
    private int _curIdx;
    private bool _asc;

    private void Start()
    {
        _asc = false;
        _timer = maxTime;
        _curIdx = timeSlots.Length - 1;
        Sort();
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_curIdx < timeSlots.Length)
        {
            if (_asc)
            {
                if (_timer >= timeSlots[_curIdx].time)
                {
                    if (timeSlots[_curIdx].material)
                        meshRenderer.material = timeSlots[_curIdx].material;
                    foreach (var o in timeSlots[_curIdx].objects) 
                        o.SetActive(true);
                    _curIdx += 1;
                }
            }
            else
            {
                if (maxTime - _timer <= timeSlots[_curIdx].time)
                {
                    if (timeSlots[_curIdx].material)
                        meshRenderer.material = timeSlots[_curIdx].material;
                    foreach (var o in timeSlots[_curIdx].objects) 
                        o.SetActive(false);
                    _curIdx += 1;
                }
            }
        }
        
        if (_timer >= maxTime)
            _timer = maxTime;
    }

    public void ResetTimer()
    {
        _timer = 0;
        _asc = !_asc;
        Sort();
        _curIdx = 0;
    }

    private void Sort()
    {
        if (_asc)
            Array.Sort(timeSlots,(a, b) => a.order.CompareTo(b.order));
        else
            Array.Sort(timeSlots,(a, b) => b.order.CompareTo(a.order));
    }
    
    [Serializable]
    public class TimeSlot
    {
        public int order;
        public float time;
        public Material material;
        public GameObject[] objects;
    }
}