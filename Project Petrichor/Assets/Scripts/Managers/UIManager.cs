using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public enum CurrentScene
    {
        Nothing,
        MainMenu,
        Level1,
        GameOver
    }

    public CurrentScene scene = CurrentScene.Nothing;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.FindUIManager();
        switch(scene)
        {
            case CurrentScene.GameOver:
                GameManager.instance.SetGameOverScore();
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
