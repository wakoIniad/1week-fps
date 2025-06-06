using System;
using System.Collections.Generic;
using UnityEngine;

public class FPSS_PlayerCoreManager : MonoBehaviour
{
    public CoreMapUI coreMapUI;
    [System.NonSerialized] public PlayerManager playerManager;
    public event Action<string> OnCoreStatusViewClicked;
    public CoreLoader coreLoader;
    public Transform coreStatusViewContainer;
    public GameObject coreStatusViewPrefab;
    //public Dictionary<int, CoreObjectData> ownedCores = new Dictionary<int, CoreObjectData>();
    public Dictionary<string, CoreStatusView> coreView = new Dictionary<string, CoreStatusView>();
    public List<string> keys = new List<string>();
    //private bool HandleCoreTransportFlagA = false;
    //private bool HandleCoreTransportFlagB = false;
    private bool HandleCoreTransportFlag = false;
    private CoreLocalModel transportTarget;
    private Dictionary<string,bool> waitForPlace = new Dictionary<string,bool>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        coreLoader.OnCoreOwned += AddCore;
        coreLoader.OnOwnedCoreBreaked += RemoveCore;
        coreLoader.OnOwnedCoreHealthChanged += DamageCore;
    }

    // Update is called once per frame
    //public void OnWarpAnchorSelected(string id) {
    //    coreLoader.TryWarp(id);
    //    coreMapUI.DeactivateUI();
    //    playerManager.ExitUIMde();
    //}
    public void CoreViewHandler(string modelId, bool handle) {
        CoreLocalModel model = coreLoader.GetModelById(modelId);
        if(model.owned) {
            if(model.transporting) {
                coreView[modelId].DisplayTransporting();
                coreLoader.RefreshHpber(model.id);
                if(handle) {
                    model.TryPlace();
                    waitForPlace[modelId] = true;
                }
            } else {
                if(waitForPlace[modelId]) {
                    coreView[modelId].DisplayPlacing();
                    coreLoader.RefreshHpber(model.id);
                    waitForPlace[modelId] = false;
                }
                
                if(handle) {
                    if(transportTarget != null && transportTarget.id == model.id) {
                        model.TryCollect();
                        transportTarget = null;
                    } else {
                        model.TryWarp();
                    }
                }
            }
        }
    }
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
            if(Input.GetKeyDown(KeyCode.C)) {
                transportTarget.TryCollect();
                transportTarget = null;
            }
        } else {
            transportTarget = null;
        }
        //if(Input.GetKeyDown(KeyCode.M)) {
        //    if(coreMapUI.active) {
        //        coreMapUI.DeactivateUI();
        //        coreMapUI.OnRespawnAnchorSelected -= OnWarpAnchorSelected;
        //        playerManager.ExitUIMde();
        //    } else {
        //        coreMapUI.ActivateUI();
        //        coreMapUI.OnRespawnAnchorSelected += OnWarpAnchorSelected;
        //        playerManager.EnterUIMde();
        //    }
        //}
        //if(transportingCoreObject != null) {//transporting属性が消えてたらnullにするので大丈夫
        bool[] alphaInput = {
            Input.GetKeyDown(KeyCode.Alpha0),
            Input.GetKeyDown(KeyCode.Alpha1),
            Input.GetKeyDown(KeyCode.Alpha2),
            Input.GetKeyDown(KeyCode.Alpha3),
            Input.GetKeyDown(KeyCode.Alpha4),
            Input.GetKeyDown(KeyCode.Alpha5),
            Input.GetKeyDown(KeyCode.Alpha6),
            Input.GetKeyDown(KeyCode.Alpha7),
            Input.GetKeyDown(KeyCode.Alpha8),
            Input.GetKeyDown(KeyCode.Alpha9),
            };
            
        for(int i = 0; i < Math.Min(10, keys.Count); i++) {
            CoreViewHandler(keys[i], alphaInput[i]);
            //CoreLocalModel model = coreLoader.GetModelById(keys[i]);
            //if(model.owned) {
            //    if(model.transporting) {
            //        coreView[keys[i]].DisplayTransporting();
            //        coreLoader.RefreshHpber(model.id);
            //        if(alphaInput[i]) {
            //            model.TryPlace();
            //            waitForPlace[keys[i]] = true;
            //        }
            //    } else {
            //        if(waitForPlace[keys[i]]) {
            //            coreView[keys[i]].DisplayPlacing();
            //            coreLoader.RefreshHpber(model.id);
            //            waitForPlace[keys[i]] = false;
            //        }
            //        if(alphaInput[i]) {
            //            model.TryWarp();
            //        }
            //    }
            //}
        }//

        
            /*if(transportingCoreObject.transporting && transportingCoreObject.owned) {
                coreView[transportingCoreObject.id].DisplayTransporting();
                if(Input.GetKeyDown(KeyCode.P)) {
                    waitForPlace = true;
                    transportingCoreObject.TryPlace();
                }
            } else if(waitForPlace){
                coreView[transportingCoreObject.id].DisplayPlacing();
                waitForPlace = false;
                transportingCoreObject = null;
            }*/
        //}
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
        viewManager.coreId = id;
        viewManager.playerCoreManager = this;
        coreView.Add(id, viewManager);
        waitForPlace.Add(id, false);
        keys.Add(id);
        viewManager.DisplayCoreHealth(coreLoader.GetModelById(id).nowHealth);
        viewManager.button.onClick.AddListener(() => OnCoreStatusViewClicked?.Invoke(id));
        RectTransform statViewRectTr = coreStatusViewContainer.transform.gameObject.GetComponent<RectTransform>();
        statViewRectTr.sizeDelta = new Vector2(statViewRectTr.sizeDelta.x, 100*coreView.Count);
    }
    public void RemoveCore(string id) {
        coreView[id].Remove();
        coreView.Remove(id);
        waitForPlace.Remove(id);
        keys.Remove(id);
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
            CoreLocalModel coreModel = other.gameObject.GetComponent<CoreLocalModel>();
            if(coreModel.owned) {
                transportTarget = coreModel;
                HandleCoreTransportFlag = true;
            }
        }
    }
    void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Core")) {
            CoreLocalModel coreModel = other.gameObject.GetComponent<CoreLocalModel>();
            if(coreModel.owned) {
                //transportTarget = coreModel;
            } else {
                coreModel.TryClaim();
            }
        }
    }
    public int CoreCount() {
        return coreView.Count;
    }
}
