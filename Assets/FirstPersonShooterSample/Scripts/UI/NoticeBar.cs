using TMPro;
using UnityEngine;

public class NoticeBar : MonoBehaviour
{
    public GameObject noticeBarGameObject;
    public TextMeshProUGUI textMeshPro;
    public float animationTime = 1.5f;
    RectTransform rectTr;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    bool processing = false;
    float time = 0;
    float startXPos;
    void Update() {
        if(processing) {
            time += Time.deltaTime;
            float frame = time/animationTime;
            rectTr.position = new Vector3(
                startXPos-frame*rectTr.sizeDelta.x,
                rectTr.position.y,
                rectTr.position.z
            );
            if(time > animationTime)processing = false;
        }
    }
    private bool alreadtDisplayed = false;
    public void StartAnimation(string text) {
        //WebSocket 接続ステータス用のみのための1度のみ表示機能（alreadtDisplayed)
        if(alreadtDisplayed)return;
        alreadtDisplayed = true;
        processing = true;
        time = 0;
        rectTr = noticeBarGameObject.GetComponent<RectTransform>();
        rectTr.position = new Vector3(
                rectTr.position.x+rectTr.sizeDelta.x,
                rectTr.position.y,
                rectTr.position.z
        );
        textMeshPro.text = text;
        noticeBarGameObject.SetActive(true);
        startXPos = rectTr.position.x;
    }
}
