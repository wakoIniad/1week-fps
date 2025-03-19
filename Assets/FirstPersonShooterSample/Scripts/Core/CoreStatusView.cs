using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoreStatusView : MonoBehaviour
{
    
    public TextMeshProUGUI healthText;
    public Button button;
    private int coreId;

    public void Remove() {
        Destroy(gameObject);
    }
    public void DisplayCoreHealth(float hp) {
        healthText.text = hp.ToString();
    }
    public void SetId(int id) {
        coreId = id;
    }
    /*void Start() {
    }
    void Clicked() {
        
    }*/
}
