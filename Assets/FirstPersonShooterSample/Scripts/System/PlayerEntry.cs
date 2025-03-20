using UnityEngine;

public class PlayerEntry : MonoBehaviour
{
    public WebSocketLoader webSocketLoader;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        webSocketLoader.Entry();
    }

}
