using UnityEngine;
using UnityEngine.SceneManagement;

//ゴール

public class FPSGoalScript : MonoBehaviour
{
    [SerializeField] string sceneName = "";//移動先シーン名

    bool isClear;

    //ゲームをはじめて最初に呼ばれる
    private void Start()
    {
        isClear = false;
    }

    //なんかぶつかった時に実行
    private void OnTriggerEnter(Collider other)
    {
        //既にクリア処理してる場合終了
        if (isClear) { return; }

        //ぶつかったもののTagがEnemyだったとき
        if (other.CompareTag("Player")) 
        {
            //クリア済みにする
            //ロード処理を多重に行わないようにするため
            isClear = true;

            //指定シーンをロードする
            if(string.IsNullOrEmpty(sceneName)) 
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }else
            {
                SceneManager.LoadScene(sceneName);
            }
        }
    }

}
