using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public enum CurrentScene
    {
        Nothing,
        MainMenu,
        Level1,
        GameOver
    }

    public CurrentScene scene = CurrentScene.Nothing;

    public string Scene = "";

    GameObject playerManager;

    // Start is called before the first frame update
    void Start()
    {
        switch (scene)
        {
            case CurrentScene.Level1:
                playerManager = GameObject.Find("PlayerManager");
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCameraFollow(GameObject target)
    {
        Transform CMcam = transform.Find("CMvcam1");

        CinemachineVirtualCamera cam = CMcam.GetComponent<CinemachineVirtualCamera>();

        cam.Follow = target.transform;
    }
}
