using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
      /*  if (MySceneManager.Instance.CurrentLevelName != "LastLevel")
        {
            Play("Theme");
        }*/
    }

    public void DeathGame()
    {
        Stop("Theme");
        Play("PlayerDeath");
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, Sound => Sound.name == name);
        //GUARD CLAUSE
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " nout found!");
            return;
        }
        s.source.Play();
        Debug.Log("Im playing");
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, Sound => Sound.name == name);
        //GUARD CLAUSE
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " nout found!");
            return;
        }
        s.source.Stop();
    }
}
