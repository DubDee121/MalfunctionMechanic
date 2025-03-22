using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public RectTransform nightSelect;
    public RectTransform customNight;
    public RectTransform quit;
    public Transform camer;
    public AudioSource music;
    
    public List<Settings> presets;



    void Start()
    {
        nightSelect.anchoredPosition = new Vector2(-1000.0f, 0.0f);
        customNight.anchoredPosition = new Vector2(1000.0f, 0.0f);
        quit.anchoredPosition = new Vector2(0.0f, -200.0f);
        music.volume = 0.0f;
    }

    void Update()
    {
        nightSelect.anchoredPosition = Vector2.MoveTowards(nightSelect.anchoredPosition, Vector2.zero, 300.0f * Time.deltaTime);
        customNight.anchoredPosition = Vector2.MoveTowards(customNight.anchoredPosition, Vector2.zero, 300.0f * Time.deltaTime);
        quit.anchoredPosition = Vector2.MoveTowards(quit.anchoredPosition, Vector2.zero, 60.0f * Time.deltaTime);
    }

    void FixedUpdate()
    {
        camer.position = Vector3.Lerp(camer.position, new Vector3(0.0f, -0.5f, -6.0f), 0.01f);
        if (Vector3.Distance(camer.position, new Vector3(0.0f, -0.5f, -6.0f)) < 0.001f)
        {
            camer.position = new Vector3(0.0f, -0.5f, -6.0f);
        }

        if (music.volume < 0.4f)
        {
            music.volume += 0.002f;
        }
    }
    
    
    
    public void SetState0(Int32 state)
    {
        if (state == 0)
        {
            presets[0].statePool[0] = ActiveState.Inactive;
        }
        if (state == 1)
        {
            presets[0].statePool[0] = ActiveState.Active;
        }
        if (state == 2)
        {
            presets[0].statePool[0] = ActiveState.Fakeout;
        }
        if (state == 3)
        {
            presets[0].statePool[0] = ActiveState.Malfunction;
        }
    }
    
    public void SetState1(Int32 state)
    {
        if (state == 0)
        {
            presets[0].statePool[1] = ActiveState.Inactive;
        }
        if (state == 1)
        {
            presets[0].statePool[1] = ActiveState.Active;
        }
        if (state == 2)
        {
            presets[0].statePool[1] = ActiveState.Fakeout;
        }
        if (state == 3)
        {
            presets[0].statePool[1] = ActiveState.Malfunction;
        }
    }
    
    public void SetState2(Int32 state)
    {
        if (state == 0)
        {
            presets[0].statePool[2] = ActiveState.Inactive;
        }
        if (state == 1)
        {
            presets[0].statePool[2] = ActiveState.Active;
        }
        if (state == 2)
        {
            presets[0].statePool[2] = ActiveState.Fakeout;
        }
        if (state == 3)
        {
            presets[0].statePool[2] = ActiveState.Malfunction;
        }
    }
    
    public void SetState3(Int32 state)
    {
        if (state == 0)
        {
            presets[0].statePool[3] = ActiveState.Inactive;
        }
        if (state == 1)
        {
            presets[0].statePool[3] = ActiveState.Active;
        }
        if (state == 2)
        {
            presets[0].statePool[3] = ActiveState.Fakeout;
        }
        if (state == 3)
        {
            presets[0].statePool[3] = ActiveState.Malfunction;
        }
    }
    
    public void SetState4(Int32 state)
    {
        if (state == 0)
        {
            presets[0].statePool[4] = ActiveState.Inactive;
        }
        if (state == 1)
        {
            presets[0].statePool[4] = ActiveState.Active;
        }
        if (state == 2)
        {
            presets[0].statePool[4] = ActiveState.Fakeout;
        }
        if (state == 3)
        {
            presets[0].statePool[4] = ActiveState.Malfunction;
        }
    }
    
    public void SetState5(Int32 state)
    {
        if (state == 0)
        {
            presets[0].statePool[5] = ActiveState.Inactive;
        }
        if (state == 1)
        {
            presets[0].statePool[5] = ActiveState.Active;
        }
        if (state == 2)
        {
            presets[0].statePool[5] = ActiveState.Fakeout;
        }
        if (state == 3)
        {
            presets[0].statePool[5] = ActiveState.Malfunction;
        }
    }
    
    public void SetState6(Int32 state)
    {
        if (state == 0)
        {
            presets[0].statePool[6] = ActiveState.Inactive;
        }
        if (state == 1)
        {
            presets[0].statePool[6] = ActiveState.Active;
        }
        if (state == 2)
        {
            presets[0].statePool[6] = ActiveState.Fakeout;
        }
        if (state == 3)
        {
            presets[0].statePool[6] = ActiveState.Malfunction;
        }
    }



    public void SetAnimatronicDifficulty(Int32 difficulty)
    {
        if (difficulty == 0)
        {
            presets[0].moveMulti = 1.6f;
            presets[0].malfunctionsChanceMulti = 0.6f;
            presets[0].visualChanceMulti = 1.4f;
        }
        if (difficulty == 1)
        {
            presets[0].moveMulti = 1.0f;
            presets[0].malfunctionsChanceMulti = 1.0f;
            presets[0].visualChanceMulti = 1.0f;
        }
        if (difficulty == 2)
        {
            presets[0].moveMulti = 0.8f;
            presets[0].malfunctionsChanceMulti = 1.4f;
            presets[0].visualChanceMulti = 0.8f;
        }
        if (difficulty == 3)
        {
            presets[0].moveMulti = 0.4f;
            presets[0].malfunctionsChanceMulti = 1.8f;
            presets[0].visualChanceMulti = 0.6f;
        }
    }

    public void SetPowerDifficulty(Int32 difficulty)
    {
        if (difficulty == 0)
        {
            presets[0].powerDrainMulti = 1.8f;
            presets[0].powerRechargeMulti = 0.67f;
        }
        if (difficulty == 1)
        {
            presets[0].powerDrainMulti = 1.0f;
            presets[0].powerRechargeMulti = 1.0f;
        }
        if (difficulty == 2)
        {
            presets[0].powerDrainMulti = 0.8f;
            presets[0].powerRechargeMulti = 1.2f;
        }
        if (difficulty == 3)
        {
            presets[0].powerDrainMulti = 0.5f;
            presets[0].powerRechargeMulti = 2.0f;
        }
    }
    
    public void StartPreset(int night)
    {
        NightSettings.settings = presets[night];
        SceneManager.LoadScene("game");
    }

    public void Quit()
    {
        Application.Quit();
    }
}

[Serializable]
public class Settings
{
    public List<ActiveState> statePool;
    public float moveMulti;
    public float malfunctionsChanceMulti;
    public float visualChanceMulti;
    public float powerDrainMulti;
    public float powerRechargeMulti;
    public bool tutorial;
}

public class NightSettings
{
    public static Settings settings;
    public static int ending;
}