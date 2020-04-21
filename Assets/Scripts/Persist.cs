using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Persist : MonoBehaviour
{
    // Singleton Instance
    public static Persist Instance { get; private set; }

    public bool restart = false;
    public bool menu = true;

    public float MouseSensitivity = 75.0f;

    public GameObject Menu;
    public MouseLook MSS;
    public Slider MS;

    public AudioSource sound;

    public bool SS;

    void Awake()
    {
        // Singleton Game Manager
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        SS = false;
    }

    void Update()
    {
        if (MS != null && MSS != null && SS)
        {
            MS.value = MouseSensitivity;
            MSS.mouseSensitivity = MouseSensitivity;
            SS = false;
        }
        if (Menu != null)
        {
            if (!restart && menu)
            {
                Menu.SetActive(true);
                if (!sound.isPlaying)
                    sound.Play();
            }
            else
            {
                Menu.SetActive(false);
                sound.Stop();
            }
            if (restart)
                restart = false;
        }
        else
        {
            sound.Stop();
        }
    }
}
