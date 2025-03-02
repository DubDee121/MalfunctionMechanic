using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Animatronic : MonoBehaviour
{
    public Material mat;
    public ActiveState state;
    public Animation pose;
    public List<Behaviour> behaviours;
    public AnimatronicPathNode awakePos;
    public int id;
    [Space(10.0f)]
    public AnimatronicPathNode position;
    public int minMoveInterval;
    public int maxMoveInterval;
    [Space(10.0f)]
    public AnimatronicPathNode jumpscarePos;
    public int attackTime;
    public AnimationClip jumpscare;
    public Transform jumpscareHook;
    public AudioClip attackSound;
    public AudioClip jumpscareSound;
    [Space(20.0f)]
    public List<Transform> twitchJoints;
    [Space(20.0f)]
    public Animation decoVersion;
    public AnimationClip activate;

    private GameManager _manager;
    private Player _player;
    private SoundManager _soundManager;
    private int _counter;
    private int _attackCounter;
    private int _cameraShyCounter;
    private int _eventCounter;
    private bool _twitchEvent;
    private List<float> _twitchRotationOffsets;
    private int _surgeActiveTime;



    void Start()
    {
        mat.DisableKeyword("_EMISSION");
        _manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _player = GameObject.Find("Player").GetComponent<Player>();
        _soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        _counter = Mathf.RoundToInt(Random.Range(minMoveInterval, maxMoveInterval) * _manager.moveDelayMulti);
        _twitchRotationOffsets = new List<float>();
        _surgeActiveTime = Random.Range(18000, 21600);
    }

    void FixedUpdate()
    {
        if (_manager.gameStopped | _manager.paused)
        {
            return;
        }

        //jumpscare logic
        if (position.isJumpscare)
        {
            _counter -= 1;
            if (_counter == 0)
            {
                _player.playerCam.transform.SetParent(jumpscareHook);
                _player.playerCam.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                pose.clip = jumpscare;
                position.lights[0].SetFlicker(60);
                pose.Play();
                _soundManager.Sound2D(jumpscareSound);
                _manager.gameStopped = true;
                _manager.ui.SetActive(false);
            }
            return;
        }

        if (_manager.gameEnded | _manager.powerout)
        {
            return;
        }
        
        //attacking logic
        if (position.isAttack)
        {
            _counter -= 1;
            
            if (_counter < 1)
            {
                Move(position.targets[0]);
            }

            if (!(_player.isInCams | _player.isInClipboard) && _attackCounter < 0 && _player.angle >= position.minAttackAngle && _player.angle <= position.maxAttackAngle)
            {
                pose.clip = position.attackClip;
                pose.Play();
                _soundManager.Sound3D(attackSound, transform.position);
                _attackCounter = 0;
                _counter = attackTime;
                _soundManager.SoundStinger();
            }
            return;
        }



        //Camera Shy
        if (_cameraShyCounter > 0)
        {
            _cameraShyCounter -= 1;
            if (_cameraShyCounter == 0)
            {
                _cameraShyCounter = -1;
                Move(position.targets[Random.Range(0, position.targets.Count)]);
            }
            if (_cameraShyCounter == 15)
            {
                if (behaviours.Contains(Behaviour.Twitchy) && Random.Range(0, 100) < 25 * _manager.visualEventChanceMulti)
                {
                    TwitchEvent();
                }
            }
        }
        
        //Twitchy + Flickering
        if (_twitchEvent)
        {
            TwitchEvent();
        }
        if (_eventCounter < 1)
        {
            _eventCounter = 30;
            if (position.isVisible())
            {
                if (behaviours.Contains(Behaviour.Twitchy) && Random.Range(0, 100) < 5 * _manager.visualEventChanceMulti)
                {
                    TwitchEvent();
                }

                if (behaviours.Contains(Behaviour.Flickering) && Random.Range(0, 100) < 10 * _manager.visualEventChanceMulti)
                {
                    position.lights[Random.Range(0,position.lights.Count)].SetFlicker(15);
                }
            }
        }
        else
        {
            _eventCounter -= 1;
        }

        //Standard movement
        if (_counter < 1)
        {
            if (state != ActiveState.Inactive && position.targets.Count > 0)
            {
                Move(position.targets[Random.Range(0,position.targets.Count)]);
                mat.EnableKeyword("_EMISSION");
            }
        }
        else
        {
            _counter -= (behaviours.Contains(Behaviour.Surge) && _manager.time > _surgeActiveTime) ? 5 : 1;
            if (behaviours.Contains(Behaviour.Flex) && !position.isVisible())
            {
                _counter -= (behaviours.Contains(Behaviour.Surge) && _manager.time > _surgeActiveTime) ? 5 : 1;
            }
        }
    }



    public void Move(AnimatronicPathNode newPos)
    {
        if (newPos.isAttack)
        {
            if (_player.isInCams | _player.isInClipboard)
            {
                _counter = maxMoveInterval;
                _attackCounter = -1;
                if (position.isVisible())
                {
                    _player.PauseCams(50);
                }
                position = newPos;
                transform.position = newPos.transform.position;
                transform.rotation = newPos.transform.rotation;
                pose.clip = newPos.pose;
                pose.Play();
            }
            else
            {
                _counter = 10;
                _attackCounter += 10;
                if (_attackCounter > maxMoveInterval)
                {
                    position = newPos;
                    Move(newPos.targets[0]);
                }
            }
            return;
        }

        if (newPos.isJumpscare)
        {
            if (!position.door.isOpen)
            {
                _counter = 10;
                _attackCounter += 10;
                if (_attackCounter > 30)
                {
                    Move(awakePos);
                }
                return;
            }
            if (state != ActiveState.Malfunction)
            {
                Move(awakePos);
            }
            else
            {
                position = newPos;
                transform.position = newPos.transform.position;
                transform.rotation = newPos.transform.rotation;
                pose.clip = newPos.pose;
                pose.Play();
                _counter = Random.Range(150, 240);
            }
            return;
        }

        if (behaviours.Contains(Behaviour.Surge) && _manager.time <= _surgeActiveTime && Random.Range(0,100) < 50 * _manager.impatientChanceMulti)
        {
            return;
        }
        
        _counter = Mathf.RoundToInt(Random.Range(minMoveInterval, maxMoveInterval) * _manager.moveDelayMulti * (behaviours.Contains(Behaviour.Surge) && _manager.time <= _surgeActiveTime ? 5 : 1));
        if (position.door != null)
        {
            if (!position.door.isOpen)
            {
                if (behaviours.Contains(Behaviour.Impatient) && Random.Range(0,100) < 50 * _manager.impatientChanceMulti && !position.isAttack)
                {
                    position.door.Interact();
                }
                else
                {
                    newPos = behaviours.Contains(Behaviour.Persistent) && !position.isAttack ? position : awakePos;
                }
            }
        }
        
        if (position.isVisible() | newPos.isVisible())
        {
            _player.PauseCams(30);
        }
        transform.position = newPos.transform.position;
        transform.rotation = newPos.transform.rotation;
        position = newPos;
        pose.clip = newPos.pose;
        pose.Play();
        _soundManager.Sound3D(_manager.moveSounds[Random.Range(0, _manager.moveSounds.Count)], transform.position);
        UpdateCam();
    }

    public void UpdateCam()
    {
        if (behaviours.Contains(Behaviour.CameraShy) && position.isVisible())
        {
            _cameraShyCounter = Random.Range(50, 75);
            if (behaviours.Contains(Behaviour.Flickering) && Random.Range(0,100) < 50 * _manager.visualEventChanceMulti)
            {
                position.lights[Random.Range(0, position.lights.Count)].SetFlicker(_cameraShyCounter);
            }
        }
    }

    public void TwitchEvent()
    {
        if (!_twitchEvent)
        {
            _twitchEvent = true;
            foreach (Transform joint in twitchJoints)
            {
                _twitchRotationOffsets.Add(Random.Range(-10.0f, 10.0f));
                joint.Rotate(joint.forward, _twitchRotationOffsets[_twitchRotationOffsets.Count - 1]);
            }
        }
        else
        {
            _twitchEvent = false;
            foreach (Transform joint in twitchJoints)
            {
                joint.Rotate(joint.forward, -_twitchRotationOffsets[0]);
                _twitchRotationOffsets.RemoveAt(0);
            }
        }
    }

    public void Death()
    {
        Debug.Log("you are deceased. how unfortunate.");
        _manager.blackScreen.color = Color.black;
        NightSettings.ending = id;
        SceneManager.LoadScene("end");
    }

    public void SetEmissive(bool thing)
    {
        if (thing)
        {
            mat.EnableKeyword("_EMISSION");
        }
        else
        {
            mat.DisableKeyword("_EMISSION");
        }
    }
}
