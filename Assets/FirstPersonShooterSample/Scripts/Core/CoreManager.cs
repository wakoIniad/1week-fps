using UnityEngine;
using System;

public class CoreManager : MonoBehaviour
{
    private float nowHealth = 10;
    public string owner = "";
    public event Action OnBreak;
    public event Action<float> OnDamage;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    //ダメージを受けるとき
    void Damage(float damage)
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
        OnDamage.Invoke(nowHealth);
    }

    //体力が無くなったときに
    void Break()
    {
        Destroy(gameObject);
        OnBreak.Invoke();
        owner = null;
    }
    void OnTriggerEnter(Collision other) {
        if(other.gameObject.CompareTag("Weapon")) {
            WeaponSetting setting = other.gameObject.GetComponent<WeaponSetting>();
            if(setting) {
                Damage(setting.GetDamage());
            }
        }
    }
    
    public Vector3 GetPosition()
    {
        return transform.position;
    }
}
