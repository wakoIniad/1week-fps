using UnityEngine;

//プレイヤーのカメラを動かす

//縦方向の回転はプレイヤーで、
//横方向の回転はカメラで行っている
//カメラの回転角を制限することでカメラが一回転することを防ぐ

[RequireComponent(typeof(Camera))]
public class FPSS_PlayerCamera : MonoBehaviour
{
    public GameObject playerBody;//プレイヤー本体をいれておく
    public float speed = 2;//視点移動の速度
    public float angle = 130;//縦方向に視点を動かせる角度
    public bool reverseX = false;//横方向の向きを反転させるか
    public bool reverseY = false;//縦方向の向きを反転させるか


    float camRot;//現在のカメラの角度を入れておく
    

    Transform cameraTransform;
    Transform playerTransform;
    Rigidbody playerRigidbody;
    Camera playerCamera;


    //Singleton擬きの実装
    //他スクリプトから簡単にアクセスできるようになる
    //その代わり一つしか存在できない
    private static FPSS_PlayerCamera instance;
    public static FPSS_PlayerCamera GetInstance()
    {
        return instance;
    }

    //Startよりも先に呼ばれる
    void Awake()
    {
        instance = this;
    }
    

    //ゲームをはじめて最初に呼ばれる
    void Start()
    {
        playerCamera = gameObject.GetComponent<Camera>();
        cameraTransform = playerCamera.transform;

        playerTransform = playerBody.transform;
        playerRigidbody = playerBody.GetComponent<Rigidbody>();
    }

    //毎フレーム呼ばれる
    void Update()
    {
        //入力を取得
        //Unity > ProjectSettings > InputManagerに設定がある
        float xInput = Input.GetAxis("Mouse X");
        float yInput = -Input.GetAxis("Mouse Y");

        float plyrRot = 0;

        //マウスが動いたぶん角度を変更
        plyrRot = xInput * speed * (reverseX ? -1 : 1);
        camRot += yInput * speed * (reverseY ? -1 : 1);

        //カメラの角度を制限する
        camRot = Mathf.Clamp(camRot, -angle/2, angle/2);

        //角度を適用
        if(playerRigidbody)
        {
            playerRigidbody.rotation *= Quaternion.AngleAxis(plyrRot, Vector3.up);
        }else
        {
            playerTransform.rotation *= Quaternion.AngleAxis(plyrRot, Vector3.up);
        }

        cameraTransform.localRotation = Quaternion.AngleAxis(camRot, Vector3.right);
    }

    public Camera GetCamera()
    {
        return playerCamera;
    }

    public Vector3 GetCameraPosition()
    {
        return cameraTransform.position;
    }
}
