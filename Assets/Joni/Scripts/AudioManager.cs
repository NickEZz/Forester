using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Range(0f, 1f)]
    public float volumeMaster;

    [SerializeField] Sound[] sounds;

    [SerializeField] bool muted;

    private void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.spatialBlend = s.spatial;
            s.source.loop = s.loop;
        }

        Sound wind = Array.Find(sounds, sound => sound.name == "windambience");
        wind.source.loop = true;
        PlaySound("windambience", Vector3.zero);
    }

    private void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.M))
        {
            muted = !muted;
        }

        if (muted)
        {
            foreach (Sound s in sounds)
            {
                s.source.volume = 0;
            }
        }
        else
        {
            foreach (Sound s in sounds)
            {
                s.source.volume = s.volume * volumeMaster;
            }
        }
        */

        foreach (Sound s in sounds)
        {
            s.source.volume = s.volume * volumeMaster;
        }
    }

    public void PlaySound(string name, Vector3 pos)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        transform.position = pos;
        s.source.Play();
    }

    public void StopSound(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Stop();
    }
}

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume;
    [Range(0f, 3f)]
    public float pitch;
    
    public bool loop;

    [Range(0f, 1f)]
    public float spatial;

    [HideInInspector]
    public AudioSource source;
}
