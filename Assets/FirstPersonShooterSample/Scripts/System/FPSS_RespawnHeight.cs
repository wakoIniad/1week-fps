using UnityEngine;

//リスポーンする高さを設定する
//これがついているものの高さ以下の時にリスポーン
//複数これがステージ上にある場合、どれが使われるかわからない

public class FPSS_RespawnHeight : MonoBehaviour
{

    //Singleton擬きの実装
    //他スクリプトから簡単にアクセスできるようになる
    private static FPSS_RespawnHeight instance;
    public static FPSS_RespawnHeight GetInstance()
    {
        return instance;
    }
    void Awake()
    {
        instance = this;
    }

    public float GetRespawnHeight()
    {
        return transform.position.y;
    }

}
