using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAudioManager  : MonoBehaviour
{
    public Sound[] walkingClips;
    public Sound[] attackClips;
    public Sound[] dieClips;

    private Sound currentWalkClip;
    private Sound currentAttackClip;
    private Sound currentDieClip;

    private bool canPlay;

    private void Start()
    {
        SetSounds();
        canPlay = UnityEngine.Random.Range(0,3) == 0;
    }

    private void SetSounds()
    {
        // Set audiosource to all the clips
        if (walkingClips.Length > 0 && dieClips.Length > 0 && attackClips.Length > 0)
        {
            foreach (Sound s in walkingClips)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;

                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.loop = s.loop;
            }

            foreach (Sound s in attackClips)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;

                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.loop = s.loop;
            }

            foreach (Sound s in dieClips)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;

                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.loop = s.loop;
            }

            //Randomize clips
            currentWalkClip = walkingClips[UnityEngine.Random.Range(0, walkingClips.Length - 1)];
            currentDieClip = dieClips[UnityEngine.Random.Range(0, dieClips.Length - 1)];
            currentAttackClip = attackClips[UnityEngine.Random.Range(0, attackClips.Length - 1)];
        }
        else
            Debug.LogError("You must add clips to the character's audio manager");
    }

    public void PlayWalking()
    {
        if (canPlay)
            currentWalkClip.source.Play();
    }

    public void PlayDeath()
    {
            currentDieClip.source.Play();
    }

    public void PlayAttack()
    {
        if (canPlay)
            currentAttackClip.source.Play();
    }



    public void StopWalking()
    {
        if (canPlay)
            currentWalkClip.source.Stop();
    }

}
