using UnityEngine;

public class FPSS_PlayerPosition : MonoBehaviour
{
    //Singleton擬きの実装
    //他スクリプトから簡単にアクセスできるようになる
    //その代わり一つしか存在できない
    private static FPSS_PlayerPosition instance;
    public static FPSS_PlayerPosition GetInstance()
    {
        return instance;
    }

    //Startよりも先に呼ばれる
    void Awake()
    {
        instance = this;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}
