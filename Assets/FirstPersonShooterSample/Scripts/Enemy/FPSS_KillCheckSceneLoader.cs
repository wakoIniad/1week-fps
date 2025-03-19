using UnityEngine;
using UnityEngine.SceneManagement;

//設定した敵が全ていなくなったらシーン移動する

public class FPSS_KillCheckSceneLoader : MonoBehaviour
{
    [SerializeField] string clearSceneName;
    [SerializeField] GameObject[] enemys;

    bool isClear;

    //ゲームをはじめて最初に呼ばれる
    void Start()
    {
        isClear = false;
    }

    //毎フレーム呼ばれる
    void Update()
    {
        //クリア済み、対象が無いときここで終了
        if(isClear){ return; }
        if(enemys == null){ return; }

        //設定した敵が全て存在するか確認する
        bool killcheck = true;
        for(int i = 0; i < enemys.Length; i++)
        {
            killcheck &= (enemys[i] == null);
        }

        //設定した敵が全ていないとき
        if(killcheck)
        {
            //クリア済みにする
            //ロード処理を多重に行わないようにするため
            isClear = true;
            
            //シーンを読み込む
            if(string.IsNullOrEmpty(clearSceneName))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }else
            {
                SceneManager.LoadScene(clearSceneName);
            }
        }
    }
}
