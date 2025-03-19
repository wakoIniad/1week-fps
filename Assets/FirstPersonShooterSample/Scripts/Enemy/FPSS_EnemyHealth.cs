using UnityEngine;

//敵の体力

public class FPSS_EnemyHealth : MonoBehaviour
{
    [SerializeField] int maxHealth = 10;//最大体力
    [SerializeField] GameObject enemyParentObject;//敵の本体になるモノ
    [SerializeField] GameObject deathObject;//倒されたときに出現させるモノ

    int nowHealth;

    //ゲームをはじめて最初に呼ばれる
    void Start ()
    {
        nowHealth = maxHealth;
    }

    //ダメージを受けるときに呼ぶ
    public void Damage(int damage)
    {
        //既に体力が無いときこの先の処理をしない
        if(nowHealth <= 0){ return; }

        //体力を減らす
        nowHealth -= damage;

        //体力が無くなったらその処理をする
        if(nowHealth <= 0)
        {
            Death();
        }
    }

    //体力が無くなったときに呼ぶ
    void Death()
    {
        //倒されたとき用のモノを出現させる
        if(deathObject)
        {
            GameObject obj = Instantiate(deathObject);
            obj.transform.position = transform.position;
        }
        //敵を消す
        if(enemyParentObject)
        {
            Destroy(enemyParentObject);
        }else
        {
            Destroy(gameObject);
        }
    }
}
