using UnityEngine;
using UnityEngine.UI;
using System;

public class TimerProgressRing : MonoBehaviour
{
    public Image progressRing;
    public float waitTime = 2.0f;

    private bool progress;

    public Action OnFinish; 

    private void Start()
    {
        CancelTimer();
    }

    void Update()
    {
        if (progress)
        {
            progressRing.fillAmount += 1.0f / waitTime * Time.deltaTime;
            if (1 <= progressRing.fillAmount)
            {
                progress = false;
                progressRing.fillAmount = 0;
                OnFinish?.Invoke();
            }
        }
    }

    public void StartTimer()
    {
        progress = true;
        gameObject.SetActive(true);
    }

    public void CancelTimer()
    {
        progressRing.fillAmount = 0;
        progress = false;
        gameObject.SetActive(false);
    }
}