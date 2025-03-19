using UnityEngine;

//常にプレイヤーの方向を向く

public class FPSS_TurretEnemy : MonoBehaviour
{
    //タレットのプレイヤーの方向に向かせたいもの
    [SerializeField] Transform headTransform;

    FPSS_PlayerCamera playerCamera;

    //ゲームをはじめて最初に呼ばれる
    void Start()
    {
        //プレイヤーのカメラを取得
        playerCamera = FPSS_PlayerCamera.GetInstance();
    }

    //毎フレーム呼ばれる
    void Update()
    {
        //プレイヤーのカメラの位置(頭の位置)を取得
        Vector3 playerHeadPosition = playerCamera.GetCameraPosition();
        //タレットからプレイヤーのカメラへの方向を計算
        Vector3 toPlayerHeadVector = (playerHeadPosition - headTransform.position).normalized;
        //プレイヤーのカメラの方向に向く
        headTransform.rotation = Quaternion.LookRotation(toPlayerHeadVector);
    }
}
