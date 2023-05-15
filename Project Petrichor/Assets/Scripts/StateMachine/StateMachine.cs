using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    public abstract void OnHitboxEnter(Collider2D collision, bool isTrigger, string component);
    public abstract void OnHitboxStay(Collider2D collision, bool isTrigger, string component);
    public abstract void OnHitboxExit(Collider2D collision, bool isTrigger, string component);
}
