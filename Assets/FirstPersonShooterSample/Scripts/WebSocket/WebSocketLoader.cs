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
            //WebSocketData model = JsonConvert.DeserializeObject<WebSocketData>(e.Data);
            
            Debug.Log("WebSocket Data: " + e.Data);
            string[] splitted = e.Data.Split(',');
            string Target = splitted[0];
            string CommandType = splitted[1];
            string[] arg = splitted[2..];
            
            Debug.Log("Header-Target: " + Target);
            Debug.Log("Header-Command: " + CommandType);
            Debug.Log("arg: " + arg);
            /*Debug.Log("Header-Target: " + model.Target);
            Debug.Log("Header-Command: " + model.CommandType);
            Debug.Log("coreId: " + model.targetCoreId);
            Debug.Log("playerId: " + model.targetPlayerId);
            Debug.Log("value: " + model.value);
            Debug.Log("vec3: " + model.vec3.ToString());*/
            switch(Target) {
                case "Core":
                    string coreId = arg[0];
                    switch(CommandType) {
                        case "Create":
                            coreLoader.CreateModel(coreId, 
                            new Vector3(
                            float.Parse(arg[1]),
                            float.Parse(arg[2]),
                            float.Parse(arg[3]))
                            );
                            break;
                        case "Break":
                            //if(!coreLoader.isOwned(coreId))return;
                            coreLoader.ApplyBreakData(coreId);
                            break;
                        case "Damage":
                            //if(!coreLoader.isOwned(coreId))return;
                            coreLoader.ApplyDamageData(coreId, float.Parse(arg[1]));
                            break;
                        case "Claim":
                            //if(!coreLoader.isOwned(coreId))return;
                            coreLoader.ApplyOwned(coreId);
                            break;
                        case "Transport":
                            PlayerLocalModel playerModel = playerLoader.GetModelById(arg[1]);
                            coreLoader.ApplyTransporter(coreId, playerModel.tr);
                            break;
                        case "Place":
                                coreLoader.ApplyPlace(coreId);
                                coreLoader.ApplyPositionData(coreId, 
                                new Vector3(
                                float.Parse(arg[1]),
                                float.Parse(arg[2]),
                                float.Parse(arg[3]))
                                );
                            break;
                    }
                    break;
                case "Player":
                    string playerId = arg[0];
                        switch(CommandType) {
                            case "Create":
                                playerLoader.CreateModel(playerId, 
                                new Vector3(
                                float.Parse(arg[1]),
                                float.Parse(arg[2]),
                                float.Parse(arg[3]))
                                );
                                break;
                            case "Position":
                                if(!playerLoader.isMe(playerId)) {
                                    playerLoader.SetPosition(playerId,
                                    new Vector3(
                                    float.Parse(arg[1]),
                                    float.Parse(arg[2]),
                                    float.Parse(arg[3]))
                                    );
                                }
                                break;
                            case "Rotation":
                                if(!playerLoader.isMe(playerId)) {
                                    playerLoader.SetRotation(playerId, 
                                    new Vector3(
                                    float.Parse(arg[1]),
                                    float.Parse(arg[2]),
                                    float.Parse(arg[3]))
                                    );
                                }
                                break;
                            case "Deactivate"://On die
                                if(!playerLoader.isMe(playerId)) {
                                    playerLoader.Deactivate(playerId);
                                }
                                break;
                            case "Activate"://On Respown
                                if(!playerLoader.isMe(playerId)) {
                                    playerLoader.Activate(playerId);
                                }
                                break;
                            case "Damage":
                                //if(playerLoader.isMe(playerId)) {
                                    playerLoader.SetMyHealth(float.Parse(arg[1]));
                                //}
                                break;
                        
                    }
                    break;
                case "System":// server[IDasign -> CoreCreate -> CoreOwned]
                    switch(CommandType) {
                        case "AsignId":
                            playerLoader.SetMyId(arg[0]);
                            //頻繁に使うため保存しておく
                            myTr = playerLoader.GetMyTransform();
                            //MyPlayerId = asignedId;
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
            "Position,"+
            Vector3ToString(myTr.position));
    }
    public void SendMyRotation() {
        ws.Send(
            "Rotation,"+
            Vector3ToString(myTr.eulerAngles));
    }
    public void ActivateMe() {
        ws.Send(
            "Activate"
            );
    }
    
    public void DeactivateMe() {
        ws.Send(
            "Deactivate"
            );
    }
    
    public void RequestTransportCore(string coreId) {
        ws.Send(
            "TransportRequest,"+
            coreId
            );
    }
    
    public void RequestClaimCore(string coreId) {
        ws.Send(
            "ClaimRequest,"+
            coreId
        );
    }
    
    public void RequestPlaceCore(string coreId) {
        ws.Send(
            "PlaceRequest,"+
            coreId
        );
    }
    
    public void EntryDamageCore(string coreId, float amount) {
        ws.Send(
            "CoreDamageEntry,"+
            coreId+','+amount
        );
    }
    
    public void EntryDamagePlayer(string playerId, float amount) {
        ws.Send(
            "PlayerDamageEntry,"+
            playerId+','+amount
        );
    }
    public void Entry() {
        ws.Send("Entry");
    }
}
