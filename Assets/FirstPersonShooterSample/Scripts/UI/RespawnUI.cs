using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class RespawnUI : MonoBehaviour
{
    public GameObject battleUIObject;
    public GameObject systemUIObject;
    
    public GameObject respawnUIObject;
    public GameObject mapObject;
    public CoreLoader coreLoader;
    public GameObject respawnPointIconPrefab;
    RectTransform mapRectTransform;
    public float MAP_SIZE = 64;
    public event Action<string> OnRespawnAnchorSelected; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mapRectTransform = mapObject.GetComponent<RectTransform>();
    }
    public void ActivateUI() {
        battleUIObject.SetActive(false);
        systemUIObject.SetActive(false);
        CoreLocalModel[] cores = new CoreLocalModel[coreLoader.CoreList.Count];
        coreLoader.CoreList.Values.CopyTo(cores,0);
        float mapDisplayWidth = mapRectTransform.sizeDelta.x;
        float scale = mapDisplayWidth/MAP_SIZE;
        Dictionary<string, GameObject> icons = new Dictionary<string, GameObject>();
        for(int i = 0;i < cores.Length; i++) {
            CoreLocalModel core = cores[i];
            GameObject icon = Instantiate(respawnPointIconPrefab, mapObject.transform);
            icons[core.id] = icon;
            Button button = icon.GetComponent<Button>();
            Vector3 corePosition = core.GetPosition();
            icon.transform.localPosition = new Vector3(
                corePosition.x * scale,
                corePosition.y * scale,
                corePosition.z * scale
            );
            button.onClick.AddListener(() => OnRespawnAnchorSelected.Invoke(core.id));
        }
        coreLoader.OnOwnedCoreBreaked += (id) => {
            Destroy(icons[id]);
        };
    }
    public void DeactivateUI() {
        battleUIObject.SetActive(true);
    }
 
    // Update is called once per frame
    void Update()
    {
        
    }
}
