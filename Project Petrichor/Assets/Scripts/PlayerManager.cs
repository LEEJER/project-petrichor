using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject playerPrefab;

    public float    TimeAlive       = 0f;
    public int      EnemiesKilled   = 0;

    private void Start()
    {
        GameManager.instance.OnEnemyDie.AddListener(EnemyKillCounterIncrement);
        GameManager.instance.FindEnemyManager();
        GameManager.instance.FindPlayerManager();
        GameManager.instance.FindUIManager();

        GameObject player = Instantiate(playerPrefab, this.transform, true);
        player.transform.position = Vector2.zero;
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
