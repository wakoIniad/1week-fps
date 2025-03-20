using UnityEngine;
using System;

public class CoreLocalModel : MonoBehaviour
{
    [System.NonSerialized]public CoreLoader loader;
    public float defaultHealth = 10;
    public bool owned = false;
    public float nowHealth = 10;
    public string id;
    public bool transporting = false;
    
    public void SetHealth(float hp) {
        nowHealth = hp;
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
        selfTr.localScale = new Vector3(1,1,1);
        transporting = false;
    }
    public void SetTransprter(Transform tr) {
        Transform selfTr = gameObject.GetComponent<Transform>();
        selfTr.parent = tr;
        selfTr.localScale = new Vector3(1f,1f,1f);//元から小さくてもいいかも
        transporting = true;
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
