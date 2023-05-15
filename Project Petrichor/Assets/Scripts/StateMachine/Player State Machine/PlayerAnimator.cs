using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Transform parent;
    private PlayerStateMachine sm;
    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent;
        sm = parent.GetComponent<PlayerStateMachine>();
    }

    public void Event_AttackState_AllowInterrupt()      { sm.Event_AttackState_AllowInterrupt(); }
    public void Event_AttackState_EndSwordAttack()      { sm.Event_AttackState_EndSwordAttack(); }
    public void Event_AttackState_AllowBuffer()         { sm.Event_AttackState_AllowBuffer(); }
    public void Event_DashState_AllowInterrupt()        { sm.Event_DashState_AllowInterrupt(); }
    public void Event_DashState_AllowNewDash()          { sm.Event_DashState_AllowNewDash(); }
    public void Event_DashState_EndDash()               { sm.Event_DashState_EndDash(); }
    public void Event_DeflectState_AllowInterrupt()     { sm.Event_DeflectState_AllowInterrupt(); }
    public void Event_DeflectState_AllowBuffer()        { sm.Event_DeflectState_AllowBuffer(); }
    public void Event_DeflectState_EndDeflect()         { sm.Event_DeflectState_EndDeflect(); }
    public void Event_DeflectState_RemoveDeflectFrames(){ sm.Event_DeflectState_RemoveDeflectFrames(); }
    public void Event_DeflectHitState_AllowInterrupt()  { sm.Event_DeflectHitState_AllowInterrupt(); }  
    public void Event_DeflectHitState_AllowBuffer()     { sm.Event_DeflectHitState_AllowBuffer(); }     
    public void Event_DeflectHitState_EndDeflectHit()   { sm.Event_DeflectHitState_EndDeflectHit(); }
    public void Event_HitState_AllowInterrupt()         { sm.Event_HitState_AllowInterrupt(); }
    public void Event_HitState_EndHit()                 { sm.Event_HitState_EndHit(); }
    public void Event_DieState_EndDie()                 { sm.Event_DieState_EndDie(); }
}
