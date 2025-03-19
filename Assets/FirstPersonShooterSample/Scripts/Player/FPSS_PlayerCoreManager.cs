using System;
using System.Collections.Generic;
using UnityEngine;

public class FPSS_PlayerCoreManager : MonoBehaviour
{
    public event Action<int> OnCoreStatusViewClicked;
    public CoreLoader coreLoader;
    public Transform coreStatusViewContainer;
    public GameObject coreStatusViewPrefab;
    //public Dictionary<int, CoreObjectData> ownedCores = new Dictionary<int, CoreObjectData>();
    public Dictionary<int, CoreStatusView> coreView = new Dictionary<int, CoreStatusView>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        coreLoader.OnCoreOwned += AddCore;
        coreLoader.OnOwnedCoreBreaked += RemoveCore;
        coreLoader.OnOwnedCoreDamaged += DamageCore;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void RespownAtCore() {
    }
    public void DamageCore(int id, float hp) {
        coreView[id].DisplayCoreHealth(hp);
    }
    public void AddCore(int id) {
        GameObject statusView = Instantiate(coreStatusViewPrefab, coreStatusViewContainer);
        CoreStatusView viewManager = statusView.GetComponent<CoreStatusView>();
        coreView[id] = viewManager;
        viewManager.DisplayCoreHealth(coreLoader.GetCoreById(id).nowHealth);
        viewManager.button.onClick.AddListener(() => OnCoreStatusViewClicked?.Invoke(id));
    }
    public void RemoveCore(int id) {
        coreView[id].Remove();
        coreView.Remove(id);
    }
    void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Core")) {
            CoreObjectData coreObject = other.gameObject.GetComponent<CoreObjectData>();
            coreObject.TryClaim();
        }
    }

    public int CoreCount() {
        return coreView.Count;
    }
}
