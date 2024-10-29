using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform shootPoint;
    public string weaponName;

    private Transform target;
    private Pistol pistol;
    private Shotgun shotgun;
    private Carbine carbine;


    private void Start()
    {
        target = FindObjectOfType<PlayerMovementScript>().gameObject.transform;
        if (weaponName == "Pistol")
        {
            pistol = GetComponentInChildren<Pistol>();
        }
        else if (weaponName == "Shotgun")
        {
            shotgun = GetComponentInChildren<Shotgun>();
        }
        else if (weaponName == "Carbine")
        {
            carbine = GetComponentInChildren<Carbine>();
        }
    }

    public void Shoot() 
    {
        if (weaponName == "Pistol")
        {
            // Instantiate and shoot the bullet
            GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
            bullet.GetComponent<Bullet>().Initialize((target.position - shootPoint.position).normalized, GetComponent<Collider2D>());
        }
        else if (weaponName == "Shotgun")
        {
            Vector3 bulletSpawnPosition = shootPoint.position; // You may want to adjust this to be at the muzzle of the gun if needed

            float angleDifference = shotgun.angleDelta / (shotgun.bulletCount - 1);

            // Calculate aimAngle
            Vector3 v = target.position - shootPoint.position;
            float aimAngle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;

            float startAngle = aimAngle - shotgun.angleDelta / 2;

            // Shoot the bullets in a cone pattern
            for (int i = 0; i < shotgun.bulletCount; i++)
            {
                float currentAngle = startAngle + (i * angleDifference);
                Vector2 direction = new Vector2(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad));

                GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPosition, Quaternion.identity);
                bullet.GetComponent<Bullet>().Initialize(direction.normalized, GetComponentInParent<Collider2D>());
            }
        }
        else if (weaponName == "Carbine")
        {
            // Calculate aimAngle
            Vector3 v = target.position - shootPoint.position;
            float aimAngle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
            Vector3 shootDirection = (target.position - shootPoint.position).normalized;

            StartCoroutine(Shoot());

            void ShootOnce(Vector3 offset)
            {
                // To shoot one bullet
                Vector3 bulletSpawnPosition = shootPoint.position + offset; // Adjust spawn position by offset
                Quaternion bulletRotation = Quaternion.Euler(0, 0, aimAngle - 90);

                GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPosition, bulletRotation);
                bullet.GetComponent<Bullet>().Initialize(shootDirection, GetComponentInParent<Collider2D>());
            }

            IEnumerator Shoot()
            {
                // To call ShootOnce bulletCount many times with deltaTimeBullets long waits in between
                for (int i = 0; i < carbine.bulletCount; i++)
                {
                    // Calculate the offset for zigzag pattern
                    float offsetDirection = (i % 2 == 0) ? -1 : 1; // Alternate between left and right
                    Vector3 offset = Vector3.Cross((target.position - shootPoint.position).normalized.normalized, Vector3.forward) * carbine.distanceBullets * offsetDirection;

                    ShootOnce(offset);

                    yield return new WaitForSeconds(carbine.deltaTimeBullets);
                }
            }
        }
    }
}
