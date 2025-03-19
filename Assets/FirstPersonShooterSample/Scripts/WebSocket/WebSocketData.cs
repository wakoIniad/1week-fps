using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using UnityEngine;


namespace WebSocketData {
    public class WebSocketData
    {
        //[JsonPropertyName("Header")]
        //自分以外のプレイヤーのHPは反映する必要がないのでしない。
        //targetIDが必要なのは１プレイやーで複数持ち得るコアだけ（今のところ）
        /**
        */
        public string Header { get; set; }
        public int? targetCore { get; set; }
        public Vector3? position { get; set; }
        public float? value { get; set; }
    }
}