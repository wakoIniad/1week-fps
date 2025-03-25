using System;
using UnityEngine;

//プレイヤーを動かす

[RequireComponent(typeof(Rigidbody))]
public class FPSS_PlayerController : MonoBehaviour
{
    
    [System.NonSerialized] public PlayerManager playerManager;
    public float walkSpeed = 2;
    public float runSpeed = 4;
    public float jumpSpeed = 8;
    public LayerMask groundLayer = 0b0001;//地面として扱うものの設定

    //入力
    float xAxis;
    float yAxis;
    bool jumpInput;
    bool dashInput;

    //状態
    [System.NonSerialized] public bool isGround;//地上に立っているか

    //その他
    float moveSpeed;//現在の移動速度を入れておく
    [System.NonSerialized] public Rigidbody rb;//PlayerのRigidbodyを入れておくもの
    RaycastHit groundHit;//地面確認時の結果を入れておくもの

    public float dashInputSpeed = 0.5f;
    float dashInputTimer = -1f;

    [System.NonSerialized] public bool stop;
    float time = 0;
    float lastSynchronizedTime = 0;
    Vector3 lastSynchronizetPosition;


    //ゲームをはじめて最初に呼ばれる
    void Start()
    {
        //必要なものを取得
        rb = gameObject.GetComponent<Rigidbody>();
    }


    //毎フレーム呼ばれる
    void Update()
    {   
        time += Time.deltaTime;
        if(stop)return;
        //機能ごとに分けてわかりやすく
        UpdateGround();
        UpdateInput();
        UpdateMove();
        if(xAxis != 0 || yAxis != 0) {
            if(
                time - lastSynchronizedTime >= 0.5 &&
                lastSynchronizetPosition != null 
                ?Vector3.Distance(lastSynchronizetPosition, transform.position) > 0.1f
                :true
            ) {
                playerManager.webSocketLoader.SendMyPosition();
                lastSynchronizedTime = time;
                lastSynchronizetPosition = transform.position;
            }
        }

    }


    //地面に立っているかなど
    void UpdateGround() 
    {
        //プレイヤーの下に何かあるか確認
        //何かあったら地面に立っていることにする
        isGround = Physics.Raycast(transform.position + transform.up * 0.1f, -transform.up, out groundHit, 0.2f, groundLayer);
    }


    //入力を取得
    void UpdateInput() 
    {
        //横方向の移動を-1～+1の値で表す
        xAxis = 0;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            xAxis -= 1;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            xAxis += 1;
        }

        //前方向の移動を-1～+1の値で表す
        yAxis = 0;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            yAxis -= 1;
        }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            yAxis += 1;
            //if(dashInputTimer != -1 && dashInputTimer < dashInputSpeed) {
            //    dashInput = true;
            //}
            //dashInputTimer = 0;
        }// else {
        //    dashInput = false;
        //}
        //
        //if(dashInputTimer != -1) {
        //    dashInputTimer += Time.deltaTime;
        //}
        //if(dashInputTimer >= dashInputSpeed) {
        //    dashInputTimer = -1f;
        //}

        //ジャンプボタンを押した瞬間か
        jumpInput = Input.GetKeyDown(KeyCode.Space);

        //ダッシュボタンを押しているか
        dashInput = Input.GetKey(KeyCode.LeftShift);
    }


    //歩行に関すること
    void UpdateMove() 
    {
        //取得した入力を正規化する
        //正規化しないと斜め移動の速度が1.4倍になる
        Vector2 moveVector = new Vector2(xAxis,yAxis).normalized;

        //ダッシュボタンを押しているかで移動速度を変える
        if (dashInput)
        {
            moveSpeed = runSpeed;
        }
        else 
        {
            moveSpeed = walkSpeed;
        }

        //移動させる
        rb.position += transform.right * moveVector.x * moveSpeed * Time.deltaTime;
        rb.position += transform.forward * moveVector.y * moveSpeed * Time.deltaTime;
        //rb.AddForce(
        //    transform.right * moveVector.x * moveSpeed * Time.deltaTime*10, 
        //    ForceMode.VelocityChange
        //);
        //rb.AddForce(
        //    transform.forward * moveVector.y * moveSpeed * Time.deltaTime*10, 
        //    ForceMode.VelocityChange
        //);
        //地上にいてジャンプボタンが押されたとき
        if(jumpInput && isGround)
        {
            //ジャンプさせる
            rb.AddForce(new Vector3(0, jumpSpeed, 0), ForceMode.VelocityChange);
        }

    }

}
