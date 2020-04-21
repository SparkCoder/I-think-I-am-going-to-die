using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Wall_Door : MonoBehaviour
{
    [Header("Parameters")]
    public bool Entrance = false;
    public bool Opened = false;

    [Header("Parts")]
    public GameObject Closed;
    public GameObject Open;

    void Update()
    {
        if (Closed != null || Open != null)
        {
            Closed.SetActive(!Opened);
            Open.SetActive(Opened);
        }
    }
}
