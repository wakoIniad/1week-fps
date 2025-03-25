using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using Unity.VisualScripting;

public class RespawnUI : MonoBehaviour
{
    public bool active = false;
    public PlayerModelLoader playerModelLoader;
    public GameObject battleUIObject;
    public GameObject systemUIObject;
    
    public GameObject respawnUIObject;
    public GameObject mapObject;
    public CoreLoader coreLoader;
    public GameObject respawnPointIconPrefab;
    RectTransform mapRectTransform;
    //下記2変数はサーバーと（手動で）同期するように！
    public float MAP_SIZE = 64;
    public float WARP_COST = 50;
    public event Action<string> OnRespawnAnchorSelected; 
    public GameObject playerIcon;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    Dictionary<string, GameObject> icons;

    public void ActivateUI() {
        mapRectTransform = mapObject.GetComponent<RectTransform>();
        battleUIObject.SetActive(false);
        systemUIObject.SetActive(false);
        respawnUIObject.SetActive(true);
        active = true;
        CoreLocalModel[] cores = new CoreLocalModel[coreLoader.CoreList.Count];
        coreLoader.CoreList.Values.CopyTo(cores,0);
        float mapDisplayWidth = mapRectTransform.sizeDelta.x;
        float scale = mapDisplayWidth/MAP_SIZE;
        icons = new Dictionary<string, GameObject>();

        //中心が0になる為、それの調整用。
        float offset = mapDisplayWidth/2;
        for(int i = 0;i < cores.Length; i++) {
            CoreLocalModel core = cores[i];
            if(!core.owned || core.transporting)continue;
            GameObject icon = Instantiate(respawnPointIconPrefab, mapObject.transform);
            icons[core.id] = icon;
            Button button = icon.GetComponent<Button>();
            Vector3 corePosition = core.GetPosition();
            icon.transform.localPosition = new Vector3(
                corePosition.x * scale - offset,
                corePosition.z * scale - offset,
                corePosition.y * scale - offset
            );
            button.onClick.AddListener(() => {
                if(CheckHealth(core.id, core.nowHealth)) {
                    if(OnRespawnAnchorSelected!=null)OnRespawnAnchorSelected.Invoke(core.id);
                }
            });
            CheckHealth(core.id, core.nowHealth);
        }
        coreLoader.OnOwnedCoreBreaked += OnCoreBreak;
        coreLoader.OnOwnedCoreHealthChanged += OnHealthChange;
        Transform playerTransform = playerModelLoader.GetMyTransform();
        if(playerModelLoader.MyHealthManager.playerHealth == 0) {
            playerIcon.SetActive(false);
        } else {
            playerIcon.SetActive(true);
            playerIcon.transform.localPosition = new Vector3(
                    playerTransform.position.x * scale - offset,
                    playerTransform.position.z * scale - offset,
                    playerTransform.position.y * scale - offset
            );
        }
    }
    bool CheckHealth(string id, float health) {
        Image[] imgs = icons[id].GetComponentsInChildren<Image>();
        ImageBar bar = icons[id].GetComponent<ImageBar>();
        //Debug.Log("Lossy:"+bar.maskObject.transform.parent.lossyScale.y);
        //Debug.Log("LossyParent:"+bar.maskObject.transform.parent.lossyScale.y);
        //Debug.Log("local:"+bar.maskObject.transform.localScale.y);
        //Debug.Log("localParent:"+bar.maskObject.transform.parent.localScale.y);
        
        bar.UpdateBar(health/CoreLocalModel.defaultHealth);
        
        if(health < WARP_COST) {
            for(int i = 0;i < imgs.Length; i++) {
                Image img = imgs[i];
                img.color = new Color(128, 0, 0);
            }
            return false;
        } else {
            for(int i = 0;i < imgs.Length; i++) {
                Image img = imgs[i];
                img.color = new Color(255, 255, 255);
            }
            return true;
        }
    }
    void OnCoreBreak(string id) {
        //運搬中のコアのアイコンはないため
        if(icons.ContainsKey(id))Destroy(icons[id]);
    }
    void OnHealthChange(string id, float hp) {
        //運搬中のコアのアイコンはないため
        if(icons.ContainsKey(id))CheckHealth(id, hp);
    }
    public void DeactivateUI() {
        battleUIObject.SetActive(true);
        respawnUIObject.SetActive(false);
        active = false;
        string[] keys = new string[icons.Count];
        icons.Keys.CopyTo(keys,0);
        for(int i = 0;i < icons.Count; i++) {
            Destroy(icons[keys[i]]);
        }
        //icons = new Dictionary<string, GameObject>();
        coreLoader.OnOwnedCoreBreaked -= OnCoreBreak;
        coreLoader.OnOwnedCoreHealthChanged -= OnHealthChange;
    }
 
    // Update is called once per frame
    void Update()
    {
        
    }
}
