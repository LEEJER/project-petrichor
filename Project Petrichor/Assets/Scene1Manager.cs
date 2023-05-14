using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene1Manager : MonoBehaviour
{
    public GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        GameObject newPlayer = Instantiate(gameManager.PlayerManager.playerPrefab, gameManager.PlayerManager.transform);
        newPlayer.transform.position = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
