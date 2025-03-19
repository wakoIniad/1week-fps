using UnityEngine;
using System.Collections.Generic;
public class PlayerModelLoader : MonoBehaviour
{
    public string ThisPlayerId;
    public GameObject ModelPrefab;
    public Dictionary<string, PlayerLocalModel> ModelList = new Dictionary<string, PlayerLocalModel>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CreateModel(string id, Vector3 position) {
        GameObject generatedObject = Instantiate(ModelPrefab);
        PlayerLocalModel model = generatedObject.GetComponent<PlayerLocalModel>();
        model.SetId(id);
        ModelList.Add(id, model);
    }
    public void Delete(string id) {

    }
    public PlayerLocalModel GetModelById(string id) {
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
