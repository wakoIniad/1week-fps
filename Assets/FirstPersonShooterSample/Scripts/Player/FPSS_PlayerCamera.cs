using UnityEngine;

//プレイヤーのカメラを動かす

//縦方向の回転はプレイヤーで、
//横方向の回転はカメラで行っている
//カメラの回転角を制限することでカメラが一回転することを防ぐ

[RequireComponent(typeof(Camera))]
public class FPSS_PlayerCamera : MonoBehaviour
{   
    public TouchPad movePad;
    Vector2 lastPos = new Vector2(0,0);
    Vector2 refPos = new Vector2(0,0);
    [System.NonSerialized] public PlayerManager playerManager;
    public GameObject playerBody;//プレイヤー本体をいれておく
    public float speed = 2;//視点移動の速度
    public float angle = 130;//縦方向に視点を動かせる角度
    public bool reverseX = false;//横方向の向きを反転させるか
    public bool reverseY = false;//縦方向の向きを反転させるか
    [System.NonSerialized] public bool stop = false;


    float camRot;//現在のカメラの角度を入れておく
    //float plyrRot;
    

    Transform cameraTransform;
    Transform playerTransform;
    Rigidbody playerRigidbody;
    Camera playerCamera;


    //Singleton擬きの実装
    //他スクリプトから簡単にアクセスできるようになる
    //その代わり一つしか存在できない
    private static FPSS_PlayerCamera instance;
    float time = 0;
    float lastSynchronizedTime = 0;
    float lastSynchronizedAngleA = 0;
    float lastSynchronizedAngleB = 0;
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

    Vector2 lastTouch = new Vector2(0,0);
    int trackingTouchId = -1;
    //毎フレーム呼ばれる
    void Update()
    {
        
        float xInput = 0;
        float yInput = 0;
        time += Time.deltaTime;
        if(stop)return;
        //入力を取得
        if(GameManager.touchMode) {
            foreach (Touch touch in Input.touches)
            {
                Vector2 touchPos = touch.position;

                if (touch.phase == TouchPhase.Began)
                {
                    if (RectTransformUtility.RectangleContainsScreenPoint(movePad.rectTr, touchPos))
                    {
                        trackingTouchId = touch.fingerId;
                        Debug.Log("タッチ追跡開始");
                    }
                }

                if (touch.fingerId == trackingTouchId)
                {
                    if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                    {
                        // 追跡中のタッチの処理
                        Debug.Log("追跡中：" + touch.position);
                        
                        xInput = 0.1f * touch.deltaPosition.x; //0.05は感度
                        yInput = 0.1f * touch.deltaPosition.y; //0.05は感度
                    }

                    if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    {
                        trackingTouchId = -1;
                        Debug.Log("タッチ終了");
                    }
                }
            }
    
        } else {
            xInput = Input.GetAxis("Mouse X");
            yInput = Input.GetAxis("Mouse Y");
        }
        //Unity > ProjectSettings > InputManagerに設定がある
        Debug.Log(refPos);
        
        float plyrRot = 0;
        plyrRot = xInput * speed * (reverseX ? -1 : 1);
        camRot += -yInput * speed * (reverseY ? -1 : 1);

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
        if(
            time - lastSynchronizedTime > 0.5 && 
            ( Mathf.Abs(lastSynchronizedAngleA - camRot) > 10 ||
              Mathf.Abs(lastSynchronizedAngleB - plyrRot) > 10 )
        ) {
            playerManager.webSocketLoader.SendMyRotation();
            lastSynchronizedTime = time;
            lastSynchronizedAngleA = camRot;
            lastSynchronizedAngleB = plyrRot;
        }
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
