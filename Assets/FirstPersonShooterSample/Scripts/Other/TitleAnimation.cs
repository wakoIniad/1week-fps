using System.Collections;
using UnityEngine;

public class TitleAnimation : MonoBehaviour
{
    public GameObject Camera;
    public GameObject ter;
    public SceneLoadButton sceneLoadButton;
    public Vector3[] animationPosition = {
        new Vector3(12,8,0),
        new Vector3(12,8,01),
        new Vector3(12,8,015),
    };
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void StartAnimatiob()
    {
        StartCoroutine(AnimationProcess());
    }
    IEnumerator AnimationProcess() {
        ter.SetActive(false);
        Camera.transform.position =  animationPosition[0];
        yield return new WaitForSeconds(0.5f);
        Camera.transform.position =  animationPosition[1];
        yield return new WaitForSeconds(0.65f);
        Camera.transform.position =  animationPosition[2];
        yield return new WaitForSeconds(0.4f);
        sceneLoadButton.PushButton();
    }
}
