using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    enum EnemyState
    {
        PatrolState,
        ChaseState
    }

    EnemyState state;

    [Range(0, 360)]
    public float angle;
    public float radius;

    public GameObject playerRef;

    public LayerMask targetMask;
    public LayerMask obstructionMask;

    public bool seesPlayer;

    public Transform[] waypoints;
    public int speed;

    private int waypointIndex;
    private float dist;

    public GameObject bullet;
    public int shootingSpeed;


    // Start is called before the first frame update
    void Start()
    {
        state = EnemyState.PatrolState;

        waypointIndex = 0;
        transform.LookAt(waypoints[waypointIndex].position);

        playerRef = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(FOVRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        RunState();
    }

    void RunState()
    {
        switch (state)
        {
            case EnemyState.PatrolState:
            {
                PatrolState();
                break;
            }
            case EnemyState.ChaseState:
            {
                ChaseState();
                break;
            }
        }
    }

    void PatrolState()
    {
        dist = Vector3.Distance(transform.position, waypoints[waypointIndex].position);
        if (dist < 1f)
        {
            IncreaseIndex();
        }
        Patrol();

        if (seesPlayer)
        {
            state = EnemyState.ChaseState;
            StartCoroutine(ShootRoutine());
        }
    }

    void Patrol()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void IncreaseIndex()
    {
        waypointIndex++;
        if (waypointIndex >= waypoints.Length)
        {
            waypointIndex = 0;
        }
        transform.LookAt(waypoints[waypointIndex].position);
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    seesPlayer = true;
                }
                else
                {
                    seesPlayer = false;
                }
            }
            else
            {
                seesPlayer = false;
            }
        }
        else if (seesPlayer)
        {
            seesPlayer = false;
        }
    }

    void ChaseState()
    {
        transform.LookAt(playerRef.transform.position);
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private IEnumerator ShootRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(3f);

        while (true)
        {
            yield return wait;
            shoot();
        }
    }

    void shoot()
    {
        Vector3 dir = (playerRef.transform.position - transform.position).normalized;
        Instantiate(bullet, transform.position + (dir * 2), Quaternion.LookRotation(dir));
    }
}
