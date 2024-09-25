using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damage : MonoBehaviour
{
    [SerializeField] enum damageType { bullet, stationary, melee }
    [SerializeField] damageType type;
    [SerializeField] Rigidbody rigidBody;

    [Range(0, 10)][SerializeField] int damageAmount;
    [Range(0, 50)][SerializeField] int speed;
    [Range(0, 5)][SerializeField] int destroyTime;
    [Range(0, 3)][SerializeField] float damageRate;

    bool canDamage = true;

    // Start is called before the first frame update
    void Start()
    {
        if (type == damageType.bullet)
        {
            rigidBody.velocity = transform.forward * speed;
            Destroy(gameObject, destroyTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<enemyAI>() != null)
            return;

        IDamage dmg = other.GetComponent<IDamage>();
        if (dmg != null)
        {
            if (type != damageType.stationary)
            {
                dmg.TakeDamage(damageAmount);
            }
            else if (canDamage)
            {
                StartCoroutine(DoDamage(dmg));
            }

            if (type == damageType.melee)
            {
                gameObject.SetActive(false);
            }
        }

        if (type == damageType.bullet)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator DoDamage(IDamage dmg)
    {
        canDamage = false;
        dmg.TakeDamage(damageAmount);
        yield return new WaitForSeconds(damageRate);

        canDamage = true;
    }
}
