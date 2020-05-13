
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public Sound[] walkingClips;
    public Sound[] attackClips;
    public Sound[] dieClips;

    private Sound currentWalkClip;
    private Sound currentAttackClip;
    private Sound currentDieClip;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Always walks the same
        currentWalkClip = walkingClips[Mathf.FloorToInt(Random.value * walkingClips.Length)];
    }

    public void PlayWalking()
    {
        if (audioSource == null)
            return;

        audioSource.volume = currentWalkClip.volume;
        audioSource.pitch = currentWalkClip.pitch;
        audioSource.loop = currentWalkClip.loop;

        audioSource.clip = currentWalkClip.clip;

        audioSource.PlayOneShot(currentWalkClip.clip);
    }

    public void PlayDeath()
    {
        if (audioSource == null)
            return;

        currentDieClip = dieClips[Mathf.FloorToInt(Random.value * dieClips.Length)];

        audioSource.volume = currentDieClip.volume;
        audioSource.pitch = currentDieClip.pitch;
        audioSource.loop = currentDieClip.loop;

        audioSource.clip = currentDieClip.clip;

        audioSource.PlayOneShot(currentDieClip.clip);
    }

    public void PlayAttack()
    {
        if (audioSource == null)
            return;

        currentAttackClip = attackClips[Mathf.FloorToInt(Random.value * attackClips.Length)];

        audioSource.volume = currentAttackClip.volume;
        audioSource.pitch = currentAttackClip.pitch;
        audioSource.loop = currentAttackClip.loop;

        audioSource.PlayOneShot(currentAttackClip.clip);
    }

    public void StopWalking()
    {
        if (audioSource == null)
            return;

        audioSource.Stop();
    }

}

[System.Serializable]
public class Sound
{
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume;

    [Range(.1f, 3f)]
    public float pitch;

    public bool loop;
}
