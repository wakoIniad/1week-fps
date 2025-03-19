using UnityEngine;

public class CoreManager : MonoBehaviour
{
    private GameObject coreObject;
    private float nowHealth = 10;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        coreObject = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    //ダメージを受けるとき
    public void Damage(int damage)
    {
        //既に体力が無いときこの先の処理をしない
        if(nowHealth <= 0){ return; }

        //体力を減らす
        nowHealth -= damage;

        //体力が無くなったらその処理をする
        if(nowHealth <= 0)
        {
            Break();
        }
    }

    //体力が無くなったときに
    void Break()
    {
    }
    void OnTriggerEnter(Collision other) {
        if(other.gameObject.CompareTag("Weapon")) {
            WeaponSetting setting = other.gameObject.GetComponent<WeaponSetting>();
        }
    }
}
