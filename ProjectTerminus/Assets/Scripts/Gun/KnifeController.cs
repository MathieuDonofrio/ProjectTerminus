using Boo.Lang;
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
    public GameObject player;

    public float AttackRange = 4f;
    public float AttackDelay = 2f;
    public float AttackDuration = .2f;

    public float Damage = 5f;
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

        //comments
        GameObject entity = SearchUtil.FindClosest(SearchUtil.FindEntitesInRange(player, AttackRange),transform.position);

        if (entity != null)
        {
            bool isDead = entity.GetComponent<Entity>().Damage(Damage,player,DamageType.PHYSICAL);
            player.GetComponentInChildren<HUDController>().Hitmarker(isDead);
        }
    }

}
