using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomLight : MonoBehaviour
{
    public Light shine;
    public float lightIntensity;
    public bool titleLight;
    
    private int _flickerCounter;



    void Start()
    {
        lightIntensity = shine.intensity;
        if (titleLight)
        {
            SetFlicker(200);
        }
    }
    
    void FixedUpdate()
    {
        if (_flickerCounter > 0)
        {
            shine.intensity = lightIntensity * Random.Range(0.0f, 0.8f);
            _flickerCounter -= 1;
        }
        else
        {
            shine.intensity = lightIntensity;
        }
    }

    public void SetFlicker(int ticks)
    {
        _flickerCounter = ticks;
    }
}
