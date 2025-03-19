using UnityEngine;

//敵の弾

public class FPSS_EnemyBullet : MonoBehaviour
{
    [SerializeField] float speed = 1;

    //毎フレーム呼ばれる
    void Update()
    {
        //移動させる
        transform.Translate(0, 0, speed * Time.deltaTime);
    }

    //何かにぶつかったら消える
    void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
