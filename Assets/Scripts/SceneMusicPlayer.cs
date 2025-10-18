using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class SceneMusicPlayer : MonoBehaviour
{
    private AudioSource musicSource;

    void Start()
    {
        musicSource = GetComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.Play();
    }

}