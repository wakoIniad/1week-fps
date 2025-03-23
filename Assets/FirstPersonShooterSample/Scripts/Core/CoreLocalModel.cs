using UnityEngine;
using System;
public class CoreLocalModel : MonoBehaviour
{
    [System.NonSerialized] public CoreLoader loader;
    //設定はサーバー側にあるため、変更次第書き換えるようにする。
    public static float defaultHealth;
    public static float warpCoolTime;
    public static float repairAmountOnPlacedPerSec = 1f;
    public static float repairAmountOnTransportingPerSec = 1.5f;
    private float repairFactorPerSec = 1f;
    public bool owned = false;
    public float nowHealth = -1;
    public string id;
    public bool transporting = false;
    public void SetHealth(float hp) {
        nowHealth = hp;
    }
    private float displayTimer = 0;
    void Update() {
        //Debug.Log(repairAmountOnPlacedPerSec+","+repairAmountOnTransportingPerSec+","+defaultHealth);
        //Debug.Log(CoreLocalModel.repairAmountOnPlacedPerSec+","+CoreLocalModel.repairAmountOnTransportingPerSec+","+CoreLocalModel.defaultHealth);
        if(owned && nowHealth < defaultHealth) {
            displayTimer += Time.deltaTime;
            //nowHealth += Time.deltaTime * repairFactorPerSec;
            if(displayTimer > 1f) {
                nowHealth += 1f * repairFactorPerSec;
                loader.ApplyHealth(id, nowHealth);
                displayTimer = 0;
            }
        }
    }
    /*public void Damage(float damage)
    {
        if(nowHealth <= 0){ return; }

        nowHealth -= damage;

        if(nowHealth <= 0)
        {
            //Break();
        }
    }*/

    //体力が無くなったときに
    void Break()
    {
        //if(owned) owned = false;
        //Destroy(gameObject);
    }

    public void SetId(string settingId) {
        id = settingId;
    }
    
    public Vector3 GetPosition()
    {
        return transform.position;
    }
    public void SetAsOwned() {
        owned = true;
        nowHealth = defaultHealth;
    }
    public void SetAsNotowned() {
        owned = false;
    }
    public string GetId() {
        return id;
    }
    public void SetPosition(Vector3 pos) {
            Transform anchor = gameObject.GetComponent<Transform>();
            anchor.position = pos;
    }
    public void SetAsPlaced() {
        Transform selfTr = gameObject.GetComponent<Transform>();
        selfTr.parent = loader.loaderTransform;
        selfTr.localScale = new Vector3(0.3f,0.3f,0.3f);
        transporting = false;
        
        UpdateFactor();
    }
    public void UpdateFactor() {
        repairFactorPerSec = transporting ? repairAmountOnTransportingPerSec: repairAmountOnPlacedPerSec;
        //切り替え時に、表示を更新
        loader.ApplyHealth(id, nowHealth);
    }
    public void SetTransprter(Transform tr) {
        Transform selfTr = gameObject.GetComponent<Transform>();
        selfTr.parent = tr;
        selfTr.localPosition = new Vector3(
            UnityEngine.Random.Range(-1f,1f),
            UnityEngine.Random.Range(0f,2f),
            UnityEngine.Random.Range(-1f,1f)
            );
        selfTr.localScale = new Vector3(0.1f,0.1f,0.1f);//元から小さくてもいいかも
        transporting = true;
        
        UpdateFactor();
    }

    //サーバーに送信する関数を呼び出す
    public void TryClaim() {
        loader.TryClaim(id);
    }
    public void TryDamage(float amount) {
        loader.TryDamage(id, amount);
    }
    //早さが求められるので、確認無しでワープした後プレイヤーの位置を更新するリクエストを送る
    /*public void TryWarp(Rigidbody rb) {
        if(owned) {
            Transform anchor = gameObject.GetComponent<Transform>();
            rb.position = anchor.position;
            rb.rotation = anchor.rotation;
            
        }
    }*/
    public void TryWarp() {
        loader.TryWarp(id);
    }
    public void TryCollect() {
        loader.TryCollect(id);
    }
        
    
    public void TryPlace() {
        loader.TryPlace(id);
    }
    public void TryRespawn() {
        loader.TryRespawn(id);
    }
}
