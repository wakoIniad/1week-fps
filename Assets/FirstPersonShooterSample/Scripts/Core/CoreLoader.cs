using UnityEngine;
using System;
using System.Collections.Generic;

public class CoreLoader : MonoBehaviour
{
    //public string playerId = "temp";
    public event Action<string, float> OnOwnedCoreDamaged;
    public event Action<string> OnOwnedCoreBreaked;
    public event Action<string> OnCoreOwned;
    public Transform loaderTransform;
    public GameObject CoreObjectPrefab;
    
    public Dictionary<string, CoreLocalModel> CoreList = new Dictionary<string, CoreLocalModel>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
   {
        loaderTransform = gameObject.GetComponent<Transform>();
        

    }
    public bool isOwned(string? id) {
        if(id is string t) {
            return CoreList[t].owned;
        } else {
            return false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public CoreLocalModel GetCoreById(string id) {
        return CoreList[id];
    }
    /*public CoreObjectData[] GetOwnedCores() {
        List<CoreObjectData> result = new List<CoreObjectData>();
        for(int i = 0;i < CoreList.Count; i++) {
            if(CoreList[i].owned)result.Add(CoreList[i]);
        }
        return result.ToArray();
    }*/
    //apply server data to local data
    /*public void ApplyClaimData(int targetCoreId, string ownerId) {
        if(ownerId == playerId) {
            CoreList[targetCoreId].SetAsOwned();
            if(OnCoreOwned != null)OnCoreOwned.Invoke(targetCoreId);
        }
    }*/
    public void ApplyOwned(string targetCoreId) {
            CoreList[targetCoreId].SetAsOwned();
            if(OnCoreOwned != null)OnCoreOwned.Invoke(targetCoreId);
    }
    public void ApplyDamageData(string targetCoreId, float damage) {
        CoreList[targetCoreId].Damage(damage);
        if(CoreList[targetCoreId].owned) {
            if(OnOwnedCoreDamaged != null)OnOwnedCoreDamaged.Invoke(targetCoreId, CoreList[targetCoreId].nowHealth);
        }
    }
    public void ApplyBreakData(string targetCoreId) {
        if(CoreList[targetCoreId].owned) {
            if(OnOwnedCoreBreaked != null)OnOwnedCoreBreaked.Invoke(targetCoreId);
        }
        //プライヤーが移動中の場合落ちる  -> すぐ拾えちゃうので破壊した人に所有権が変わるようにする
        //CoreList[targetCoreId].TryPlace();
        CoreList[targetCoreId].SetAsNotowned();
    }
    public void ApplyPositionData(string targetCoreId, Vector3 pos) {
        CoreList[targetCoreId].SetPosition(pos);
    }
    public void ApplyPlace(string targetCoreId) {
        CoreList[targetCoreId].SetAsPlaced();
    }
    public void ApplyTransporter(string targetCoreId, Transform tr) {
        CoreList[targetCoreId].SetTransprter(tr);
    }
    
    //サーバーに処理のリクエストを送る関数。今はそのまま通しているが、今後サーバー側の処理に置き換える
    public void TryClaim(string targetCoreId) {
        //今後サーバー側の処理に置き換える
        if(CoreList[targetCoreId].nowHealth <= 0) {
            //ApplyClaimData(targetCoreId, playerId);
            ApplyOwned(targetCoreId);
        }
    }
    public void TryDamage(string targetCoreId, float amount) {
        ApplyDamageData(targetCoreId, amount);
        if(CoreList[targetCoreId].nowHealth <= 0) {
            ApplyBreakData(targetCoreId);
        }
    }
    public void TryPlace(string targetCoreId) {
        
        if(CoreList[targetCoreId].transporting) {
            ApplyPlace(targetCoreId);
        }
    }
    public void TryCollect(string targetCoreId, Transform tr) {
        if(CoreList[targetCoreId].owned) {
            ApplyTransporter(targetCoreId, tr);
        }
    }
}
