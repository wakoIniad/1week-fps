using UnityEngine;

public class PlayerLocalModel : MonoBehaviour
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
    public void SetPosition(Vector3 position) {
        tr.position = position;
    }
    public void SetRotate(Vector3 rotate) {
        tr.eulerAngles = rotate;
    }
    public void Deactivate() {
        gameObject.SetActive(false);
    }
    public void Activate() {
        gameObject.SetActive(true);
    }
}
