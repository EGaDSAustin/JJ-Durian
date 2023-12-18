using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioClip clip;

    AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        StartCoroutine(loopMusic());
    }

    IEnumerator loopMusic()
    {
        yield return new WaitUntil(() => !source.isPlaying);
        source.clip = clip;
        source.loop = true;
        source.Play();
    }
}
