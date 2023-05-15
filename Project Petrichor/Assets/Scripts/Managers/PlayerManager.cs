using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public Transform spawnPoint;

    public Transform testMarker;

    public float    TimeAlive       = 0f;
    public int      EnemiesKilled   = 0;
    public float    DifficultyInterval = 30f;
    public float    DifficultyMultiplier = 1f;

    public Collider2D _overlap;

    public PlayerStateMachine player;

    private float timeBetweenEnemySpawns = 5f;
    private int enemySpawnCountPityLevel = 6;

    private void Start()
    {
        GameManager.instance.OnEnemyDie.AddListener(EnemyKillCounterIncrement);
        GameManager.instance.FindEnemyManager();
        GameManager.instance.FindPlayerManager();
        GameManager.instance.FindUIManager();

        GameObject player = Instantiate(playerPrefab, this.transform, true);
        player.transform.position = spawnPoint.position;

        this.player = player.GetComponent<PlayerStateMachine>();

        LevelManager levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        levelManager.SetCameraFollow(player);

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        TimeAlive += Time.fixedDeltaTime;
        if (transform.childCount <= 0)
        {
            AllPlayersDead();
            GameManager.instance.SavePlayerScore();
        }

        DifficultyMultiplier = 1 + 0.1f * Mathf.FloorToInt(TimeAlive / DifficultyInterval);
        player.difficultyMultiplier = DifficultyMultiplier;

        // the time between spawns should be augmented by 2 second for each enemy over the soft cap
        float checkConditionsForEnemySpawning = timeBetweenEnemySpawns + 2f*(Mathf.Max(0f, GameManager.instance.EnemyManager.transform.childCount - enemySpawnCountPityLevel));

        if (GameManager.instance.EnemyManager.timeSinceLastEnemySpawned >= checkConditionsForEnemySpawning)
        {
            // try to spawn an enemy
            Vector2 relativeDirection = Vector2.right;

            // rotate the vector2
            relativeDirection = Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.forward) * relativeDirection;
            // place it somewhere from 1.5 units to 2.5 units away
            relativeDirection *= Random.Range(1.5f, 2.5f);
            // check if it is a valid spawning location
            Collider2D overlap = Physics2D.OverlapCircle(relativeDirection + player.PlayerRidigbody.position, 0.2f, LayerMask.GetMask("Environment", "Enemy"));
            //Collider2D overlap2 = Physics2D.OverlapCircle(testMarker.position, 0.5f, LayerMask.GetMask("Environment"));
            //_overlap = overlap2;
            //if (overlap2 != null)
            //{
            //    Debug.Log("CollisionDetected");
            //} 
            //if (overlap2 == null)
            //{
            //    Debug.Log("none");
            //}
            //RaycastHit2D hit = Physics2D.CircleCast(relativeDirection, 0.2f, Vector2.right, 0.1f, LayerMask.NameToLayer("Environment") | LayerMask.NameToLayer("Enemy"));
            //if (hit.collider == null)
            //{
            //    GameManager.instance.EnemyManager.SpawnEnemyAtLocation(relativeDirection + player.PlayerRidigbody.position);
            //} else
            //{
            //    //Debug.Log(hit.collider.gameObject.name);
            //    //Debug.Log("collider detected");
            //}
            if (overlap == null)
            {
                GameManager.instance.EnemyManager.SpawnEnemyAtLocation(relativeDirection + player.PlayerRidigbody.position);
            }
        }
    }

    private void OnDisable()
    {
        GameManager.instance.OnEnemyDie.RemoveListener(EnemyKillCounterIncrement);
    }

    public void EnemyKillCounterIncrement()
    {
        EnemiesKilled++;
    }

    public void AllPlayersDead()
    {
        GameManager.instance.GameOver();
    }
}
