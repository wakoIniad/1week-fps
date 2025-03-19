using UnityEngine;
using UnityEngine.AI;

//地上を移動させる
//"NavMesh"というものに従って移動する
//障害物を避け、最短距離で移動する

[RequireComponent(typeof(NavMeshAgent))]
public class FPSS_WalkEnemy : MonoBehaviour
{
    NavMeshAgent navMeshAgent;

    //ゲームをはじめて最初に呼ばれる
    void Start()
    {
        //キャラクターの移動に用いるものを取得
        navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        //これをしないとあらぬ場所で出現する
        navMeshAgent.Warp(transform.position);
    }

    //毎フレーム呼ばれる
    void Update()
    {
        //目的地を設定する
        navMeshAgent.destination = FPSS_PlayerPosition.GetInstance().GetPosition();
    }
}
