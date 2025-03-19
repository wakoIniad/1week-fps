using System.Collections.Generic;
using UnityEngine;

public class FPSS_PlayerCoreManager : MonoBehaviour
{
    public Transform coreStatusViewContainer;
    public GameObject coreStatusViewPrefab;
    public List<CoreItem> ownedCores = new List<CoreItem>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AddCore(CoreManager source) {
        source.ChangeOwner("me");
        GameObject statusView = Instantiate(coreStatusViewPrefab, coreStatusViewContainer);
        CoreUIManager viewManager = statusView.GetComponent<CoreUIManager>();
        ownedCores.Add(new CoreItem(this, source, viewManager));
    }
    public void removeCore(CoreItem target) {
        ownedCores.Remove(target);
    }
    void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Core")) {
            CoreManager core = other.gameObject.GetComponent<CoreManager>();
            if(core.owner == "") {
                AddCore(core);
            }
        }
    }
}
