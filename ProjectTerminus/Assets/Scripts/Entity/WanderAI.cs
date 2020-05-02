using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class WanderAI : MonoBehaviour
{

    public float moveSpeed = 3f;
    public float rotSpeed = 100f;

    private bool isWandering = false;
    private bool isRotatingRight = false;
    private bool isWalking = false;

    private Animator zombieAnimator;


    // Start is called before the first frame update
    void Start()
    {
        zombieAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isWandering)
        {
            StartCoroutine(Wander());
        }

        if (isRotatingRight)
        {
            transform.Rotate(transform.up * Time.deltaTime * rotSpeed);
        }
        if (!isRotatingRight)
        {
            transform.Rotate(transform.up * Time.deltaTime * -rotSpeed);
        }
        if (isWalking)
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }

        //Set animation
        zombieAnimator.SetBool("walking", isWalking);
    }

    private IEnumerator Wander()
    {
        //amount of time that the zombie will be rotating
        int rotTime = Random.Range(1,3);
        //amount of time we will wait between rotations
        int rotateWait = Random.Range(1, 3);
        int rotateLorR = Random.Range(1, 2);
        int walkWait = Random.Range(1, 10);
        int walkTime = Random.Range(2, 10);

        isWandering = true;

        yield return new WaitForSeconds(walkWait);
        isWalking = true;
        yield return new WaitForSeconds(walkTime);
        isWalking = false;
        yield return new WaitForSeconds(rotateWait);
        if (rotateLorR == 1)
        {
            isRotatingRight = true;
            yield return new WaitForSeconds(rotTime);
            isRotatingRight = false;
        }

        if (rotateLorR == 2)
        {
            isRotatingRight = false;
            yield return new WaitForSeconds(rotTime);
            isRotatingRight = true;
        }
        isWandering = false;
    }
}
