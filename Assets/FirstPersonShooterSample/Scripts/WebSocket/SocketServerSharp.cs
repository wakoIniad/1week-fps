using UnityEngine;
using System.Collections;
using WebSocketSharp;
using WebSocketSharp.Server;

public class SocketServerSharp : MonoBehaviour {

   private WebSocketServer server;

   void Start ()
   {
       server = new WebSocketServer(3000);

       server.AddWebSocketService<Echo>("/");
       server.Start();

   }

   void OnDestroy()
   {
       server.Stop();
       server = null;
   }

}

public class Echo : WebSocketBehavior
{
   protected override void OnMessage (MessageEventArgs e)
   {
       Debug.Log(e.Data);
       Sessions.Broadcast(e.Data);
   }
}
