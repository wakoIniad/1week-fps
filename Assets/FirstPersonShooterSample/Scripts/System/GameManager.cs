using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int score;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SceneManager.activeSceneChanged += ActiveSceenChanged;
    }

    // Update is called once per frame
    void Update()
    {
        
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
