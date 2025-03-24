using UnityEngine;

public class GameSystemMenu : MonoBehaviour
{
    public GameObject panelObject;
    public GameObject reticle;
    public PlayerManager playerManager;
    private bool displaying = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        panelObject.SetActive(false);
        displaying = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            displaying = !displaying;
            panelObject.SetActive(displaying);
            if(displaying) {
                playerManager.EnterUIMde();
                reticle.SetActive(false);
            } else {
                playerManager.ExitUIMde();
                reticle.SetActive(true);
            }
        }
    }
}
