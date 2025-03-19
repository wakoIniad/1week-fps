using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    public Transform tr;
    public string id;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tr = gameObject.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetId(string settingId) {
        id = settingId;
    }
}
