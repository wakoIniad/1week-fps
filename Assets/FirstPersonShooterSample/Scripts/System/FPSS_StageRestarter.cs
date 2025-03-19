using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//ゲームをリスタートするもの

public class FPSS_StageRestarter : MonoBehaviour
{
    [SerializeField] float restartTime = 2.5f;
    [SerializeField] GameObject restartCanvas;
    [SerializeField] Image restartGauge;

    bool isClear;
    float nowtime = 0;

    //ゲームをはじめて最初に呼ばれる
    void Start()
    {
        restartCanvas.SetActive(false);
        nowtime = 0;
        isClear = false;
    }

    //毎フレーム呼ばれる
    void Update()
    {
        //ゲームをクリアしていた場合
        if (isClear) 
        {
            //リスタートのUIを表示してる時、非表示にし、終了
            if (nowtime > 0)
            {
                restartCanvas.SetActive(false);
                nowtime = 0;
            }
            return;
        }

        //ボタン入力
        bool input = false;
        input |= Input.GetKey(KeyCode.Tab);

        //リスタートボタンが押されていた時
        if (input)
        {
            //押し始めた時
            if(nowtime <= 0) 
            {
                restartCanvas.SetActive(true);
            }

            nowtime += Time.deltaTime;//押している時間
            restartGauge.transform.localScale = new Vector3(nowtime / restartTime,1,1);//ゲージの大きさを変える
            //restartGauge.fillAmountとかも調べてみるといいかもしれない

            //指定時間以上長押しした時
            if(nowtime >= restartTime) 
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
        else//押されていない時 
        {
            //押されていたら
            if(nowtime > 0) 
            {
                restartCanvas.SetActive(false);
                nowtime = 0;
            }
        }
    }
}
