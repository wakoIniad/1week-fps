using UnityEngine;
using UnityEngine.UI;
using System;

public class TimerProgressRing : MonoBehaviour
{
    public Image progressRing;
    public float waitTime = 2.0f;

    private bool progress = false;

    public Action OnFinish; 

    //private void Start()
    //{
    //    CancelTimer();
    //}

    void Update()
    {
        if (progress)
        {
            progressRing.fillAmount -= 1.0f / waitTime * Time.deltaTime;
            if (0 >= progressRing.fillAmount)
            {
                progress = false;
                progressRing.fillAmount = 1;
                OnFinish?.Invoke();
                gameObject.SetActive(false);
            }
        }
    }

    public void StartTimer()
    {
        gameObject.SetActive(true);
        progressRing = gameObject.GetComponent<Image>();
        progress = true;
        progressRing.fillAmount = 1;
    }

    public void CancelTimer()
    {
        progressRing.fillAmount = 1;
        progress = false;
        gameObject.SetActive(false);
    }
}