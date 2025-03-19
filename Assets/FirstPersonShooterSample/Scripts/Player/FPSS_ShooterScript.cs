using UnityEngine;

//銃を撃つ
//弾速の概念は無い (ヒットスキャンと呼ばれたり...)
//画面の中央になにかあるかを確認している

public class FPSS_ShooterScript : MonoBehaviour
{
    public int damage = 10;//ダメージ
    public GameObject hitParticlePrefab;//撃った場所に出現するパーティクル (エフェクト)
    public float rayMaxDistance = 100;//最大射程
    public LayerMask rayLayer = 0b0001;//判定するもののレイヤー設定
    public AudioSource shotAudioSource;//発射音をならすもの


    bool isHit;
    Ray ray;
    RaycastHit hit;

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
        //左クリックされたとき
        if(Input.GetMouseButtonDown(0))
        {
            //画面中央にあたる場所から出現するレイ(直線)を求める
            ray = playerCamera.GetCamera().ScreenPointToRay(new Vector3(Screen.width/2, Screen.height/2, 0));
            //レイを出してその先に何があるか調べる
            isHit = Physics.Raycast(ray, out hit, rayMaxDistance, rayLayer);
            //発射音を鳴らす
            shotAudioSource.Play(0);
            
            //何かあったとき
            if(isHit)
            {
                //何かのTagがEnemyだったとき
                if(hit.transform.CompareTag("Enemy"))
                {
                    //敵の体力を管理しているものがあるか
                    FPSS_EnemyHealth enemyHealth = hit.transform.GetComponent<FPSS_EnemyHealth>();
                    if(enemyHealth)
                    {
                        enemyHealth.Damage(damage);
                    }
                }
                if(hit.transform.CompareTag("Core"))
                {
                    //敵の体力を管理しているものがあるか
                    CoreObjectData coreObject = hit.transform.GetComponent<CoreObjectData>();
                    if(coreObject)
                    {
                        coreObject.TryDamage(coreObject.GetId());
                    }
                }
                //パーティクルを出現させる
                if(hitParticlePrefab)
                {
                    GameObject obj = Instantiate(hitParticlePrefab);
                    obj.transform.position = hit.point;
                    obj.transform.rotation = Quaternion.LookRotation(-hit.normal);
                }
            }
        }
    }
}
