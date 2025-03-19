using UnityEngine;
using System;

public class CoreObjectData : MonoBehaviour
{
    public CoreLoader loader;
    public float defaultHealth = 10;
    public bool owned = false;
    public float nowHealth = 10;
    public int id = 0;
    public bool transporting = false;
    
    public void Damage(float damage)
    {
        if(nowHealth <= 0){ return; }

        nowHealth -= damage;

        if(nowHealth <= 0)
        {
            //Break();
        }
    }

    //体力が無くなったときに
    void Break()
    {
        //if(owned) owned = false;
        //Destroy(gameObject);
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
    public int GetId() {
        return id;
    }

    //サーバーに送信する関数を呼び出す
    public void TryClaim() {
        loader.TryClaim(id);
    }
    public void TryDamage(float amount) {
        loader.TryDamage(id, amount);
    }
    public void TryWarp(Rigidbody rb) {
        if(owned) {
            Transform anchor = gameObject.GetComponent<Transform>();
            rb.position = anchor.position;
            rb.rotation = anchor.rotation;
            
        }
    }
    public void TryCollect(Transform tr) {
        if(owned) {
            Transform selfTr = gameObject.GetComponent<Transform>();
            selfTr.parent = tr;
            selfTr.localScale = new Vector3(0.1f,0.1f,0.1f);//元から小さくてもいいかも
            transporting = true;
        }
    }
    public void TryPlace() {
        if(transporting) {
            Transform selfTr = gameObject.GetComponent<Transform>();
            selfTr.parent = loader.loaderTransform;
            selfTr.localScale = new Vector3(1,1,1);
            transporting = false;
        }
    }
}
