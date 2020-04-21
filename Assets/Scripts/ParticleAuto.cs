using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAuto : MonoBehaviour
{
    public bool Disable = false;

    private ParticleSystem ps;
    private float st;

    void OnEnable()
    {
        st = Time.time;
    }

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if ((Time.time - st) > ps.main.duration)
        {
            if (Disable)
                gameObject.SetActive(false);
            else
                Destroy(gameObject);
        }

    }
}
