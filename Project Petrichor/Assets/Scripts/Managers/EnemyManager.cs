using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float timeSinceLastEnemySpawned = 0f;
    public int EnemyCountHardCap = 20;

    // Start is called before the first frame update
    void Start()
    {

    }

    void FixedUpdate()
    {
        timeSinceLastEnemySpawned += Time.fixedDeltaTime;
    }

    public void SpawnEnemyAtLocation(Vector2 location)
    {
        if (transform.childCount < EnemyCountHardCap)
        {
            GameObject enemy = Instantiate(enemyPrefab, this.transform, true);
            enemy.transform.position = location;
            enemy.GetComponent<EnemyStateMachine>().SpawnPoint = location;
        }
        timeSinceLastEnemySpawned = 0f;
    }
}
