using UnityEngine;

//時間経過でオブジェクトを消す

public class FPSS_AutoDestroyer : MonoBehaviour
{
    [SerializeField] float destroyTime = 10;

    float nowtime = 0;
    
    //ゲームをはじめて最初に呼ばれる
    void Start()
    {
        nowtime = 0;
    }

    //毎フレーム呼ばれる
    void Update()
    {
        //経過時間を追加
        nowtime += Time.deltaTime;
        //指定時間以上になったら消去
        if(nowtime >= destroyTime)
        {
            Destroy(gameObject);
        }
    }
}
