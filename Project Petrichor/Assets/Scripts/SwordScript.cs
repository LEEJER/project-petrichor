using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordScript : MonoBehaviour
{
    private Transform swordBody;
    private Rigidbody2D swordRB;
    private Coroutine swing;

    // Start is called before the first frame update
    void Start()
    {
        swordBody = transform.Find("SwordBody");
        swordRB = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwingSword() 
    {
        
        // https://docs.unity3d.com/ScriptReference/Camera.ScreenToWorldPoint.html
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        // Vector2 mousePosition2D = new Vector2(mousePos.x, mousePos.y);
        // Vector2 direction = transform.position - mousePos;
        // https://docs.unity3d.com/560/Documentation/Manual/DirectionDistanceFromOneObjectToAnother.html
        Vector3 heading = mousePos - transform.position;
        Vector3 direction = heading / heading.magnitude;
        // Debug.Log(direction);
        if (swing != null) { StopCoroutine(swing); }
        swing = StartCoroutine(SwingSwordCoroutine(direction));
    }

    private IEnumerator SwingSwordCoroutine(Vector3 direction) 
    {
        swordBody.GetComponent<SpriteRenderer>().enabled = true;

        int steps = 120;
        float angleDegrees = 1;

        // https://www.youtube.com/watch?v=mKLp-2iseDc
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - steps*angleDegrees/2;
        swordRB.SetRotation(angle);

        float rotation = swordRB.rotation;

        for (int i = 1; i <= steps; i++) 
        {
            float move = EaseSwingAnimation((float)i/(float)steps, 4) * steps * angleDegrees;
            swordRB.MoveRotation(rotation + move);
            yield return new WaitForSeconds(0.35f/steps);
        }
        swordBody.GetComponent<SpriteRenderer>().enabled = false;
    }

    //https://stackoverflow.com/questions/13462001/ease-in-and-ease-out-animation-formula
    private float EaseSwingAnimation(float t, float a)
    {
        // float sqt = t * t;
        // return sqt / (2.0f * (sqt-t) + 1.0f);
        return Mathf.Pow(t, a) / (Mathf.Pow(t, a) + Mathf.Pow((1-t), a));
    }
}
