using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TransformState
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
}

public class doorButton : MonoBehaviour
{
    public Transform door;
    public MeshRenderer visual;
    public bool isOffice;
    public bool isConnected;
    public doorButton connectedDoor;
    public TransformState openState;
    public TransformState closedState;
    public bool isOpen;
    public int powerUseDelay;

    private TransformState _targetState;
    private GameManager _manager;
    private SoundManager _soundManager;
    private int _counter;



    void Start()
    {
        _manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        _targetState = openState;
        isOpen = true;
    }

    void Update()
    {
        door.position = Vector3.MoveTowards(door.position, _targetState.position, 15.0f * Time.deltaTime);
        door.localScale = Vector3.MoveTowards(door.localScale, _targetState.scale, 5.0f * Time.deltaTime);
    }

    void FixedUpdate()
    {
        if (_manager.gameStopped | _manager.paused)
        {
            return;
        }

        if (_manager.powerout)
        {
            if (!isOpen)
            {
                Interact();
            }
            return;
        }
        
        if (isConnected)
        {
            return;
        }
        
        if (isOpen)
        {
            _counter = 0;
        }
        else
        {
            _counter -= 1;
            if (_counter < 1)
            {
                _manager.UpdatePower(-1.0f);
                _counter = Mathf.RoundToInt(powerUseDelay * _manager.drainDelayMulti);
            }
        }
    }

    public void Interact()
    {
        if (_manager.powerout)
        {
            return;
        }
        
        if (!isConnected)
        {
            if (connectedDoor != null)
            {
                connectedDoor.ToggleDoor();
            }
            if (isOpen)
            {
                _targetState = closedState;
                _soundManager.Sound3D(_manager.doorClose, door.position);
                if (!isOffice)
                {
                    visual.material = _manager.buttonActive;
                }
                isOpen = false;
            }
            else
            {
                _targetState = openState;
                _soundManager.Sound3D(_manager.doorOpen, door.position);
                if (!isOffice)
                {
                    visual.material = _manager.buttonInactive;
                }
                isOpen = true;
            }
        }
    }

    public void ToggleDoor()
    {
        if (isOpen)
        {
            _targetState = closedState;
            _soundManager.Sound3D(_manager.doorClose, door.position);
            if (!isOffice)
            {
                visual.material = _manager.buttonInactive;
            }
            isOpen = false;
        }
        else
        {
            _targetState = openState;
            _soundManager.Sound3D(_manager.doorOpen, door.position);
            if (!isOffice)
            {
                visual.material = _manager.buttonActive;
            }
            isOpen = true;
        }
    }
}
