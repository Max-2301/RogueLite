using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Melee : WeaponBase
{
    [SerializeField] private Transform aoeL, aoeR;
    private Animator animator;
    [SerializeField] private Animation anim;
    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
    }
    protected override void UseWeapon()
    {
        animator.SetFloat("SpeedDevider", 1/(useTime/useTimeMultipler));
        animator.SetTrigger("Attack");
        if (spriteRenderer.flipX)
        {
            AoeAttack(aoeL.position);
        }
        else
        {
            AoeAttack(aoeR.position);
        }
        weaponAudioSource.Play();
        weaponAudioSource.pitch = 1.2f;
    }
    private void AoeAttack(Vector2 t)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(t, range);
        foreach (Collider2D collider in colliders)
        {
            IDamagable id = collider.gameObject.GetComponent<IDamagable>();
            if(collider.CompareTag("Enemy") && id != null)
            {
                id.TakeDamage(dmg, player);
                collider.GetComponent<Rigidbody2D>().AddForce(-(player.transform.position - collider.transform.position) * knockBack, ForceMode2D.Impulse);
                weaponAudioSource.pitch = 1;
            }
        }
    }
    public void OnDrawGizmos()
    {
        if (used)
        {
            Gizmos.color = Color.red;
            if (spriteRenderer.flipX)
            {
                Gizmos.DrawSphere(aoeL.position, range);
            }
            else
            {
                Gizmos.DrawSphere(aoeR.position, range);
            }
        }
    }
}
