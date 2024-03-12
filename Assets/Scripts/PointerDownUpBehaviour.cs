using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerDownUpBehaviour : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
{
    private bool _isPointerDown;
    public event Action<PointerDownUpBehaviour, bool, bool> onIsPointerDownChanged;

    public bool IsPointerDown
    {
        get => _isPointerDown;
        private set
        {
            bool oldValue = _isPointerDown;
            bool isChanged = _isPointerDown != value;
            _isPointerDown = value;
            if (isChanged)
            {
                onIsPointerDownChanged?.Invoke(this, oldValue, value);
            }
        }
    }
    public event Action<PointerEventData> onPointerDown;
    public event Action<PointerEventData> onPointerUp;

    private void OnDisable()
    {
        _isPointerDown = false;
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        IsPointerDown = true;
        onPointerDown?.Invoke(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsPointerDown = false;
        onPointerUp?.Invoke(eventData);
    }
}
