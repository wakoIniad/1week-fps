using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public GameObject warpEffectPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void WarpEffect(Transform transform) {
        Instantiate(warpEffectPrefab, transform);
    }
}
