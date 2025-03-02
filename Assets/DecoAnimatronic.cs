using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecoAnimatronic : MonoBehaviour
{
    public Material mat;
    public AnimationClip endLoop;

    private Animation _anim;

    public void SetEmissive()
    {
        mat.EnableKeyword("_EMISSION");
    }

    public void ResetEmissive()
    {
        mat.DisableKeyword("_EMISSION");
    }

    public void SetEndLoop()
    {
        _anim = gameObject.GetComponent<Animation>();
        _anim.clip = endLoop;
        _anim.Play();
    }
}
