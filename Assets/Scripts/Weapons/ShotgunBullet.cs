using UnityEngine;

public class ShotgunBullet : Bullet
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private int spreadBulletAmmount;

    [SerializeField] private float bulletSpread = 1, bulletSpreadSpeedOffset = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        for (int i = -spreadBulletAmmount/2; i <= spreadBulletAmmount/2; i++)
        {
            float spread = Random.Range(-bulletSpread, bulletSpread);
            if (i != 0)
            {
                GameObject b = Instantiate(bullet, transform.position, transform.rotation);
                b.transform.Rotate(0,0,spread * i);
                Bullet scrB = b.GetComponent<Bullet>();
                scrB.SetPlayer(player);
                scrB.AdjustSpeed(Random.Range(-bulletSpreadSpeedOffset, bulletSpreadSpeedOffset));
                scrB.SetDamage(dmg);
                scrB.SetDeleteTime(deleteTime);
            }
        }
        base.Start();
    }
}
