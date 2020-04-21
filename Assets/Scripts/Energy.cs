using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Energy : MonoBehaviour
{
    public GameObject Vfx;
    public float AngularVelocity = 10.0f;
    public float Increment = 50.0f;
    public AudioSource audio;

    void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            GameManager.Instance.Stamina(Increment);
            Instantiate(Vfx, transform.position, Quaternion.identity);
            audio.Play();
            StartCoroutine(SelfDestruct());
        }
    }

    IEnumerator SelfDestruct()
    {
        if (audio.isPlaying)
            yield return null;
        Destroy(gameObject);
    }

    void FixedUpdate()
    {
        transform.Rotate(0.0f, 6.0f * Increment * Time.fixedDeltaTime, 0.0f);
    }
}
