using UnityEngine;
using System;
using System.Collections.Generic;
using WebSocketSharp;

public class CoreLoader : MonoBehaviour
{
    public string playerId = "temp";
    public event Action<int, float> OnOwnedCoreDamaged;
    public event Action<int> OnOwnedCoreBreaked;
    public event Action<int> OnCoreOwned;
    
    public List<CoreObjectData> CoreList = new List<CoreObjectData>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private WebSocket ws;
    void Start()
   {
       ws = new WebSocket("ws://localhost:3000/");

       ws.OnOpen += (sender, e) =>
       {
           Debug.Log("WebSocket Open");
       };

       ws.OnMessage += (sender, e) =>
       {
           Debug.Log("WebSocket Data: " + e.Data);
       };

       ws.OnError += (sender, e) =>
       {
           Debug.Log("WebSocket Error Message: " + e.Message);
       };

       ws.OnClose += (sender, e) =>
       {
           Debug.Log("WebSocket Close");
       };

       ws.Connect();

   }

    // Update is called once per frame
    void Update()
    {
        
    }
    public CoreObjectData GetCoreById(int id) {
        return CoreList[id];
    }
    public CoreObjectData[] GetOwnedCores() {
        List<CoreObjectData> result = new List<CoreObjectData>();
        for(int i = 0;i < CoreList.Count; i++) {
            if(CoreList[i].owned)result.Add(CoreList[i]);
        }
        return result.ToArray();
    }
    //apply server data to local data
    public void ApplyClaimData(int targetCoreId, string ownerId) {
        if(ownerId == playerId) {
            CoreList[targetCoreId].SetAsOwned();
            if(OnCoreOwned != null)OnCoreOwned.Invoke(targetCoreId);
        }
    }
    public void ApplyDamageData(int targetCoreId, float damage) {
        CoreList[targetCoreId].Damage(damage);
        if(CoreList[targetCoreId].owned) {
            if(OnOwnedCoreDamaged != null)OnOwnedCoreDamaged.Invoke(targetCoreId, CoreList[targetCoreId].nowHealth);
        }
    }
    public void ApplyBreakData(int targetCoreId) {
        if(CoreList[targetCoreId].owned) {
            if(OnOwnedCoreBreaked != null)OnOwnedCoreBreaked.Invoke(targetCoreId);
        }
    }
    
    //サーバーに処理のリクエストを送る関数。今はそのまま通しているが、今後サーバー側の処理に置き換える
    public void TryClaim(int targetCoreId) {
        //今後サーバー側の処理に置き換える
        if(CoreList[targetCoreId].nowHealth <= 0) {
            ApplyClaimData(targetCoreId, playerId);
        }
    }
    public void TryDamage(int targetCoreId, float amount) {
        ApplyDamageData(targetCoreId, amount);
        if(CoreList[targetCoreId].nowHealth < 0) {
            ApplyBreakData(targetCoreId);
        }
    }
}
