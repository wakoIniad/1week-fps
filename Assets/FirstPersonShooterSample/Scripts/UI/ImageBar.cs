using UnityEngine;
using UnityEngine.UI;

public class ImageBar : MonoBehaviour
{
    public Vector4 MaskMap = new Vector4(0,0,0,1);
    private Vector4 edgeMaskMap;
    public GameObject maskObject;
    public GameObject edgeMaskObject;
    public GameObject minusEdgeMaskObject;
    private RectMask2D mask;
    private RectTransform rectTransform;
    private RectMask2D edgeMask;
    private RectMask2D minusEdgeMask;
    private float lastValue = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mask = maskObject.GetComponent<RectMask2D>();   
        rectTransform = maskObject.GetComponent<RectTransform>();
        //rectTransform.rotation = Quaternion.Euler(0.0f, 0.0f, maskDirectionRotation);
        if(edgeMaskObject) {
            edgeMask = edgeMaskObject.GetComponent<RectMask2D>();   
            edgeMaskMap = new Vector4(MaskMap.z, MaskMap.w, MaskMap.x, MaskMap.y);
        }
        if(minusEdgeMaskObject) {
            minusEdgeMask = edgeMaskObject.GetComponent<RectMask2D>();   
        }
    }

    public void UpdateBar(float remainingPercentage) {
        mask.padding = MaskMap * 
        rectTransform.sizeDelta.y * (1-remainingPercentage);
        if(edgeMaskObject != null && lastValue != -1) {
            float delta = remainingPercentage - lastValue;
            RectMask2D edgeTarget = edgeMask;
            if(delta < 0 && minusEdgeMask) {
                edgeTarget = minusEdgeMask;
                delta *= -1;
            }
            edgeTarget.padding = 
            edgeMaskMap *
            rectTransform.sizeDelta.y * (remainingPercentage-delta)
            +
            MaskMap * 
            rectTransform.sizeDelta.y * (1-remainingPercentage-delta);
        }
        lastValue = remainingPercentage;
    }
}
