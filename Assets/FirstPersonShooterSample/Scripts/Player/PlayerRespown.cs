using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerRespawn : MonoBehaviour
{
    public string gameoverSceneName = "";
    private bool waitingRespawn = false;
    [System.NonSerialized] public PlayerManager playerManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(waitingRespawn) {
            if(playerManager.playerCore.CoreCount() == 0) {
                EndHandleRespown();
                if(string.IsNullOrEmpty(gameoverSceneName)) {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                } else {  
                    SceneManager.LoadScene(gameoverSceneName);
                }
            }
        }

    }
    public void StartHandleRespown() {
        playerManager.EnterUIMde();
        waitingRespawn = true;
        playerManager.playerCore.OnCoreStatusViewClicked += SpawnAnchorSelected;
    }
    public void SpawnAnchorSelected(string id) {
        Debug.Log("test:"+id);
        if(waitingRespawn) {
            CoreLocalModel core = playerManager.playerCore.coreLoader.GetModelById(id);
            core.TryRespawn();
            //healthManager.nowH
        }
        EndHandleRespown();
    }
    public void EndHandleRespown() {
        playerManager.ExitUIMde();
        waitingRespawn = false;
        playerManager.playerCore.OnCoreStatusViewClicked -= SpawnAnchorSelected;

    }

}
