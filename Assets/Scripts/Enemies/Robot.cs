using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Robot : Enemy
{
    public float robotDistanceRun = 4.0f;

    private NavMeshAgent robot;
    private AudioSource audio;

    void Start()
    {
        robot = GetComponent<NavMeshAgent>();
        Active = false;

        Target = GameObject.FindGameObjectWithTag("Player").transform;
        audio = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Active && !GameManager.Instance.dead)
        {
            float distance = Vector3.Distance(transform.position, Target.position);

            //Run towards Player
            if (distance < robotDistanceRun)
            {
                Vector3 dirToPlayer = transform.position - Target.position;
                Vector3 newPos = transform.position - dirToPlayer;
                robot.SetDestination(newPos);
            }
        }
    }
}