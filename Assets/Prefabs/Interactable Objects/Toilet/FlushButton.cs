using UnityEngine;
using UnityEngine.EventSystems;

public class FlushButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] GameObject canvas;
    public bool SENDIT = false;
    public void OnPointerDown(PointerEventData eventData)
    {
        SENDIT = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        SENDIT = false;
    }

    private void Update()
    {
        if (SENDIT) canvas.GetComponent<FlushingCanvas>().FlushButton();
    }
}