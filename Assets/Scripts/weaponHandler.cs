using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponHandler : MonoBehaviour
{
    [SerializeField] Transform firePos;
    [SerializeField] float fireRate = 0.1f;
    [SerializeField] int magazineSize = 35;
    [SerializeField] float reloadRate = 3.0f;

    [SerializeField] GameObject bullet;
    [SerializeField] ParticleSystem partSystem;

    public float GetFireRate()
    {
        return fireRate;
    }

    int magazineCurrent = 0;
    bool isReloading;

    public void Fire()
    {
        if(isReloading) return;
        Instantiate(bullet, firePos.position, firePos.rotation);
        partSystem.Play();
        magazineCurrent -= 1;
    }

    IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadRate);

        isReloading = false;
        magazineCurrent = magazineSize;
    }
}
