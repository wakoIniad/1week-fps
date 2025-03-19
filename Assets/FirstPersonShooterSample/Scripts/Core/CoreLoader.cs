using UnityEngine;
using System;
using System.Collections.Generic;
using WebSocketSharp;
using WebSocketSetting;
using System.Text;
using Newtonsoft.Json;
using Unity.VisualScripting;

public class CoreLoader : MonoBehaviour
{
    public string playerId = "temp";
    public event Action<int, float> OnOwnedCoreDamaged;
    public event Action<int> OnOwnedCoreBreaked;
    public event Action<int> OnCoreOwned;
    public Transform loaderTransform;
    
    public List<CoreObjectData> CoreList = new List<CoreObjectData>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private WebSocket ws;
    void Start()
   {
        loaderTransform = gameObject.GetComponent<Transform>();
        ws = new WebSocket("ws://localhost:3000/");
 
        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("WebSocket Open");
        };
 
        ws.OnMessage += (sender, e) =>
        {
            /**Headers**
        
        otherPlayerPsitionChange: targetaPlayerId, vec3(position)
        otherPlayerObjectCreate: targetaPlayerId(settingId), vec3(position)
        otherPlayerObjectDelete: targetaPlayerId
        otherPlayerRotateChange: targetaPlayerId, vec3(rotate)
        updateMyhealth(mainly damaged): value

        coreTransporting; targetaPlayerId, targetCoreId
        corePlaced: targetCoreId, vec3(position)
        coreBreaked(for owners): targetCoreId
        coreDamaged(for owners): targetCoreId, value(hp)
        coreOwned(for owners): targetCoreId
        coreObjectCreate: targetCoreId(settingId), vec3(position)
        */
            WebSocketSetting.WebSocketData model = JsonConvert.DeserializeObject<WebSocketSetting.WebSocketData>(e.Data);
            Debug.Log("WebSocket Data: " + e.Data);
            
            Debug.Log("Header-Target: " + model.Target);
            Debug.Log("Header-Command: " + model.CommandType);
            Debug.Log("coreId: " + model.targetCoreId);
            Debug.Log("playerId: " + model.targetPlayerId);
            Debug.Log("value: " + model.value);
            Debug.Log("vec3: " + model.vec3.ToString());
            if(model.CommandType == "Create") {

            } else {
                switch(model.Target) {
                    case "Core":
                        if(model.targetCoreId is int id) {
                            switch(model.CommandType) {
                                case "Breaked":
                                    if(!isOwned(id))return;
                                    ApplyBreakData(id);
                                    break;
                                case "Damageed":
                                    if(!isOwned(id))return;
                                    if(model.value is float hp)ApplyDamageData(id, hp);
                                    break;
                                case "Owned":
                                    if(!isOwned(id))return;
                                    ApplyOwned(id);
                                    break;
                                
                            }
                        }
                        break;
                }
            }
                    
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
    bool isOwned(int? id) {
        if(id is int t) {
            return CoreList[t].owned;
        } else {
            return false;
        }
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
    /*public void ApplyClaimData(int targetCoreId, string ownerId) {
        if(ownerId == playerId) {
            CoreList[targetCoreId].SetAsOwned();
            if(OnCoreOwned != null)OnCoreOwned.Invoke(targetCoreId);
        }
    }*/
    public void ApplyOwned(int targetCoreId) {
            CoreList[targetCoreId].SetAsOwned();
            if(OnCoreOwned != null)OnCoreOwned.Invoke(targetCoreId);
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
        //プライヤーが移動中の場合落ちる  -> すぐ拾えちゃうので破壊した人に所有権が変わるようにする
        //CoreList[targetCoreId].TryPlace();
        CoreList[targetCoreId].SetAsNotowned();
    }
    public void ApplyPositionData(int targetCoreId, Vector3 pos) {
        CoreList[targetCoreId].SetPosition(pos);
    }
    public void ApplyPlace(int targetCoreId) {
        CoreList[targetCoreId].SetAsPlaced();
    }
    public void ApplyTransporter(int targetCoreId, Transform tr) {
        CoreList[targetCoreId].SetTransprter(tr);
    }
    
    //サーバーに処理のリクエストを送る関数。今はそのまま通しているが、今後サーバー側の処理に置き換える
    public void TryClaim(int targetCoreId) {
        //今後サーバー側の処理に置き換える
        if(CoreList[targetCoreId].nowHealth <= 0) {
            //ApplyClaimData(targetCoreId, playerId);
            ApplyOwned(targetCoreId);
        }
    }
    public void TryDamage(int targetCoreId, float amount) {
        ApplyDamageData(targetCoreId, amount);
        if(CoreList[targetCoreId].nowHealth <= 0) {
            ApplyBreakData(targetCoreId);
        }
    }
    public void TryPlace(int targetCoreId) {
        
        if(CoreList[targetCoreId].transporting) {
            ApplyPlace(targetCoreId);
        }
    }
    public void TryCollect(int targetCoreId, Transform tr) {
        if(CoreList[targetCoreId].owned) {
            ApplyTransporter(targetCoreId, tr);
        }
    }
}
