using UnityEngine;
using UnityEngine.SceneManagement;

//UIのボタンでシーンのロードをしたいときに使う
//ボタン側の設定も忘れずに

public class SceneLoadButton : MonoBehaviour
{
    [SerializeField] string sceneName;

    bool isLoading = false;


    //これをボタンから呼ばれるようにする必要がある
    public void PushButton()
    {
        //既にロードされている時はここで終了
        if(isLoading){return;}

        //ロード済みにする
        //ロード処理を多重に行わないようにするため
        isLoading = true;

        if(string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }else
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
