using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public GameObject playerPrefab;

    public float    TimeAlive       = 0f;
    public int      EnemiesKilled   = 0;

    private void Start()
    {
        GameManager.instance.OnEnemyDie.AddListener(EnemyKillCounterIncrement);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        TimeAlive += Time.fixedDeltaTime;
        if (transform.childCount <= 0)
        {
            AllPlayersDead();
        }
    }

    private void OnDisable()
    {
        GameManager.instance.SavePlayerScore();
        GameManager.instance.OnEnemyDie.RemoveListener(EnemyKillCounterIncrement);
    }

    public void EnemyKillCounterIncrement()
    {
        EnemiesKilled++;
    }

    public void AllPlayersDead()
    {
        SceneManager.LoadScene("GameOver");
    }
}
