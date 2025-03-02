using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraButton : MonoBehaviour
{
    public Player player;
    public MeshRenderer visuals;
    public Transform target;

    private GameManager _manager;



    public void Start()
    {
        _manager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    
    public void Interact()
    {
        player.SetCam(target);
        foreach (cameraButton cam in player.activeCameras)
        {
            cam.Unset();
        }
        visuals.material = _manager.buttonActive;
    }

    public void Unset()
    {
        visuals.material = _manager.buttonInactive;
    }
}
