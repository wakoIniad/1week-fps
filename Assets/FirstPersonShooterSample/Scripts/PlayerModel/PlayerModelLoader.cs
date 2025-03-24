using UnityEngine;
using System.Collections.Generic;
public class PlayerModelLoader : MonoBehaviour
{
    [System.NonSerialized] public WebSocketLoader webSocketLoader;
    public PlayerLocalModel thisPlayerModel;
    public FPSS_PlayerHealth MyHealthManager;
    public FPSS_PlayerController MyController;
    public GameObject ModelPrefab;
    public Dictionary<string,PlayerLocalModel> ModelList = new Dictionary<string,PlayerLocalModel>();
    public void RegisterMyModel(string id) {
        thisPlayerModel.id = id;
        thisPlayerModel.loader = this;
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
        model.loader = this;
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
        PlayerLocalModel targetModel;
        try {
            targetModel = GetModelById(id);
        } catch {
            CreateModel(id, position);
            targetModel = GetModelById(id);
        }
        targetModel.SetPosition(position);
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
    public void TryDamage(string id, float amount) {
        webSocketLoader.EntryDamagePlayer(id, amount);
    }

    
//テスト
    void Update() {
        
        if(Input.GetKeyDown(KeyCode.T)) {
//            thisPlayerModel.TryDamage(2);
            webSocketLoader.TestDamage(2);
        }
        if(Input.GetKeyDown(KeyCode.E)) {
            webSocketLoader.TestDamageCore(8);
        }
    }
}
