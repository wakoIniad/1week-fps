using UnityEngine;
using System.Collections.Generic;
public class PlayerModelLoader : MonoBehaviour
{
    public string ThisPlayerId;
    public GameObject ModelPrefab;
    public List<PlayerModel> ModelList = new List<PlayerModel>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CreateModel(string id) {
        GameObject generatedObject = Instantiate(ModelPrefab);
        PlayerModel model = generatedObject.GetComponent<PlayerModel>();
        model.SetId(id);
        ModelList.Add(model);
    }
    public void Delete(string id) {

    }
    public PlayerModel GetPlayerModelById(string id) {
        return ModelList.Find(model => model.id == id);
    }
}
