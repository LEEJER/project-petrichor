using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordScript : MonoBehaviour
{
    private Transform swordBody;
    private Rigidbody2D swordRB;

    private Coroutine swing;
    private bool swingIsRunning = false;
    private bool continueSwing = false;
    private int swingDir = 1;
    private float timeOfLastSwing;

    // Start is called before the first frame update
    void Start()
    {
        swordBody = transform.Find("SwordBody");
        swordRB = GetComponent<Rigidbody2D>();
        timeOfLastSwing = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsSwordSwinging()
    {
        return swingIsRunning;
    }

    public void SwingSword(Vector2 direction) 
    {
        // reset swingDir if enough time has passed.
        if (Time.time - timeOfLastSwing > 1f) { swingDir = 1; }

        // update values
        timeOfLastSwing = Time.time;
        continueSwing = true;
        if (!swingIsRunning)
        {
            swing = StartCoroutine(SwingSwordCoroutine(direction, 160, 1));
        }
    }

    private IEnumerator SwingSwordCoroutine(Vector3 direction, int steps, float angleDegrees) 
    {
        swordBody.GetComponent<SpriteRenderer>().enabled = true;
        swordBody.GetComponent<BoxCollider2D>().enabled = true;

        swingIsRunning = true;
        float swingSpeed = 6;
        float swingTime = 0.2f;

        // get the appropriate rotations
        float startRotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float currRot = (startRotation - (steps * angleDegrees / 2) * swingDir);
        swordRB.SetRotation(currRot);

        // swing as long as we should
        while (continueSwing)
        {
            continueSwing = false;
            currRot = (startRotation - (steps * angleDegrees / 2) * swingDir);
            // swing
            for (int i = 1; i <= steps; i++)
            {
                float move = EaseSwingAnimation((float)i / steps, swingSpeed) * steps * angleDegrees;
                float newRot = currRot + move * swingDir;
                swordRB.MoveRotation(newRot);

                if (newRot >= 0 && newRot <= 180)
                {
                    swordBody.GetComponent<SpriteRenderer>().sortingOrder = -1;
                } else
                {
                    swordBody.GetComponent<SpriteRenderer>().sortingOrder = 1;
                }
                 
                yield return new WaitForSeconds(swingTime / steps);
            }
            // change direction
            swingDir *= -1;
        }
        // hide sword, end swing
        swordBody.GetComponent<SpriteRenderer>().enabled = false;
        swordBody.GetComponent<BoxCollider2D>().enabled = false;
        swingIsRunning = false;
        continueSwing = false;
    }

    //https://stackoverflow.com/questions/13462001/ease-in-and-ease-out-animation-formula
    private float EaseSwingAnimation(float t, float a)
    {
        return Mathf.Pow(t, a) / (Mathf.Pow(t, a) + Mathf.Pow((1-t), a));
    }
}
