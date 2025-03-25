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
    private float lastValue = -1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void GetData()
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
            minusEdgeMaskObject.SetActive(false); 
        }
    }
    void Start() {
        GetData();
    }

    public void UpdateBar(float remainingPercentage) {
        if(!mask)GetData();
        float scale = maskObject.transform.parent.localScale.y;
        mask.padding = scale * MaskMap * 
        rectTransform.sizeDelta.y * (1-remainingPercentage);
        if(edgeMaskObject != null && lastValue != -1) {
            float delta = remainingPercentage - lastValue;
            RectMask2D edgeTarget = edgeMask;
            if(minusEdgeMask) {
                if(delta < 0) {
                    minusEdgeMaskObject.SetActive(true);
                    edgeTarget = minusEdgeMask;
                    delta *= -1;
                } else {
                    minusEdgeMaskObject.SetActive(false); 
                }
            }
            
            //Debug.Log("M-Lossy:"+maskObject.transform.parent.lossyScale.y);
            //Debug.Log("M-LossyParent:"+maskObject.transform.parent.lossyScale.y);
            //Debug.Log("M-local:"+maskObject.transform.localScale.y);
            //Debug.Log("M-localParent:"+maskObject.transform.parent.localScale.y);
            edgeTarget.padding = 
            edgeMaskMap *
            (rectTransform.sizeDelta.y-8) * (remainingPercentage-delta) * scale
            +
            MaskMap * 
            (rectTransform.sizeDelta.y-8) * (1-remainingPercentage-delta) * scale;
        }
        lastValue = remainingPercentage;
    }
}
