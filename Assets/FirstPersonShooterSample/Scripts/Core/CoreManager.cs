using UnityEngine;
using System;

public class CoreManager : MonoBehaviour
{
    public float defaultHealth = 10;
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
    
    public void Damage(float damage)
    {
        if(nowHealth <= 0){ return; }

        nowHealth -= damage;

        if(nowHealth <= 0)
        {
            Break();
        }
        if(OnDamage != null) OnDamage.Invoke(nowHealth);
    }

    //体力が無くなったときに
    void Break()
    {
        if(OnBreak != null)OnBreak.Invoke();
        owner = "";
        //Destroy(gameObject);
    }
    void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("EnemyBullet")) {
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
    public void ChangeOwner(string ownerName) {
        this.owner = ownerName;
        nowHealth = defaultHealth;
    }
}
