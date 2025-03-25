using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class OnepointSE : MonoBehaviour
{
    AudioSource audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    public void play() {
        audioSource.Play();
    }
}
