using UnityEngine;
using UnityEngine.UI;

public class SensibilitySlider : MonoBehaviour
{
    public GameManager gameManager;
    Slider slider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slider = GetComponent<Slider>();;
    }

    public void OnValueChange() {
        GameManager.sensibility =  slider.value;//Mathf.Pow(slider.value, 1.5f);
        //Debug.Log("SENSIBILITY:"+slider.value);
    }
}
