using UnityEngine;

//音声設定: AudioSourceに反映させる
//AudioSourceにつける

[RequireComponent(typeof(AudioSource))]
public class ASAudioController : MonoBehaviour
{
    [SerializeField] ASAudioManager.AudioType audioType;

    float startvolume;
    float nowvolume;
    AudioSource audioSource;

    void OnEnable()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        startvolume = audioSource.volume;
        ASAudioManager.Add(audioType, ChangeVolume);
    }

    void OnDisable()
    {
        audioSource.volume = startvolume;
        ASAudioManager.Remove(audioType, ChangeVolume);
    }

    void ChangeVolume(float volume)
    {
        nowvolume = volume;
        audioSource.volume = startvolume * volume;
    }

    public void SetVolume(float volume)
    {
        startvolume = volume;
        ChangeVolume(nowvolume);
    }
}
