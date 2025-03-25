using UnityEngine;

public class SystemMenuUI : MonoBehaviour
{
    public GameObject containerObject;
    public GameObject reticle;
    public PlayerManager playerManager;
    public bool active = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        containerObject.SetActive(false);
        active = false;
    }
    public void ActivateUI() {
        active = true;
        playerManager.EnterUIMde();
        reticle.SetActive(false);
    }
    public void DeactivateUI() {
        active = false;
        playerManager.ExitUIMde();
        reticle.SetActive(true);
        
    }
    public void toggleVisibility() {
        active = !active;
        containerObject.SetActive(active);
        if(active) {
            playerManager.EnterUIMde();
            reticle.SetActive(false);
        } else {
            playerManager.ExitUIMde();
            reticle.SetActive(true);
        }
    }
}
