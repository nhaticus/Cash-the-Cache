using UnityEngine;
using UnityEngine.EventSystems;

public class FlushButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] FlushingCanvas flushingCanvas;
    public void OnPointerDown(PointerEventData eventData)
    {
        flushingCanvas.PlungeButton();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        flushingCanvas.StopPlunge();
    }

}