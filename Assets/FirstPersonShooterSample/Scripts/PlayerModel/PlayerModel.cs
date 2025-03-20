using UnityEngine;

public class PlayerLocalModel : MonoBehaviour
{
    [System.NonSerialized] public PlayerModelLoader loader;
    public string id;
    public void SetId(string settingId) {
        id = settingId;
    }
    public void SetPosition(Vector3 position) {
        gameObject.transform.position = position;
    }
    public void SetRotate(Vector3 rotate) {
        gameObject.transform.eulerAngles = rotate;
    }
    public void Deactivate() {
        gameObject.SetActive(false);
    }
    public void Activate() {
        gameObject.SetActive(true);
    }
    public void TryDamage(float amount) {
        loader.TryDamage(id, amount);
    }
}
