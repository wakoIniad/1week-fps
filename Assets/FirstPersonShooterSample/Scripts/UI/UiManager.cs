using UnityEngine;

public class UiManager : MonoBehaviour
{
    
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
        if(Input.GetKeyDown(KeyCode.M)) {
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
            playerManager.EnterUIMde();
            battleUIContainer.SetActive(false);
        } else {
            playerManager.ExitUIMde();
            battleUIContainer.SetActive(true);
        }
    }
    
    public void OnWarpAnchorCoreSelected(string id) {
        playerManager.playerCore.coreLoader.TryWarp(id);
        coreMapUI.DeactivateUI();
        playerManager.ExitUIMde();
    }
}
