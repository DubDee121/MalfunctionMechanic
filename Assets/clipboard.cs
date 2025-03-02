using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class clipboard : MonoBehaviour
{
    public Player player;
    public SoundManager soundManager;
    public GameObject selectionPage;
    public GameObject infoPage;

    public void Interact()
    {
        selectionPage.SetActive(!selectionPage.activeSelf);
        infoPage.SetActive(!infoPage.activeSelf);
        soundManager.Sound3D(player.clipChange, transform.position);
    }
}
