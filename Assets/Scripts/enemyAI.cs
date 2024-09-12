using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Animator animator;
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;

    [SerializeField] AudioSource audioPlayer; 
    [SerializeField] AudioClip audioHitMarker; 

    [SerializeField] float HP;
    [SerializeField] int faceTargetSpeed;

    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    Color colorOrig;

    bool playerInRange;
    bool isShooting;
    bool isDead;

    Vector3 playerDir;

    // Start is called before the first frame update
    void Start()
    {
        //colorOrig = model.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            playerDir = gameManager.instance.player.transform.position - headPos.position;

            agent.SetDestination(gameManager.instance.player.transform.position);

            if (animator != null)
                animator.SetBool("IsMoving", agent.velocity != Vector3.zero);

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                FaceTarget();
            }

            if (playerInRange)
            {
                if (!isShooting)
                {
                    StartCoroutine(Shoot());
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            animator.SetBool("CanAttack", playerInRange);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            animator.SetBool("CanAttack", playerInRange);
        }
    }


    public void TakeDamage(float amount)
    {
        HP -= amount;

        //StartCoroutine(DamageTakenFlash());
        PlayHitMarkerSound(); 

        if (HP <= 0 && !isDead)
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
        animator.Play("Z_FallingBack");
        isDead = true;
        agent.isStopped = true;

        foreach (damage d in GetComponentsInChildren<damage>())
            Destroy(d);

        Destroy(GetComponentInChildren<MeshCollider>());
        Destroy(gameObject, 3.0f);
    }

    void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    IEnumerator Shoot()
    {
        isShooting = true;
        
        //Instantiate(bullet, shootPos.position, transform.rotation);
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    void PlayHitMarkerSound()
    {
        if(audioPlayer != null && audioHitMarker != null)
        {
            audioPlayer.PlayOneShot(audioHitMarker); 
        }    
    }
}
