using TMPro;
using UnityEngine;

public class CoreStatusView : MonoBehaviour
{
    
    public TextMeshProUGUI healthText;

    public void Remove() {
        Destroy(gameObject);
    }
    public void DisplayCoreHealth(float hp) {
        healthText.text = hp.ToString();
    }
}
