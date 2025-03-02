using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkbox : MonoBehaviour
{
    public List<Vector3> states;
    public int index;

    public void Interact()
    {
        index += 1;
        if (index == states.Count)
        {
            index = 0;
        }

        transform.localRotation = Quaternion.Euler(states[index]);
    }
}
