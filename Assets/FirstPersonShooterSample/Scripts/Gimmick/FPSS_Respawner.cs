using UnityEngine;

//一定以下の高さになったら最初の場所へ移動する

public class FPSS_Respawner : MonoBehaviour
{
    //自動的にリスポーンする高さ
    [SerializeField] float autoRespownHeight = -1000;

    Vector3 startPosition;
    Quaternion startRotation;
    FPSS_RespawnHeight rh;
    Rigidbody rb;

    //ゲームをはじめて最初に呼ばれる
    void Start()
    {
        //リスポーンする高さを決めるものを取得
        rh = FPSS_RespawnHeight.GetInstance();
        //Rigidbodyを取得
        //(無い時はnullになる)
        rb = GetComponent<Rigidbody>();

        //初期位置、向きを保存
        //startPosition = transform.position;
        //startRotation = transform.rotation;
    }
    public void setRespownAnchor(Transform anchor) {
        startPosition = anchor.position;
        startRotation = anchor.rotation;
    }

    //毎フレーム呼ばれる
    void Update()
    {
        //指定した高さより低い場所にいる時
        if(rh)
        {
            if (transform.position.y <= rh.GetRespawnHeight())
            {
                Respawn();
                return;
            }
        }
        else//RespawnHeightがなかった時
        {
            if (transform.position.y <= autoRespownHeight) 
            {
                Respawn();
                return;
            }
        }
    }

    //リスポーンする
    public void Respawn() 
    {
        transform.position = startPosition;
        transform.rotation = startRotation;

        //Rigidbodyがある時
        //(nullじゃない時)
        if (rb != null)
        {
            //速さを0にする
            rb.linearVelocity = Vector3.zero;
        }
    }
}
