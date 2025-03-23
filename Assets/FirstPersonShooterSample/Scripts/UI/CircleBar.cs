using UnityEngine;
using UnityEngine.UI;
public class CircleBar : MonoBehaviour
{
    public Image progressRing;

    public void UpdateBar(float percentage) {
        progressRing.fillAmount = percentage;
    }
}