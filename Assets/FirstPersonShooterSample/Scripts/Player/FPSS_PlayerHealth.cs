using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;


//プレイヤーの体力

public class FPSS_PlayerHealth : MonoBehaviour
{
    PlayerRespown respownManager;
    public int playerHealth = 10;
    public TMP_Text healthText;

    float nowHealth;//現在の体力を入れておく

    //ゲームをはじめて最初に呼ばれる
    void Start()
    {
        //体力を設定
        nowHealth = playerHealth;
        //表示を更新する
        healthText.text = "Health: " + nowHealth.ToString();
        respownManager = gameObject.GetComponent<PlayerRespown>();
    }

    //マルチプレイの都合上、Damageをこっちに変える
    public void SetHealth(float hp) {
        nowHealth = hp;
        //表示を更新する
        healthText.text = "Health: " + nowHealth.ToString();
        //ここはサーバー側の処理に変える
        if(nowHealth <= 0)
        {
            Death();
        }
    }
    //ダメージを受けるとき
    public void Damage(int damage)
    {
        //既に体力が無いときこの先の処理をしない
        if(nowHealth <= 0){ return; }

        //体力を減らす
        nowHealth -= damage;
        //表示を更新する
        healthText.text = "Health: " + nowHealth.ToString();

        //体力が無くなったらその処理をする
        if(nowHealth <= 0)
        {
            Death();
        }
    }

    //体力が無くなったときに
    public void Death()
    {
        nowHealth = playerHealth;
        respownManager.StartHandleRespown();
    }

    //すり抜ける当たり判定にあたったとき
    void OnTriggerEnter(Collider other)
    {
        //ぶつかったもののTagがEnemyだったとき
        if(other.CompareTag("Enemy"))
        {
            //敵用攻撃判定があるか
            FPSS_EnemyAttacker enemyAttacker = other.gameObject.GetComponent<FPSS_EnemyAttacker>();
            //攻撃判定があったらダメージ処理をする
            if(enemyAttacker)
            {
                Damage(enemyAttacker.GetDamage());
            }
        }
    }

}
