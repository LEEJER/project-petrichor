using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject       UIManager;
    public PlayerManager    PlayerManager;
    public EnemyManager     EnemyManager;
    public GameObject       Environment;

    private Transform _healthBarObject;
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
        

        UIManager = GameObject.Find("UIManager");
        if (UIManager != null)
        {
            _canvas = UIManager.transform.Find("Canvas");
            _healthBarObject = _canvas.transform.Find("HealthBar");
            if (_healthBarObject != null)
            {
                _healthBar = _healthBarObject.transform.Find("Bar");
            }
        }

        GameObject playerManager = GameObject.Find("PlayerManager");
        if (playerManager != null)
        {
            PlayerManager = playerManager.GetComponent<PlayerManager>();
        }
        GameObject enemyManager = GameObject.Find("EnemyManager");
        if (enemyManager != null)
        {
            EnemyManager = enemyManager.GetComponent<EnemyManager>();
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

    private void OnEnable()
    {
        
    }

    public void SavePlayerScore()
    {
        _playerKills = PlayerManager.EnemiesKilled;
        _playerTime = PlayerManager.TimeAlive;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

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
        if (_healthBar != null)
        {
            _healthBar.GetComponent<Image>().rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 200f * val);
        }
    }
}
