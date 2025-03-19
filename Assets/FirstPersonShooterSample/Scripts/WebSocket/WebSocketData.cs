using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using UnityEngine;


namespace WebSocketSetting {
    
    public class WebSocketData
    {
        //[JsonPropertyName("Header")]
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

        //header (２つ合わせて)
        public string Target { get; set; }
        public string CommandType { get; set; }

        //data
        public string? targetCoreId { get; set; }
        public string? targetPlayerId { get; set; }
        public Vector3? vec3 { get; set; }
        public float? value { get; set; }
    }
}