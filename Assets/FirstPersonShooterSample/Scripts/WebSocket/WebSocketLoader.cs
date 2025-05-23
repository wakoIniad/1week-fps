using UnityEngine;

//using WebSocketSharp;
using NativeWebSocket;
//using WebSocketSetting;
//using Newtonsoft.Json;
//using Unity.VisualScripting;

public class WebSocketLoader : MonoBehaviour
{
    public NoticeBar noticeBarToDisplayConnectionStatus;
    public OnepointSE BGM;
    public PlayerModelLoader playerLoader;
    public CoreLoader coreLoader;
    public PlayerManager playerManager;
    public GameManager gameManager;
    public DefenceZone defenceZone;
    //public Transform PlayerPositionAnchor;

    private WebSocket ws;
    private Transform myTr;
    public bool EntryAccepted = false;
    //private string MyPlayerId;
    void Update()
    {
        #if !UNITY_WEBGL || UNITY_EDITOR
        ws.DispatchMessageQueue();
        #endif

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerManager.webSocketLoader = coreLoader.webSocketLoader = playerLoader.webSocketLoader = this;
        playerManager.playerLoader = playerLoader;
        Debug.Log("Start");
        ws = new WebSocket("wss://earwig-ruling-forcibly.ngrok-free.app");

        ws.OnOpen += () =>
        {
            Debug.Log("WebSocket Open");
            Entry();
        };
 
        ws.OnMessage += (bytes) =>
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
            string data = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log("WebSocket Data: " + data);
            string[] splitted = data.Split(',');
            string Target = splitted[0];
            string CommandType = splitted[1];
            string[] arg = {};
            if(splitted.Length > 2)arg = splitted[2..];
            
            //Debug.Log("Header-Target: " + Target);
            //Debug.Log("Header-Command: " + CommandType);
            //Debug.Log("arg: " + arg);
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
                            coreLoader.ApplyHealth(coreId, float.Parse(arg[1]));
                            break;
                        case "Claim":
                            //if(!coreLoader.isOwned(coreId))return;
                            coreLoader.ApplyOwned(coreId);
                            break;
                        case "Transport":
                            PlayerLocalModel playerModel = playerLoader.GetModelById(arg[1]);
                            coreLoader.ApplyTransporter(coreId, playerModel.transform);
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
                            case "Activate"://On Respawn
                                if(!playerLoader.isMe(playerId)) {
                                    playerLoader.Activate(playerId);
                                }
                                break;
                        
                    }
                    break;
                case "System":// server[IDasign -> CoreCreate -> CoreOwned]
                    switch(CommandType) {
                        case "AsignId":
                            playerLoader.RegisterMyModel(arg[0]);
                            break;
                        
                        //※※ playerIDを使わないため、argを0から使う。※※
                        case "SetHealth":
                            //if(playerLoader.isMe(playerId)) {
                                playerLoader.SetMyHealth(float.Parse(arg[0]));
                            //}
                            break;
                        case "SetPosition":
                            playerLoader.SetMyPosition(
                                new Vector3(
                                float.Parse(arg[0]),
                                float.Parse(arg[1]),
                                float.Parse(arg[2]))
                                );
                            EntryAccepted = true;
                            break;
                        case "Fireball":
                            playerManager.ShooterScript.ShootAt(
                                new Vector3(
                                float.Parse(arg[0]),
                                float.Parse(arg[1]),
                                float.Parse(arg[2])),
                                new Vector3(
                                float.Parse(arg[3]),
                                float.Parse(arg[4]),
                                float.Parse(arg[5]))
                            );
                            break;
                        case "Rank":
                            gameManager.score = int.Parse(arg[0]);
                            break;
                        /*case "CoreIsFull":
                            coreLoader.ApplyHealth(arg[0], CoreLocalModel.defaultHealth);
                            break;*/
                        case "Information":
                            /*(
                                string playerDefaultHealth,
                                string coreDefaultHealth,
                                string coreRepairFactorOnPlaced,
                                string coreRepairFactorOnTransporting
                            ) = arg;*/
                            PlayerLocalModel.defaultHealth = float.Parse(arg[0]);
                            CoreLocalModel.defaultHealth = float.Parse(arg[1]);
                            CoreLocalModel.repairAmountOnPlacedPerSec = float.Parse(arg[2]);
                            CoreLocalModel.repairAmountOnTransportingPerSec = float.Parse(arg[3]);
                            break;
                        case "Revival":
                            
                            break;
                        case "GameEnd":
                            playerManager.playerRespawn.GotoEndScene();
                            break;
                        case "Angel":
                            playerManager.PlayerController.AngelMode();
                            break;
                        case "ClaimingDefenceZone":
                            defenceZone.StartClaiming();
                            break;
                        case "CancelClaimingDefenceZone":
                            defenceZone.EndClaiming();
                            break;
                    }
                    break;
            }
            
                    
        };
 
        ws.OnError += (e) =>
        {
            Debug.Log("WebSocket Error Message: " + e);
            noticeBarToDisplayConnectionStatus.StartAnimation("NetworkError");
        };
 
        ws.OnClose += (e) =>
        {
            Debug.Log("WebSocket Close");
            //noticeBarToDisplayConnectionStatus.StartAnimation("Disconnected");

        };
 
        ws.Connect();
        
        //頻繁に使うため保存しておく
        myTr = playerLoader.GetMyTransform();
        //MyPlayerId = asignedId;
    }
    async void SendText(string text) {
        if (ws.State == WebSocketState.Open)
        {
            // Sending plain text
            await ws.SendText(text);
        } else {
            Debug.Log(ws.State);
            noticeBarToDisplayConnectionStatus.StartAnimation("Disconnected");
        }
    }
    string Vector3ToString(Vector3 vec3) {
        return vec3.x+","+vec3.y+","+vec3.z;
    }
    public void SendMyPosition() {
        SendText(
            "Position,"+
            Vector3ToString(myTr.position));
    }
    public void SendMyRotation() {
        SendText(
            "Rotation,"+
            Vector3ToString(myTr.eulerAngles));
    }
    
    public void RequestTransportCore(string coreId) {
        SendText(
            "TransportRequest,"+
            coreId
            );
    }
    
    public void RequestClaimCore(string coreId) {
        SendText(
            "ClaimRequest,"+
            coreId
        );
    }
    
    public void RequestPlaceCore(string coreId) {
        SendText(
            "PlaceRequest,"+
            coreId
        );
    }
    public void RequestRespawn(string targetCoreId) {
        SendText(
            "RespawnRequest,"+
            targetCoreId
        );
    }
    public void RequestWarp(string targetaCoreId) {
        SendText(
            "WarpRequest,"+
            targetaCoreId
        );
    }//||||||||>>>>>>>******;.._-^^
    //--------
    public void EntryDamageCore(string coreId, float amount) {
        SendText(
            "CoreDamageEntry,"+
            coreId+','+amount
        );
    }
    
    public void EntryDamagePlayer(string playerId, float amount) {
        SendText(
            "PlayerDamageEntry,"+
            playerId+','+amount
        );
    }
    public void TestDamage(float amount) {
        SendText(
            "SelfDamage,1,"+
            amount
        );
    }
    public void TestDamageCore(float amount) {
        SendText(
            "SelfDamageCore,1,"+playerLoader.thisPlayerModel.id+","+
            amount
        );
    }
    public void EntryShoot(Vector3 position, Vector3 direction) {
        SendText(
            "ShootEntry,"+
            Vector3ToString(position)+','+Vector3ToString(direction)
        );

    }
    public void Entry() {
        SendText("Entry");
    }
    public void EntryAngel() {
        SendText(
            "AngelEntry"
        );
    }
    public void EnterDefenceZone() {
        SendText(
            "EnterDefenceZone"
        );
    }
    public void ExitDefenceZone() {
        SendText(
            "ExitDefenceZone"
        );
    }
}
