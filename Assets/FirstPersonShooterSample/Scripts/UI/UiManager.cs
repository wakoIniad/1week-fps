using UnityEngine;

public class UiManager : MonoBehaviour
{
    public TouchPad PadM;
    public TouchPad PadClose;
    public SystemMenuUI systemMenuUI;
    public CoreMapUI coreMapUI;
    public GameObject battleUIContainer;
    public PlayerManager playerManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            if(coreMapUI.active) {
                coreMapUI.DeactivateUI();
            } else {
                systemMenuUI.toggleVisibility();
            }
            UpdateUIMode();
        }
        if(Input.GetKeyDown(KeyCode.M) || PadM.CheckTouched() || PadClose.CheckTouched()) {
            if(!systemMenuUI.active) {
                coreMapUI.toggleVisibility();
                if(systemMenuUI.active)systemMenuUI.DeactivateUI();
                if(coreMapUI.active) {
                    coreMapUI.OnRespawnAnchorSelected += OnWarpAnchorCoreSelected;
                } else {
                    coreMapUI.OnRespawnAnchorSelected -= OnWarpAnchorCoreSelected;
                }
                UpdateUIMode();
            }
        }
    }
    void UpdateUIMode() {
        if(coreMapUI.active || systemMenuUI.active) {
            StartUIMode();
        } else {
            EndUIMode();
        }
    }
    void EndUIMode() {
        playerManager.ExitUIMde();
        battleUIContainer.SetActive(true);
    }
    void StartUIMode() {
        playerManager.EnterUIMde();
        battleUIContainer.SetActive(false);
    }
    
    public void OnWarpAnchorCoreSelected(string id) {
        playerManager.playerCore.coreLoader.TryWarp(id);
        coreMapUI.DeactivateUI();
        EndUIMode();
    }
}
