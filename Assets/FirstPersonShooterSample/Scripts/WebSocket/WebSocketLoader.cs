using UnityEngine;

using WebSocketSharp;
using WebSocketSetting;
using Newtonsoft.Json;
using Unity.VisualScripting;

public class WebSocketLoader : MonoBehaviour
{
    public PlayerModelLoader playerLoader;
    public CoreLoader coreLoader;
    private WebSocket ws;
    private float connectionKey = UnityEngine.Random.Range( 0.0f, 1.0f );
    private Transform myTr;
    //private string MyPlayerId;
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
            switch(model.Target) {
                case "Core":
                    if(model.targetCoreId is string coreId) {
                        switch(model.CommandType) {
                            case "Created":
                                if(model.vec3 is Vector3 position) {
                                    coreLoader.CreateModel(coreId, position);
                                }
                                break;
                            case "Breaked":
                                if(!coreLoader.isOwned(coreId))return;

                                coreLoader.ApplyBreakData(coreId);
                                break;
                            case "Damageed":
                                if(!coreLoader.isOwned(coreId))return;

                                if(model.value is float hp) {
                                    coreLoader.ApplyDamageData(coreId, hp);
                                }
                                break;
                            case "Owned":
                                if(!coreLoader.isOwned(coreId))return;
                                coreLoader.ApplyOwned(coreId);
                                break;
                            case "Transporting":
                                if(model.targetPlayerId is string playerId) {
                                    PlayerLocalModel playerModel = playerLoader.GetModelById(playerId);
                                    coreLoader.ApplyTransporter(coreId, playerModel.tr);
                                }
                                break;
                            case "Placed":
                                if(model.vec3 is Vector3 placedPsition) {
                                    coreLoader.ApplyPlace(coreId);
                                    coreLoader.ApplyPositionData(coreId, placedPsition);
                                }
                                break;
                        }
                    }
                    break;
                case "Player":
                    if(model.targetPlayerId is string id) {
                        switch(model.CommandType) {
                            case "Created":
                                if(model.vec3 is Vector3 createdPosition)
                                playerLoader.CreateModel(id, createdPosition);
                                break;
                            case "SetPosition":
                                if(!playerLoader.isMe(id)) {
                                    if(model.vec3 is Vector3 position)
                                    playerLoader.SetPosition(id, position);
                                }
                                break;
                            case "SetRotation":
                                if(!playerLoader.isMe(id)) {
                                    if(model.vec3 is Vector3 rotation)
                                    playerLoader.SetRotation(id, rotation);
                                }
                                break;
                            case "Deactivate"://On die
                                if(!playerLoader.isMe(id)) {
                                    playerLoader.Deactivate(id);
                                }
                                break;
                            case "Activate"://On Respown
                                if(!playerLoader.isMe(id)) {
                                    playerLoader.Activate(id);
                                }
                                break;
                            case "Damaged":
                                if(playerLoader.isMe(id)) {
                                    if(model.value is float hp)
                                    playerLoader.SetMyHealth(hp);
                                }
                                break;
                        
                    }
                    }
                    break;
                case "System":// server[IDasign -> CoreCreate -> CoreOwned]
                    switch(model.CommandType) {
                        case "IdAsigned":
                            if(model.value is float key) {
                                if(key == connectionKey) {
                                    if(model.targetPlayerId is string asignedId) {
                                        playerLoader.SetMyId(asignedId);
                                        //頻繁に使うため保存しておく
                                        myTr = playerLoader.GetMyTransform();
                                        //MyPlayerId = asignedId;
                                    }
                                }
                            }
                            break;
                    }
                    break;
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
    string Vector3ToString(Vector3 vec3) {
        return vec3.x+","+vec3.y+","+vec3.z;
    }
    public void SendMyPosition() {
        ws.Send(
            "rotation,"+
            playerLoader.ThisPlayerId+
            ","+
            Vector3ToString(myTr.position));
    }
    public void SendMyRotation() {
        ws.Send(
            "rotation,"+
            playerLoader.ThisPlayerId+
            ","+
            Vector3ToString(myTr.eulerAngles));
    }
    public void ActivateMe() {
        ws.Send(
            "activate,"+
            playerLoader.ThisPlayerId);
    }
    
    public void DeactivateMe() {
        ws.Send(
            "deactivate,"+
            playerLoader.ThisPlayerId);
    }
    
    public void RequestTransportCore(string coreId) {
        ws.Send(
            "req-transport,"+
            playerLoader.ThisPlayerId+","+
            coreId
            );
    }
    public void GetMyId() {
        ws.Send("id_asign,"+connectionKey);
    }
}
