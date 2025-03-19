using UnityEngine;

//マウスカーソルのOn/Offを切り替える

public class ToggleCursor : MonoBehaviour
{
    [SerializeField] bool showCursor = false;

    //ゲームをはじめて最初に呼ばれる
    void UpdateCursor()
    {
        //マウスカーソルの設定
        if (showCursor)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    void Start() {
        UpdateCursor();
    }
    public void ShowCursor() {
        showCursor = true;
        UpdateCursor();
    }
    public void HideCursor() {
        showCursor = false;
        UpdateCursor();
        
    }
}
