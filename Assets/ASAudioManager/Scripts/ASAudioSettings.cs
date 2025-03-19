using UnityEngine;
using UnityEngine.UI;
using TMPro;

//音声設定: 設定画面

public class ASAudioSettings : MonoBehaviour
{
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider seSlider;
    [SerializeField] TMP_Text masterVolumeText;
    [SerializeField] TMP_Text bgmVolumeText;
    [SerializeField] TMP_Text seVolumeText;

    void OnEnable()
    {
        float masterVolume = 1;
        float bgmVolume = 1;
        float seVolume = 1;
        ASAudioManager.GetVolumes(out masterVolume, out seVolume, out bgmVolume);
        masterSlider.value = masterVolume;
        bgmSlider.value = bgmVolume;
        seSlider.value = seVolume;
        UpdateText();
    }

    public void ChangeSlider()
    {
        ASAudioManager.SetVolumes(masterSlider.value, seSlider.value, bgmSlider.value);
        UpdateText();
    }

    void UpdateText()
    {
        masterVolumeText.text = masterSlider.value.ToString("0.0");
        bgmVolumeText.text = bgmSlider.value.ToString("0.0");
        seVolumeText.text = seSlider.value.ToString("0.0");
    }
}
