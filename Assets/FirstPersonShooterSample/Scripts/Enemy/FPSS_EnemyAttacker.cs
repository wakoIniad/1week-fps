using UnityEngine;

//敵の攻撃をするもの

public class FPSS_EnemyAttacker : MonoBehaviour
{
    [SerializeField] int damage = 1;

    public int GetDamage()
    {
        return damage;
    }
}
