using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundObject : MonoBehaviour
{
    public AudioSource sound;

    void FixedUpdate()
    {
        if (!sound.isPlaying)
        {
            Destroy(gameObject);
        }
    }
}
