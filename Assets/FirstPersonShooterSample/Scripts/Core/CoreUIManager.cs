using TMPro;
using UnityEngine;

public class CoreUIManager : MonoBehaviour
{
    
    public TextMeshProUGUI healthText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Remove() {
        Destroy(gameObject);
    }
    public void DisplayCoreHealth(float hp) {
        healthText.text = hp.ToString();
    }
}
