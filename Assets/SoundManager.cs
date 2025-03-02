using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public GameObject sound;
    public List<AudioClip> stingerSounds;
    public int stingerCullDelay;
    public int stingerIncreaseDelay;

    private AudioSource _sound;
    private int _stingerIndex;
    private int _stingerCounter;



    void FixedUpdate()
    {
        if (_stingerCounter < stingerIncreaseDelay)
        {
            _stingerCounter += 1;
        }
    }
    
    public void Sound2D(AudioClip clip)
    {
        _sound = Instantiate(sound, Vector3.zero, Quaternion.identity).GetComponent<AudioSource>();
        _sound.clip = clip;
        _sound.Play();
        _stingerCounter = stingerIncreaseDelay;
    }

    public void Sound3D(AudioClip clip, Vector3 position)
    {
        _sound = Instantiate(sound, position, Quaternion.identity).GetComponent<AudioSource>();
        _sound.spatialBlend = 1.0f;
        _sound.clip = clip;
        _sound.Play();
    }

    public void SoundStinger()
    {
        if (_stingerCounter < stingerCullDelay)
        {
            return;
        }

        if (_stingerCounter < stingerIncreaseDelay)
        {
            _stingerIndex += 1;
            if (_stingerIndex == stingerSounds.Count)
            {
                _stingerIndex = 0;
            }
        }
        else
        {
            _stingerIndex = 0;
        }
        Sound2D(stingerSounds[_stingerIndex]);
        _stingerCounter = 0;
    }
}
