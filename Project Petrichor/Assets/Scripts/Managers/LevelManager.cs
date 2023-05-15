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

    public AudioSource audioSource;
    public AudioClip[] Music = new AudioClip[3];
    private float waitTimeUnit = 30f;
    private float timeWaited = 0f;
    private float waitTimeGoal = 0f;
    private int currentlyPlayingIndex = 0;

    GameObject playerManager;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = transform.GetComponent<AudioSource>();
        audioSource.spatialize = false;
        audioSource.volume = 0.5f;

        currentlyPlayingIndex = Random.Range(0, Music.Length);
        audioSource.clip = Music[currentlyPlayingIndex];
        waitTimeGoal = waitTimeUnit * Random.Range(1f, 3.5f);
        audioSource.Play();

        

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
    void FixedUpdate()
    {
        if (!audioSource.isPlaying)
        {
            timeWaited += Time.fixedDeltaTime;
        }

        if (timeWaited >= waitTimeGoal)
        {
            timeWaited = 0;
            currentlyPlayingIndex = (currentlyPlayingIndex + 1) % Music.Length;
            audioSource.clip = Music[currentlyPlayingIndex];
            waitTimeGoal = waitTimeUnit * Random.Range(1f, 3.5f);
        }
    }

    public void SetCameraFollow(GameObject target)
    {
        Transform CMcam = transform.Find("CMvcam1");

        CinemachineVirtualCamera cam = CMcam.GetComponent<CinemachineVirtualCamera>();

        cam.Follow = target.transform;
    }
}
