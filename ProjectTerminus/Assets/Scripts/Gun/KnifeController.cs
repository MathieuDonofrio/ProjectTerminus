using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioClip))]
[RequireComponent(typeof(Animator))]
public class KnifeController : MonoBehaviour
{
    private AudioSource audioSource;
    private Animator knifeAnimator;

    public float AttackRange;
    public float AttackDelay;
    public float AttackDuration = .2f;
    private float lastAttackTime = Mathf.NegativeInfinity;

    private int layerMask;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        knifeAnimator = GetComponent<Animator>();
        layerMask = LayerMask.GetMask("Entity");

    }

    void Update()
    {
        StartAttack();   
    }

    void StartAttack()
    {
        if (Input.GetKeyDown(KeyCode.B) && Time.time - lastAttackTime >= AttackDuration)
        {
            knifeAnimator.SetTrigger("attacking");
            audioSource.PlayOneShot(audioSource.clip);
            FinishAttack();
        }

    }


    void FinishAttack()
    {
        //Register last attack time
        lastAttackTime = Time.time;
    }

}
