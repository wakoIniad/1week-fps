using UnityEngine;
using UnityEngine.UI;

public class ImageBar : MonoBehaviour
{
    public float maskDirectionRotation = 0;
    public GameObject maskObject;
    private RectMask2D mask;
    private RectTransform rectTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mask = maskObject.GetComponent<RectMask2D>();   
        rectTransform = maskObject.GetComponent<RectTransform>();
        rectTransform.rotation = Quaternion.Euler(0.0f, 0.0f, maskDirectionRotation);
    }

    public void UpdateBar(float remainingPercentage) {
        mask.padding = new Vector4(0,0,0,rectTransform.sizeDelta.y * (1-remainingPercentage));
    }
}
