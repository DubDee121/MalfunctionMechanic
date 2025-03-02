using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class EndManager : MonoBehaviour
{
    public Transform camer;
    public List<endScreen> endings;
    public AudioSource moveSounds;
    public List<AudioClip> randomMove;

    void Start()
    {
        camer.parent = endings[NightSettings.ending].hook;
        camer.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        endings[NightSettings.ending].screen.SetActive(true);
    }

    void FixedUpdate()
    {
        if (!moveSounds.isPlaying)
        {
            moveSounds.clip = randomMove[Random.Range(0, randomMove.Count)];
            moveSounds.Play();
        }
    }

    public void Return()
    {
        SceneManager.LoadScene("title");
    }
}

[Serializable]
public class endScreen
{
    public Transform hook;
    public GameObject screen;
}