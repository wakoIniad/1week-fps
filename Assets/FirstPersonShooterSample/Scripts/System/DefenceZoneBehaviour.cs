using UnityEngine;

public class DefenceZone : MonoBehaviour
{
    public TimerProgressRing timerProgressRing;
    public WebSocketLoader webSocketLoader;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
    
    }
    public void StartClaiming() {
        timerProgressRing.waitTime = 32;
        timerProgressRing.StartTimer();
    }
    public void EndClaiming() {
        timerProgressRing.CancelTimer();
    }
    void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Player")) {
            webSocketLoader.EnterDefenceZone();
        }
    }
    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Player")) {
            webSocketLoader.ExitDefenceZone();
        }
    }
}
