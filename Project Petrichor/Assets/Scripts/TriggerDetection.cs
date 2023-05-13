using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDetection : MonoBehaviour
{
    public GameObject obj;
    private StateMachine machine;
    public string component;

    private void Start()
    {
        machine = obj.GetComponent<StateMachine>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject gameObject = collision.gameObject;
        if (gameObject != null && gameObject.layer != transform.gameObject.layer)
        {
            machine.OnHitboxEnter(collision, true, component);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        GameObject gameObject = collision.gameObject;
        if (gameObject != null && gameObject.layer != transform.gameObject.layer)
        {
            machine.OnHitboxStay(collision, true, component);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject gameObject = collision.gameObject;
        if (gameObject != null && gameObject.layer != transform.gameObject.layer)
        {
            machine.OnHitboxExit(collision, true, component);
        }
    }
}
