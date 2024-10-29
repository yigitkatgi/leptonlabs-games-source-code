using UnityEngine;

public class Pistol : WeaponBase
{
    protected override void Shoot()
    {
        if (readyToShoot)
        {
            timeManager.TriggerActionPassTime();
            Vector3 bulletSpawnPosition = transform.position; // Adjust this to be at the muzzle if needed

            // Use worldDirection for consistent direction relative to the camera
            Quaternion bulletRotation = Quaternion.Euler(0, 0, aimAngle - 90);
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPosition, bulletRotation);
            bullet.GetComponent<Bullet>().Initialize(worldDirection.normalized, GetComponentInParent<Collider2D>());

            readyToShoot = false;
            timer = 0; // Reset timer for cooldown
        }
    }
}
