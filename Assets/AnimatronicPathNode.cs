using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatronicPathNode : MonoBehaviour
{
    [Header("Node Features")]
    public List<AnimatronicPathNode> targets;
    public AnimationClip pose;
    public List<RoomLight> lights;
    public bool isAttack;
    public AnimationClip attackClip;
    public float minAttackAngle;
    public float maxAttackAngle;
    public bool isJumpscare;
    [Space(20.0f)] [Header("Door Features")]
    public doorButton door;
    [Space(20.0f)] [Header("Camera Features")]
    public List<Transform> visibleCameras;
    public AnimatronicPathNode fallbackTarget;
    public Transform activeCamera;

    private Player _player;



    void Start()
    {
        activeCamera = GameObject.Find("cams").transform;
        _player = GameObject.Find("Player").GetComponent<Player>();
    }

    public bool isVisible()
    {
        foreach (Transform test in visibleCameras)
        {
            if (test.position == activeCamera.position && _player.isInCams)
            {
                return true;
            }
        }
        return false;
    }
}
