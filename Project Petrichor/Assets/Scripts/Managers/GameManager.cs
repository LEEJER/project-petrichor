using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject       UIManager;
    public PlayerManager    PlayerManager;
    public EnemyManager     EnemyManager;
    public GameObject       Environment;

    private Transform _healthBar;
    private Transform _canvas;

    [SerializeField]
    private float _playerTime;
    [SerializeField]
    private float _playerKills;

    [SerializeField]
    public UnityEvent OnEnemyDie;
    // Start is called before the first frame update
    void Start()
    {
        if (OnEnemyDie == null)
        {
            OnEnemyDie = new UnityEvent();
        }
    }

    private void Awake()
    {
        // singleton
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        } 
        else
        {
            instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    public void SavePlayerScore()
    {
        _playerKills = PlayerManager.EnemiesKilled;
        _playerTime = PlayerManager.TimeAlive;
    }

    private void OnDisable()
    {
        OnEnemyDie.RemoveAllListeners();
    }

    public void Run_OnEnemyDie()
    {
        OnEnemyDie.Invoke();
    }

    public void SetBarPercentage(float val)
    {

        if (UIManager != null)
        {
            _healthBar = UIManager.transform.Find("Canvas/HealthBar/Bar");
        }
        if (_healthBar != null)
        {
            _healthBar.GetComponent<Image>().rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 200f * val);
        }
    }

    public void SetGameOverScore()
    {
        Debug.Log("Setting Score");
        TextMeshProUGUI score = null;
        TextMeshProUGUI timeAlive = null;
        if (UIManager != null)
        {
            score = UIManager.transform.Find("Canvas/Menu/Score").GetComponent<TextMeshProUGUI>();
            timeAlive = UIManager.transform.Find("Canvas/Menu/TimeAlive").GetComponent<TextMeshProUGUI>();
        }
        if (score != null)
        {
            score.text = "Enemies Killed: " + _playerKills;
        }
        if (timeAlive != null)
        {
            timeAlive.text = "Time Survived: " + _playerTime;
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Level1");
    }
    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void GameOver()
    {
        SceneManager.LoadScene("GameOver");
    }
    public void FindUIManager()
    {
        UIManager = GameObject.Find("UIManager");
    }
    public void FindPlayerManager()
    {
        GameObject playerManager = GameObject.Find("PlayerManager");
        if (playerManager != null)
        {
            PlayerManager = playerManager.GetComponent<PlayerManager>();
        }
    }
    public void FindEnemyManager()
    {
        GameObject enemyManager = GameObject.Find("EnemyManager");
        if (enemyManager != null)
        {
            EnemyManager = enemyManager.GetComponent<EnemyManager>();
        }
    }
}
