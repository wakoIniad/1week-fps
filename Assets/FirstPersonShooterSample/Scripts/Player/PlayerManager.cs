using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [System.NonSerialized] public WebSocketLoader webSocketLoader;
    [System.NonSerialized] public PlayerModelLoader playerLoader;
    public bool ControllStop = false;
    public ToggleCursor CursorSetting;
    public FPSS_PlayerCamera PlayerCamera;
    public FPSS_PlayerController PlayerController;
    public FPSS_ShooterScript ShooterScript;
    public FPSS_PlayerHealth playerHealth;
    public FPSS_PlayerCoreManager playerCore;
    public PlayerRespawn playerRespawn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        playerCore.playerManager = playerHealth.playerManager = playerRespawn.playerManager = PlayerController.playerManager = ShooterScript.playerManager = PlayerCamera.playerManager = this;
    }
    public void EnterUIMde()
    {
        ControllStop = true;
        CursorSetting.ShowCursor();
        PlayerCamera.stop = true;
        PlayerController.stop = true;
        ShooterScript.stop = true;
    }

    // Update is called once per frame
    public void ExitUIMde()
    {
        ControllStop = false;
        CursorSetting.HideCursor();
        PlayerCamera.stop = false;
        PlayerController.stop = false;
        ShooterScript.stop = false;
    }
    private bool displayedWall = false;
    void Update() {
        if(!displayedWall && webSocketLoader.EntryAccepted && PlayerController.isGround) {
            webSocketLoader.gameManager.wallActivater.DisplayWall();
            displayedWall = true;
        }
    }
}
