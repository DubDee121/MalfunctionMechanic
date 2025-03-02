using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class selector : MonoBehaviour
{
    public List<Vector3> states;
    public int index;
    public bool selected;
    public Animatronic guess;

    private Vector3 _initPos;
    private GameManager _manager;



    void Start()
    {
        _initPos = transform.localPosition;
        _manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _manager.selectors.Add(this);
    }

    public void Interact()
    {
        index += 1;
        if (index == states.Count)
        {
            index = 0;
        }

        transform.localPosition = _initPos + states[index];
        selected = !selected;

        if (selected)
        {
            _manager.playerGuess.Add(guess);
            if (_manager.playerGuess.Count > _manager.malfunctions.Count)
            {
                foreach (selector button in _manager.selectors)
                {
                    button.Deselect(_manager.playerGuess[0]);
                }
                _manager.playerGuess.RemoveAt(0);
            }
        }
        else
        {
            if (_manager.playerGuess.Contains(guess))
            {
                _manager.playerGuess.Remove(guess);
            }
        }
    }

    public void Deselect(Animatronic target)
    {
        if (target == guess && selected)
        {
            index = 0;
            transform.localPosition = _initPos + states[0];
            selected = false;
        }
    }
}
