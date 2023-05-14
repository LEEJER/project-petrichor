using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyManager : MonoBehaviour
{
    public delegate void EnemyDie();
    public static event EnemyDie OnDie;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void RunEnemyDie()
    {
        if (OnDie != null)
        {
            OnDie();
        }
    }
}
