using UnityEngine;

public class Gun : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public float impactForce = 30f;

    public Camera cam;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;

    private void Update()
    {
        if (Input.GetButtonDown("Shoot"))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        muzzleFlash.Play();

        RaycastHit hit;
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range))
        {
            Target target = hit.transform.GetComponent<Target>();
            if(target != null)
            {
                target.TakeDamage(damage);
            }

            if(hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            }

            GameObject impactGameObject = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGameObject, 2f);
        }

    }
}
