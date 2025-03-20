using UnityEngine;
using System.Collections.Generic;
public class PlayerModelLoader : MonoBehaviour
{
    public PlayerLocalModel thisPlayerModel;
    public FPSS_PlayerHealth MyHealthManager;
    public FPSS_PlayerController MyController;
    public GameObject ModelPrefab;
    public Dictionary<string,PlayerLocalModel> ModelList = new Dictionary<string,PlayerLocalModel>();
    public void RegisterMyModel(string id) {
        thisPlayerModel.id = id;
        ModelList.Add(id, thisPlayerModel);
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
        return id == thisPlayerModel.id;
    }
    public void CreateModel(string id, Vector3 position) {
        GameObject generatedObject = Instantiate(ModelPrefab);
        PlayerLocalModel model = generatedObject.GetComponent<PlayerLocalModel>();
        model.SetId(id);
        model.SetPosition(position);
        ModelList.Add(id, model);
        //ModelList.Add(model);
    }
    public void Delete(string id) {

    }
    public PlayerLocalModel GetModelById(string id) {
        //Debug.Log("id:"+id+"::"+ModelList.Find(model => model.id == id));
        //return ModelList.Find(model => model.id == id);
        return ModelList[id];
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
