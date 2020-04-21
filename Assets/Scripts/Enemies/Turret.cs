using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Enemy
{
    [Header("Parameters")]
    public float Damage = 5.0f;
    public float Range = 15.0f;
    [Range(0, 1)]
    public float BaseSpeed = 0.8f;
    [Range(0, 1)]
    public float BarrelSpeed = 0.8f;

    public float BarrelMinAngle = -90.0f;
    public float BarrelMaxAngle = 90.0f;

    [Header("Components")]
    public Transform Base;
    public Transform Barrel;
    public Transform BarrelTip;
    public GameObject Flash;
    public AudioClip ShootAudio;

    private float x_angle, y_angle;
    private bool shootable;
    private AudioSource audio;

    void Start()
    {
        x_angle = 0.0f;
        y_angle = 0.0f;

        shootable = false;
        Active = false;

        Target = GameObject.FindGameObjectWithTag("Player").transform;
        audio = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        if (Active && !GameManager.Instance.dead)
        {
            // Aim
            Vector3 d = Target.position - Barrel.position;

            if (d.magnitude <= Range)
            {
                x_angle = Mathf.Rad2Deg * Mathf.Atan2(d.z, d.x);
                Quaternion baseR = Quaternion.AngleAxis(x_angle, -Vector3.up);
                shootable = Mathf.Abs(x_angle + ((Base.rotation.eulerAngles.y < 180.0f) ? Base.rotation.eulerAngles.y : (Base.rotation.eulerAngles.y - 360.0f))) < 5.0f;
                Base.rotation = Quaternion.Slerp(Base.rotation, baseR, BaseSpeed);

                y_angle = Mathf.Rad2Deg * Mathf.Atan2(d.magnitude, d.y);
                y_angle = Mathf.Clamp(y_angle, BarrelMinAngle, BarrelMaxAngle);
                Barrel.localRotation = Quaternion.Slerp(Barrel.localRotation, Quaternion.AngleAxis(-y_angle, Vector3.forward), BarrelSpeed);
                // Shoot
                if (shootable)
                {
                    if (!audio.isPlaying)
                    {
                        RaycastHit shot;
                        if (Physics.Raycast(Barrel.position, (BarrelTip.position - Barrel.position).normalized, out shot, Range))
                        {
                            if (shot.collider.name == "Player")
                                GameManager.Instance.Health(-Damage);
                        }
                        audio.PlayOneShot(ShootAudio, 1.0f);
                        Instantiate(Flash, BarrelTip);
                    }
                }
            }
        }
    }
}
