using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponHandler : MonoBehaviour
{
    [SerializeField] Transform firePos;
    [SerializeField] int damage;
    [SerializeField] float fireRate = 0.1f;
    [SerializeField] int magazineSize = 35;
    [SerializeField] float reloadRate = 3.0f;

    [SerializeField] GameObject bullet;
    [SerializeField] ParticleSystem partSystem;
    [SerializeField] AudioSource audioSystem;

    public float GetFireRate()
    {
        return fireRate;
    }

    int magazineCurrent = 0;
    bool isReloading;

    public void Fire()
    {
        if(isReloading) return;
        Vector3 ScreenCentreCoordinates = new Vector3(0.5f + Random.Range(-0.02f, 0.02f), 0.5f + Random.Range(-0.02f, 0.02f), 0f);
        Ray ray = Camera.main.ViewportPointToRay(ScreenCentreCoordinates);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.TakeDamage(damage);
            }
            else
            {
                GameObject bulletDecal = Instantiate(bullet, hit.point + hit.normal * 0.001f, Quaternion.FromToRotation(Vector3.forward, hit.normal));
                Destroy(bulletDecal, 3.0f);
            }
        }

        //Instantiate(bullet, firePos.position, firePos.rotation);
        partSystem.Play();
        audioSystem.PlayOneShot(audioSystem.clip);

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
