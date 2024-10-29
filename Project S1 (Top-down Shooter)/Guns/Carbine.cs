using System.Collections;
using UnityEngine;

public class Carbine : WeaponBase
{
    public float distanceBullets = 0.5f;
    public float deltaTimeBullets = 0.2f;
    public int bulletCount = 3;

    protected override void Shoot()
    {
        if (readyToShoot)
        {
            StartCoroutine(ShootCoroutine());
        }
    }

    private IEnumerator ShootCoroutine()
    {
        for (int i = 0; i < bulletCount; i++)
        {
            float offsetDirection = (i % 2 == 0) ? -1 : 1; // Alternate between left and right
            Vector3 offset = Vector3.Cross(worldDirection.normalized, Vector3.forward) * distanceBullets * offsetDirection;

            ShootOnce(offset);
            yield return new WaitForSeconds(deltaTimeBullets);
        }

        readyToShoot = false;
        timer = 0; // Reset timer for cooldown
    }

    private void ShootOnce(Vector3 offset)
    {
        timeManager.TriggerActionPassTime();
        Vector3 bulletSpawnPosition = transform.position + offset;
        Quaternion bulletRotation = Quaternion.Euler(0, 0, aimAngle - 90);

        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPosition, bulletRotation);
        bullet.GetComponent<Bullet>().Initialize(worldDirection.normalized, GetComponentInParent<Collider2D>());
    }
}
