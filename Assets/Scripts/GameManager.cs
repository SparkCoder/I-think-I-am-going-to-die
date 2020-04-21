using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton Instance
    public static GameManager Instance { get; private set; }

    public GameObject Player;
    public GameObject HUD;
    public GameObject MenuUI;
    public GameObject DeadUI;
    public GameObject ExplosionVfx;
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI FinalScoreText;
    public AudioSource sound;
    public AudioSource boom;
    public Slider VelocityBar;
    public Slider StaminaBar;
    public Slider HealthBar;
    public Slider MS;
    public MouseLook MSS;
    public Animator DamageUI;
    public Animator CameraAnim;
    public Volume volume;
    public bool dead;

    private float score;
    private float stamina;
    private float health;
    private float t1, t2, t3;

    private Vector3 pos;

    private Vignette vignette;

    void Awake()
    {
        // Singleton Game Manager
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(this);
    }

    void Start()
    {
        t1 = Time.time;
        t2 = Time.time;
        t3 = Time.time;
        dead = Persist.Instance.menu;
        score = 0.0f;
        stamina = 100.0f;
        health = 100.0f;
        pos = Player.transform.position;

        HUD.SetActive(!dead);
        if (!dead)
            sound.Play();

        Persist.Instance.Menu = MenuUI;
        Persist.Instance.MS = MS;
        Persist.Instance.MSS = MSS;
    }

    void Update()
    {
        if (!dead)
        {
            if (Input.GetButtonDown("Cancel"))
                health = 0.0f;

            ScoreText.text = "Score: " + score.ToString();

            if (volume.profile.TryGet<Vignette>(out vignette))
            {
                vignette.intensity.value = (((100 - health) / 100) * 0.5f) + 0.1f;
            }

            PlayerMovement pm = Player.GetComponent<PlayerMovement>();
            float v = Mathf.Clamp((pos - Player.transform.position).magnitude * pm.speed, 0, pm.speed);
            VelocityBar.value = v / pm.speed;

            StaminaBar.value = stamina / 100.0f;

            HealthBar.value = health / 100.0f;

            if (health < 100.0f && (Time.time - t2) > 2.0f)
            {
                Health(5.0f);
                t2 = Time.time;
            }

            if (VelocityBar.value < 0.3f && (Time.time - t1) > 1.0f)
            {
                Health(-5.0f);
                t1 = Time.time;
            }

            if ((Time.time - t3) > 0.5f)
            {
                stamina -= VelocityBar.value;
                t3 = Time.time;
            }

            if (health <= 0.0f)
            {
                Player.GetComponent<PlayerInput>().enabled = false;
                dead = true;
                FinalScoreText.text = "Final Score: " + score;
                DeadUI.SetActive(true);
                Instantiate(ExplosionVfx, Player.transform);
                sound.Stop();
                boom.Play();
            }

            pos = Player.transform.position;
        }
    }

    public void StartGame()
    {
        Persist.Instance.menu = false;
        dead = false;
        HUD.SetActive(true);
        if (health <= 0.0f)
            Restart();
        else
            sound.Play();
    }

    public void StopGame()
    {
        DeadUI.SetActive(false);
        Persist.Instance.menu = true;
        dead = true;
        HUD.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void Restart()
    {
        t1 = Time.time;
        t2 = Time.time;
        t3 = Time.time;
        dead = false;
        score = 0.0f;
        stamina = 100.0f;
        health = 100.0f;

        Persist.Instance.MouseSensitivity = MSS.mouseSensitivity;
        Persist.Instance.SS = true;

        ScoreText.text = "Score: " + score.ToString();

        if (volume.profile.TryGet<Vignette>(out vignette))
        {
            vignette.intensity.value = 0.0f;
        }

        Persist.Instance.menu = false;
        Persist.Instance.restart = true;

        SceneManager.LoadScene("Bombs Up");
    }

    public void Score(float ds)
    {
        if (!dead)
        {
            score += ds;
        }
    }

    public void Stamina(float ds)
    {
        if (!dead)
        {
            stamina += ds;
            stamina = Mathf.Clamp(stamina, 0.0f, 100.0f);
        }
    }

    public void Health(float ds)
    {
        if (!dead)
        {
            health += ds;
            health = Mathf.Clamp(health, 0.0f, 100.0f);
            if (ds < 0.0f)
            {
                DamageUI.Play("ShotUI", 0, 0.0f);
                CameraAnim.Play("Camera", 0, 0.0f);
            }
        }
    }
}
