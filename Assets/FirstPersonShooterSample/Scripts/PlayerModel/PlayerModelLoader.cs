using UnityEngine;
using System.Collections.Generic;
public class PlayerModelLoader : MonoBehaviour
{
    [System.NonSerialized] public string ThisPlayerId;
    public FPSS_PlayerHealth MyHealthManager;
    public FPSS_PlayerController MyController;
    public GameObject ModelPrefab;
    public List<PlayerLocalModel> ModelList = new List<PlayerLocalModel>();
    
    public void SetMyId(string asignedId) {
        ThisPlayerId = asignedId;
    }
    public void SetMyHealth(float hp) {
        MyHealthManager.SetHealth(hp);
    }
    public void SetMyPosition(Vector3 position) {
        MyController.rb.position = position;
    }
    public Transform GetMyTransform() {
        return MyController.transform;
    }
    public bool isMe(string id) {
        return id == ThisPlayerId;
    }
    public void CreateModel(string id, Vector3 position) {
        GameObject generatedObject = Instantiate(ModelPrefab);
        PlayerLocalModel model = generatedObject.GetComponent<PlayerLocalModel>();
        model.SetId(id);
        //ModelList.Add(id, model);
        ModelList.Add(model);
    }
    public void Delete(string id) {

    }
    public PlayerLocalModel GetModelById(string id) {
        return ModelList.Find(model => model.id == id);
        //return ModelList[id];
    }
    
    public void SetPosition(string id, Vector3 position) {
        GetModelById(id).SetPosition(position);
    }
    public void SetRotation(string id, Vector3 rotation) {
        GetModelById(id).SetRotate(rotation);
    }
    public void Deactivate(string id) {
        GetModelById(id).Deactivate();
    }
    public void Activate(string id) {
        GetModelById(id).Activate();
    }
}
