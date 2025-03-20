using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerRespown : MonoBehaviour
{
    public string gameoverSceneName = "";
    FPSS_PlayerCoreManager coreManager;
    private bool waitingRespawn = false;
    private Rigidbody rb;
    private FPSS_PlayerHealth healthManager;
    public GameManager gameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        coreManager = gameObject.GetComponent<FPSS_PlayerCoreManager>();
        healthManager = gameObject.GetComponent<FPSS_PlayerHealth>();
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(waitingRespawn) {
            if(coreManager.CoreCount() == 0) {
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
        gameManager.EnterUIMde();
        waitingRespawn = true;
        coreManager.OnCoreStatusViewClicked += SpawnAnchorSelected;
    }
    public void SpawnAnchorSelected(string id) {
        Debug.Log("test:"+id);
        if(waitingRespawn) {
            CoreLocalModel core = coreManager.coreLoader.GetModelById(id);
            core.TryRespawn();
            //healthManager.nowH
        }
        EndHandleRespown();
    }
    public void EndHandleRespown() {
        gameManager.ExitUIMde();
        waitingRespawn = false;
        coreManager.OnCoreStatusViewClicked -= SpawnAnchorSelected;

    }

}
