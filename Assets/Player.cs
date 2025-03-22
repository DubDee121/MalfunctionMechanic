using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player")]
    public Camera playerCam;
    public float minAngle;
    public float maxAngle;
    public float angle;
    public float rotationSpeed;
    public CursorLockMode lockMode;
    [Space(20.0f)] [Header("Cameras")]
    public List<cameraButton> activeCameras;
    public Transform camScreen;
    public Transform camObject;
    public Material camStatic;
    public Transform camStaticObject;
    public float targetAngle;
    public float targetOffset;
    public bool isInCams;
    public AudioSource cameraSounds;
    public AudioClip camOpen;
    public AudioClip camClose;
    public AudioClip camChange;
    [Space(20.0f)] [Header("Clipboard")]
    public Transform clipboardObject;
    public float clipboardAngle;
    public float clipboardOffset;
    public bool isInClipboard;
    public AudioClip clipOpen;
    public AudioClip clipClose;
    public AudioClip clipChange;

    private GameManager _manager;
    private SoundManager _soundManager;
    private bool _camToggleable;
    private float _initCamAngle;
    private float _initOffset;
    private float _initClipboardAngle;
    private float _initClipboardOffset;
    private int _camStaticCounter;
    private bool _usedCams;
    private bool _usedClip;



    void Start()
    {
        Cursor.lockState = lockMode;
        _manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        _initCamAngle = targetAngle;
        _initOffset = targetOffset;
        _initClipboardAngle = clipboardAngle;
        _initClipboardOffset = clipboardOffset;
    }

    void Update()
    {
        if (_manager.paused)
        {
            return;
        }
        
        if (Input.mousePosition.x < Screen.width / 10.0f && !isInCams && !isInClipboard)
        {
            angle = Mathf.Clamp(angle - rotationSpeed * Time.deltaTime, minAngle, maxAngle);
        }
        if (Input.mousePosition.x > Screen.width * 9.0f / 10.0f && !isInCams && !isInClipboard)
        {
            angle = Mathf.Clamp(angle + rotationSpeed * Time.deltaTime, minAngle, maxAngle);
        }
        transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);

        if (Input.GetMouseButtonDown(0))
        {
            Physics.Raycast(playerCam.ScreenPointToRay(Input.mousePosition), out RaycastHit hitinfo);
            if (hitinfo.collider != null)
            {
                hitinfo.collider.gameObject.SendMessage("Interact");
            }
        }
    }
    
    void FixedUpdate()
    {
        if (_manager.gameStopped)
        {
            return;
        }
        
        camScreen.localRotation = Quaternion.RotateTowards(camScreen.localRotation, Quaternion.Euler(targetAngle, 0.0f, 0.0f), 900.0f * Time.deltaTime);
        camScreen.localPosition = Vector3.MoveTowards(camScreen.localPosition, new Vector3(0.0f, targetOffset, 1.0f), 9.0f * Time.deltaTime);
        clipboardObject.localRotation = Quaternion.RotateTowards(clipboardObject.localRotation, Quaternion.Euler(clipboardAngle, 0.0f, 0.0f), 900.0f * Time.deltaTime);
        clipboardObject.localPosition = Vector3.MoveTowards(clipboardObject.localPosition, new Vector3(0.0f, clipboardOffset, 1.0f), 9.0f * Time.deltaTime);

        if (_manager.paused)
        {
            return;
        }
        
        if (_manager.powerout)
        {
            if (isInCams)
            {
                ToggleCams();
            }
            if (isInClipboard)
            {
                ToggleClipboard();
            }
            return;
        }
        
        if (Input.mousePosition.y < Screen.height / 10.0f && _camToggleable)
        {
            _camToggleable = false;
            if (Input.mousePosition.x < Screen.width / 2.0f)
            {
                ToggleCams();
            }
            else
            {
                ToggleClipboard();
            }
        }
        if (Input.mousePosition.y > Screen.height / 10.0f)
        {
            _camToggleable = true;
        }

        camStaticObject.localRotation = Quaternion.Euler(new Vector3(0.0f, camStaticObject.localRotation.eulerAngles.y + 180.0f, 0.0f));
        if (_camStaticCounter == 0)
        {
            camStatic.color = new Color(1.0f, 1.0f, 1.0f, 0.05f);
            cameraSounds.Stop();
        }
        else
        {
            _camStaticCounter -= 1;
        }
    }



    public void ToggleCams()
    {
        if (isInClipboard)
        {
            ToggleClipboard();
        }
        
        if (isInCams)
        {
            targetAngle = _initCamAngle;
            targetOffset = _initOffset;
            cameraSounds.Stop();
            _soundManager.Sound3D(camClose, camObject.position);
            isInCams = false;
        }
        else
        {
            if (_manager.powerout)
            {
                return;
            }
            
            targetAngle = 0.0f;
            targetOffset = -0.5f;
            _soundManager.Sound3D(camOpen, camObject.position);
            isInCams = true;
            SetCam(camObject);
            if (NightSettings.settings.tutorial && !_usedCams)
            {
                _usedCams = true;
                _manager.tutorialCams.SetActive(true);
                _manager.Pause();
            }
        }
    }

    public void ToggleClipboard()
    {
        if (isInCams)
        {
            ToggleCams();
        }

        if (isInClipboard)
        {
            clipboardAngle = _initClipboardAngle;
            clipboardOffset = _initClipboardOffset;
            _soundManager.Sound3D(clipClose, clipboardObject.position);
            isInClipboard = false;
        }
        else
        {
            if (_manager.powerout)
            {
                return;
            }
            
            clipboardAngle = 0.0f;
            clipboardOffset = -0.5f;
            _soundManager.Sound3D(clipOpen, clipboardObject.position);
            isInClipboard = true;
            if (NightSettings.settings.tutorial && !_usedClip)
            {
                _usedClip = true;
                _manager.tutorialClip.SetActive(true);
                _manager.Pause();
            }
        }
    }

    public void SetCam(Transform pos)
    {
        camObject.transform.position = pos.position;
        camObject.transform.rotation = pos.rotation;
        foreach (Animatronic bot in _manager.animatronics)
        {
            bot.UpdateCam();
        }
        PauseCams(20);
    }

    public void PauseCams(int ticks)
    {
        camStatic.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        cameraSounds.Play();
        _camStaticCounter = ticks;
    }
}
