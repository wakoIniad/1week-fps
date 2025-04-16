using UnityEngine;
using UnityEngine.UI;

public class SensibilitySlider : MonoBehaviour
{
    Slider slider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slider = GetComponent<Slider>();;
    }

    public void OnValueChange() {
        GameManager.sensibility = slider.value;
    }
}
