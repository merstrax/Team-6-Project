using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;

    [SerializeField] float HP;
    [SerializeField] int faceTargetSpeed;

    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    Color colorOrig;

    bool playerInRange;
    bool isShooting;

    Vector3 playerDir;

    // Start is called before the first frame update
    void Start()
    {
        colorOrig = model.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        playerDir = gameManager.instance.player.transform.position - headPos.position;

        agent.SetDestination(gameManager.instance.player.transform.position);

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            FaceTarget();
        }

        //if (playerInRange)
        {
            if (!isShooting)
            {
                StartCoroutine(Shoot());
            }
        }
    }

    public void TakeDamage(float amount)
    {
        HP -= amount;

        StartCoroutine(DamageTakenFlash());

        if (HP <= 0)
        {
            Die();
        }
    }

    IEnumerator DamageTakenFlash()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        model.material.color = colorOrig;
    }

    void Die()
    {
        gameManager.instance.UpdateGameGoal();
        Destroy(gameObject); 
    }

    void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    IEnumerator Shoot()
    {
        isShooting = true;
        Instantiate(bullet, shootPos.position, transform.rotation);
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }
}
