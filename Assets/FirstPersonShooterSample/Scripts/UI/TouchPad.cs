using UnityEngine;
using UnityEngine.EventSystems;

public class TouchPad : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool isHeld = false; //押し続けてる間
    public bool touchEnd = false;
    public bool touched = false; //1frameのみ

    public void OnPointerDown(PointerEventData eventData)
    {
        isHeld = true;
        touched = true;
        Debug.Log("ボタンが押された！");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHeld = false;
        touchEnd = true;
        Debug.Log("ボタンが離された！");
    }
    void LateUpdate() {
        if(touched || touchEnd)Debug.Log("TouchPad:"+touchEnd+","+touched);
        if(touched) touched = false;
        if(touchEnd) touchEnd = false;
    }
}