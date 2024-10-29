using UnityEngine;

public class Shotgun : WeaponBase
{
    public float angleDelta = 45;
    public int bulletCount = 5;

    protected override void Shoot()
    {
        if (readyToShoot)
        {
            timeManager.TriggerActionPassTime();
            Vector3 bulletSpawnPosition = transform.position;

            float angleDifference = angleDelta / (bulletCount - 1);
            float startAngle = aimAngle - angleDelta / 2;

            for (int i = 0; i < bulletCount; i++)
            {
                float currentAngle = startAngle + (i * angleDifference);
                Vector2 dir = new Vector2(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad));

                GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPosition, Quaternion.identity);
                bullet.GetComponent<Bullet>().Initialize(dir, GetComponentInParent<Collider2D>());
            }

            readyToShoot = false;
            timer = 0; // Reset timer for cooldown
        }
    }
}
