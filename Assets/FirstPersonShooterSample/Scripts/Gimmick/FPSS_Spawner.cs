using UnityEngine;

//モノを出現させる
//一定周期毎に行う

public class FPSS_Spawner : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] float cycleTime = 1;

    float nowTime;

    //ゲームをはじめて最初に呼ばれる
    void Start()
    {
        nowTime = 0;
    }

    //毎フレーム呼ばれる
    void Update()
    {
        nowTime += Time.deltaTime;

        if(nowTime >= cycleTime)
        {
            Spawn();
            nowTime = 0;
        }
    }

    //生成する
    void Spawn()
    {
        GameObject obj = Instantiate(prefab);
        obj.transform.position = transform.position;
        obj.transform.rotation = transform.rotation;
    }
}
