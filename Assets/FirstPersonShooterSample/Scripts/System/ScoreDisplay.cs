using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    GameManager gameManager;
    TextMeshProUGUI displayTarget;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        displayTarget.text = "#"+gameManager.score.ToString();
    }

}
