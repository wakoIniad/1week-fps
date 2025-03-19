using UnityEngine;
using UnityEngine.Timeline;

public class CoreItem: MonoBehaviour
{
    CoreUIManager coreUIManager;
    public FPSS_PlayerCoreManager CoreManager;
    private CoreManager sourceCore;
    public CoreItem(FPSS_PlayerCoreManager manager, CoreManager source, CoreUIManager viewController) {
        source.OnBreak += Breaked;
        source.OnDamage += Damaged;
        coreUIManager = viewController;
        CoreManager = manager;
        sourceCore = source;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Damaged(float nowHp) {
        coreUIManager.DisplayCoreHealth(nowHp);
    }
    public void Breaked() {
        sourceCore.OnDamage -= Damaged;
        sourceCore.OnBreak -= Breaked;
        
        CoreManager.removeCore(this);
        coreUIManager.Remove();
    }
}
