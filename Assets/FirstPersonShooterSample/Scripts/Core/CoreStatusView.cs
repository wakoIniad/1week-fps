using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoreStatusView : MonoBehaviour
{
    public Image image;
    public TextMeshProUGUI healthText;
    public Button button;
    private int coreId;

    public void Remove() {
        Destroy(gameObject);
    }
    public void DisplayCoreHealth(float hp) {
        healthText.text = hp.ToString();
    }
    public void DisplayTransporting() {
        image.color = Color.blue;
    }
    public void DisplayPlacing() {
        image.color = Color.white;
    }
    public void SetId(int id) {
        coreId = id;
    }
    /*void Start() {
    }
    void Clicked() {
        
    }*/
}
