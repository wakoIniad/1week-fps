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
    [System.NonSerialized] public float RespownWaitingHeight = -100;
    private bool Scanned = false;
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

        ////指定した高さより低い場所にいる時
        //if(rh)
        //{
        //    if (transform.position.y <= rh.GetRespawnHeight())
        //    {
        //        Respawn();
        //        return;
        //    }
        //}
        //else//RespawnHeightがなかった時
        //{
        //    if (transform.position.y <= autoRespownHeight) 
        //    {
        //        Respawn();
        //        return;
        //    }
        //}

    }
    public void StartHandleRespown() {
        playerManager.playerLoader.SetMyPosition(new Vector3(0,RespownWaitingHeight,0));
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
