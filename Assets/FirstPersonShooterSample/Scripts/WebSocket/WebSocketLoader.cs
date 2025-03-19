using UnityEngine;

using WebSocketSharp;
using WebSocketSetting;
using Newtonsoft.Json;

public class WebSocketLoader : MonoBehaviour
{
    public PlayerModelLoader playerLoader;
    public CoreLoader coreLoader;
    private WebSocket ws;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
            WebSocketData model = JsonConvert.DeserializeObject<WebSocketData>(e.Data);
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
                        if(model.targetCoreId is string id) {
                            switch(model.CommandType) {
                                case "Breaked":
                                    if(!coreLoader.isOwned(id))return;
                                    coreLoader.ApplyBreakData(id);
                                    break;
                                case "Damageed":
                                    if(!coreLoader.isOwned(id))return;
                                    if(model.value is float hp)coreLoader.ApplyDamageData(id, hp);
                                    break;
                                case "Owned":
                                    if(!coreLoader.isOwned(id))return;
                                    coreLoader.ApplyOwned(id);
                                    break;
                                case "Transporting":
                                    if(model.targetPlayerId is string playerId) {
                                        playerLoader.GetPlayerModelById(playerId);
                                    }
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
