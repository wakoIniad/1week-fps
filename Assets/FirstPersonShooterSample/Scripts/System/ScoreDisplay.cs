using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    public GameManager gameManager;
    public TextMeshProUGUI displayTarget;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        displayTarget.text = "#"+gameManager.score.ToString();
    }

}
