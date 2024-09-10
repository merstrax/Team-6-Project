using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;


    [SerializeField] float HP;

    Color colorOrig;

    // Start is called before the first frame update
    void Start()
    {
        colorOrig = model.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(gameManager.instance.player.transform.position);
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
}
