using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public AudioSource[] sources;
    public AudioClip[]   audioClips;

    private static AudioManager _instance = null;

    public static AudioManager GetInstance()
    {
        return _instance;
    }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void PlayBGM(AudioClip inClip)
    {
        sources[0].clip = inClip;
        sources[0].Play();
    }

    public void PlaySound(AudioClip inClip)
    {
        float volume = (inClip.name == "wrong") ? 0.5f : 1f;
       sources[1].PlayOneShot(inClip,volume);
    }

    public void PlaySound(int clipNo)
    {
        float volume = (clipNo == 1) ? 0.5f : 1f;
        if (!sources[1].isPlaying)
            sources[1].PlayOneShot(audioClips[clipNo],volume);
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
