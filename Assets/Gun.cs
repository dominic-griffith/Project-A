using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public float impactForce = 30f;

    public Camera cam;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;
    public TrailRenderer bulletTrail;

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
        TrailRenderer trail = Instantiate(bulletTrail, muzzleFlash.transform.position, Quaternion.identity);

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range))
        {

            StartCoroutine(StartTrail(trail, hit.point));

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
        else
        {
            StartCoroutine(StartTrail(trail, Vector3.Scale(cam.transform.forward, new Vector3(range, range, range)) + cam.transform.position));
        }

    }

    private IEnumerator StartTrail(TrailRenderer trail, Vector3 hitPoint)
    {
        float time = 0;

        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(muzzleFlash.transform.position, hitPoint, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }
        Destroy(trail.gameObject, trail.time);
    }
}
