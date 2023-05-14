using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    GameObject playerManager;

    // Start is called before the first frame update
    void Start()
    {
        playerManager = GameObject.Find("PlayerManager");
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
