using System;
using System.Collections.Generic;
using UnityEngine;

public class FPSS_PlayerCoreManager : MonoBehaviour
{
    public event Action<string> OnCoreStatusViewClicked;
    public CoreLoader coreLoader;
    public Transform coreStatusViewContainer;
    public GameObject coreStatusViewPrefab;
    //public Dictionary<int, CoreObjectData> ownedCores = new Dictionary<int, CoreObjectData>();
    public Dictionary<string, CoreStatusView> coreView = new Dictionary<string, CoreStatusView>();
    private bool HandleCoreTransportFlagA = false;
    private bool HandleCoreTransportFlagB = false;
    private bool HandleCoreTransportFlag = false;
    private CoreLocalModel transportTarget;
    private CoreLocalModel transportingCoreObject;
    private bool waitForPlace;
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
        //和集合で全ての範囲を取る
        /*if(HandleCoreTransportFlagA || HandleCoreTransportFlagB) {
            if(Input.GetKeyDown(KeyCode.C) && transportingCoreObject == null) {
                transportTarget.TryCollect();
                transportingCoreObject = transportTarget;
                transportTarget = null;
            }
        } else {
            transportTarget = null;
        Debug.Log("TRTargetIsNull:");
        }*/
        if(HandleCoreTransportFlag) {
            if(Input.GetKeyDown(KeyCode.C) && transportingCoreObject == null) {
                transportTarget.TryCollect();
                transportingCoreObject = transportTarget;
                transportTarget = null;
            }
        } else {
            transportTarget = null;
        }
        if(transportingCoreObject != null) {//transporting属性が消えてたらnullにするので大丈夫
            if(transportingCoreObject.transporting && transportingCoreObject.owned) {
                if(Input.GetKeyDown(KeyCode.P)) {
                    waitForPlace = true;
                    transportingCoreObject.TryPlace();
                }
            } else if(waitForPlace){
                waitForPlace = false;
                transportingCoreObject = null;
            }
        }
        HandleCoreTransportFlag = false;
    }
    public void RespownAtCore() {
    }
    public void DamageCore(string id, float hp) {
        coreView[id].DisplayCoreHealth(hp);
    }
    public void AddCore(string id) {
        GameObject statusView = Instantiate(coreStatusViewPrefab, coreStatusViewContainer);
        CoreStatusView viewManager = statusView.GetComponent<CoreStatusView>();
        coreView[id] = viewManager;
        viewManager.DisplayCoreHealth(coreLoader.GetModelById(id).nowHealth);
        viewManager.button.onClick.AddListener(() => OnCoreStatusViewClicked?.Invoke(id));
    }
    public void RemoveCore(string id) {
        coreView[id].Remove();
        coreView.Remove(id);
    }
    /*void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Core")) {
            CoreLocalModel coreModel = other.gameObject.GetComponent<CoreLocalModel>();
            if(coreModel.owned) {
                //HandleCoreTransportFlagAでは、隣接するコアの範囲の共通部分が反応しない
                //HandleCoreTransportFlagBでは、先に入ったコアと共通部分が反応する
                //この範囲を足し合わせれば全ての範囲で反応する
                HandleCoreTransportFlagA = !HandleCoreTransportFlagA;
                HandleCoreTransportFlagB = true;
                transportTarget = coreModel;
            } else {
                coreModel.TryClaim();
            }
        }
    }
    //コアが２つ以上密接している場合、新たなコアの範囲に入った状態で、も解いたコアから出ることで、
    //反応しない可能性があるため対策を考える
    //->対策：falseにするのではなく反転する
    
    void OnTriggerExit(Collider other) {
        if(other.gameObject.CompareTag("Core")) {
            CoreLocalModel coreModel = other.gameObject.GetComponent<CoreLocalModel>();
            if(coreModel.owned) {
                HandleCoreTransportFlagA = !HandleCoreTransportFlagA;
                HandleCoreTransportFlagB = true;
            }
        }
    }*/

    void OnTriggerStay(Collider other) {
        if(other.gameObject.CompareTag("Core")) {
            HandleCoreTransportFlag = true;
        }
    }
    void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Core")) {
            CoreLocalModel coreModel = other.gameObject.GetComponent<CoreLocalModel>();
            if(coreModel.owned) {
                transportTarget = coreModel;
            } else {
                coreModel.TryClaim();
            }
        }
    }
    public int CoreCount() {
        return coreView.Count;
    }
}
