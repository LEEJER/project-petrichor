using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTriggerDetection : MonoBehaviour
{
    EnemyStateMachine enemy;

    // Start is called before the first frame update
    void Start()
    {
        enemy = transform.parent.GetComponent<EnemyStateMachine>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        enemy.OnDetectionBoxEnter(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        enemy.OnDetectionBoxStay(collision);
    }
}
