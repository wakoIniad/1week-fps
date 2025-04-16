using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static float BGM_Volum = 1;
    public static float MouseSpeed = 1;
    public static string UserName = "Unnamed";
    public WallActivater wallActivater;
    public int score;
    public bool touchModeSetting = true;
    public static bool touchMode = true;
    public static float sensibility = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake () {
        
        if(SystemInfo.deviceType == DeviceType.Handheld) {
            touchMode = true;
        } else if(SystemInfo.deviceType == DeviceType.Desktop) {
            touchMode = false;
        }
        if(touchModeSetting)touchMode = touchModeSetting;
    }

    void Start()
    {
        SceneManager.activeSceneChanged += ActiveSceenChanged;
    }

    // Update is called once per frame
    void Update()
    {
       if(Input.GetKeyDown(KeyCode.I)) {
            touchMode = !touchMode;
       }  
    }
    void Handover(GameManager nextGameManager) {
        nextGameManager.score = score;
    }
    void ActiveSceenChanged(Scene currentScene, Scene nextScene) {
        GameObject gameManagerObject = GameObject.FindWithTag("GameManager");
        if(gameManagerObject) {
            GameManager gameManager = gameManagerObject.GetComponent<GameManager>();
            Handover(gameManager);
        }
    }
}
