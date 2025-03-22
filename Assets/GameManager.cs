using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Initialisation")]
    public List<ActiveState> statePool;
    public List<Animatronic> animatronics;
    public List<Behaviour> malfunctionBehaviours;
    public TMP_Text powerText;
    public TMP_Text timeText;
    public AnimatronicPathNode preJumpscare;
    public GameObject ui;
    public GameObject pauseUi;
    public AudioClip endJingle;
    public AudioClip stinger;
    public Image blackScreen;
    [Space(20.0f)] [Header("Gameplay")]
    public int time;
    public int maxTime;
    public List<string> timeMarkers;
    public float power;
    public float maxPower;
    public int powerRechargeDelay;
    public List<RoomLight> officeLights;
    public List<AudioClip> moveSounds;
    public AudioClip doorOpen;
    public AudioClip doorClose;
    public AudioClip poweroutSound;
    public Material buttonInactive;
    public Material buttonActive;
    public GameObject tutorialStart;
    public GameObject tutorialCams;
    public GameObject tutorialClip;
    [Space(20.0f)] [Header("Difficulty modifiers")]
    public float moveDelayMulti;
    public float impatientChanceMulti;
    public float visualEventChanceMulti;
    public float drainDelayMulti;
    public float rechargeDelayMulti;
    [Space(20.0f)] [Header("Ending Sequence")]
    public List<Animation> decoAnimatronics;
    public int jumpscareDelay;
    public Transform endCamHook;
    public RoomLight endLight;
    [Space(20.0f)] [Header("DO NOT TOUCH")]
    public List<Animatronic> malfunctions;
    public List<Animatronic> playerGuess;
    public List<selector> selectors;
    public bool gameStopped;
    public bool gameEnded;
    public bool powerout;
    public bool paused;

    private Player _player;
    private SoundManager _soundManager;
    private int _index;
    private int _behaviourCounter;
    private Behaviour _behaviour;
    private List<Behaviour> _mBehaviours;
    private int _powerCounter;
    private int _jumpscareCounter;
    private int _endCounter;



    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        _mBehaviours = new List<Behaviour>();
        
        statePool.Clear();
        statePool.AddRange(NightSettings.settings.statePool);
        moveDelayMulti = NightSettings.settings.moveMulti;
        impatientChanceMulti = NightSettings.settings.malfunctionsChanceMulti;
        visualEventChanceMulti = NightSettings.settings.visualChanceMulti;
        drainDelayMulti = NightSettings.settings.powerDrainMulti;
        rechargeDelayMulti = NightSettings.settings.powerRechargeMulti;

        if (statePool.Count < animatronics.Count)
        {
            while (statePool.Count < animatronics.Count)
            {
                statePool.Add(ActiveState.Inactive);
            }
        }
        foreach (Animatronic target in animatronics)
        {
            //reset logic
            _behaviourCounter = 0;
            _mBehaviours.Clear();
            _mBehaviours.AddRange(malfunctionBehaviours);
            
            //apply a random state from the pool
            _index = Random.Range(0, statePool.Count);
            target.state = statePool[_index];
            
            //test state for further logic
            if (statePool[_index] == ActiveState.Malfunction)
            {
                malfunctions.Add(target);
                _behaviourCounter = 3;
            }
            if (statePool[_index] == ActiveState.Fakeout)
            {
                _behaviourCounter = Random.Range(1, 3);
            }

            //apply malfunction behaviours to Malfunctions and Fakeouts
            while (_behaviourCounter > 0)
            {
                _behaviourCounter -= 1;
                
                _behaviour = _mBehaviours[Random.Range(0, _mBehaviours.Count)];
                if (_behaviour == Behaviour.CameraShy)
                {
                    _mBehaviours.Remove(Behaviour.Surge);
                }
                if (_behaviour == Behaviour.Surge)
                {
                    _mBehaviours.Remove(Behaviour.CameraShy);
                }

                _mBehaviours.Remove(_behaviour);
                target.behaviours.Add(_behaviour);
            }
            
            statePool.RemoveAt(_index);
        }

        if (NightSettings.settings.tutorial)
        {
            Pause();
            tutorialStart.SetActive(true);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused)
            {
                Resume();
            }
            else
            {
                Pause();
                pauseUi.SetActive(true);
            }
        }
    }
    
    void FixedUpdate()
    {
        if (gameStopped | paused)
        {
            return;
        }

        if (gameEnded)
        {
            _jumpscareCounter -= 1;
            if (_jumpscareCounter == 0)
            {
                foreach (Animatronic guess in playerGuess)
                {
                    if (malfunctions.Contains(guess))
                    {
                        malfunctions.Remove(guess);
                    }
                }

                if (malfunctions.Count > 0)
                {
                    _index = Random.Range(0, malfunctions.Count);
                    malfunctions[_index].position = preJumpscare;
                    malfunctions[_index].Move(malfunctions[_index].jumpscarePos);
                }
            }

            _endCounter -= 1;
            if (_endCounter == 0)
            {
                Debug.Log("game win");
                NightSettings.ending = 0;
                SceneManager.LoadScene("end");
            }
            return;
        }

        if (powerout)
        {
            _jumpscareCounter -= 1;
            if (_jumpscareCounter == 0)
            {
                _index = Random.Range(0, malfunctions.Count);
                malfunctions[_index].position = preJumpscare;
                malfunctions[_index].Move(malfunctions[_index].jumpscarePos);
                malfunctions[_index].decoVersion.transform.SetPositionAndRotation(new Vector3(0.0f, -1.5f, 3.5f), Quaternion.Euler(0.0f, 180.0f, 0.0f));
                malfunctions[_index].decoVersion.clip = malfunctions[_index].activate;
                malfunctions[_index].decoVersion.Play();
            }
            return;
        }
        
        time += 1;
        timeText.text = timeMarkers[Mathf.FloorToInt((time - 1.0f) / maxTime * timeMarkers.Count)];
        blackScreen.color = new Color (0.0f, 0.0f, 0.0f, Mathf.Clamp(0.02f * (time - maxTime + 60.0f), 0.0f, 1.0f));
        if (time >= maxTime)
        {
            BeginEnding();
        }

        _powerCounter -= 1;
        if (_powerCounter < 1)
        {
            UpdatePower(1.0f);
        }
    }



    void BeginEnding()
    {
        gameEnded = true;
        _jumpscareCounter = jumpscareDelay;
        _endCounter = jumpscareDelay + 300;
        foreach (Animatronic thing in animatronics)
        {
            thing.mat.DisableKeyword("_EMISSION");
            if (playerGuess.Contains(thing))
            {
                decoAnimatronics.Remove(thing.decoVersion);
            }
            else
            {
                thing.decoVersion.clip = thing.activate;
            }
        }
        foreach (Animation thing in decoAnimatronics)
        {
            thing.Play();
        }
        _player.playerCam.transform.SetParent(endCamHook);
        _player.playerCam.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        endLight.SetFlicker(60);
        _soundManager.Sound2D(endJingle);
        ui.SetActive(false);
    }

    void Powerout()
    {
        powerout = true;
        _jumpscareCounter = Random.Range(90, 150);
        foreach (RoomLight light in officeLights)
        {
            light.lightIntensity = 0.1f;
            light.shine.color = new Color(0.0f, 1.0f, 1.0f);
        }
        ui.SetActive(false);
        _soundManager.Sound2D(poweroutSound);
    }
    
    public void UpdatePower(float amount)
    {
        power = Mathf.Clamp(power + amount, 0.0f, maxPower);
        _powerCounter = Mathf.RoundToInt(powerRechargeDelay * rechargeDelayMulti);
        powerText.text = power * 100.0f / maxPower + "%";
        if (power <= 0.0f)
        {
            Powerout();
        }
    }

    public void Pause()
    {
        paused = true;
    }

    public void Resume()
    {
        paused = false;
        pauseUi.SetActive(false);
        tutorialStart.SetActive(false);
        tutorialCams.SetActive(false);
        tutorialClip.SetActive(false);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("title");
    }
}

public enum ActiveState
{
    Inactive,
    Active,
    Fakeout,
    Malfunction
}

public enum Behaviour
{
    CameraShy,
    Flickering,
    Impatient,
    Surge,
    Twitchy,
    
    Persistent,
    Flex
}