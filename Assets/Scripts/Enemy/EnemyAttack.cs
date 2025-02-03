using System.Collections;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private EnemyScriptableObject enemyScr;
    private int damage;
    private float range;
    private float attackSpeed;

    private SpriteRenderer spriteRenderer;

    [SerializeField] private GameObject attackR, attackL;
    private SpriteRenderer spriteRendererR, spriteRendererL;

    [SerializeField] private AudioSource attackAudioSource;

    bool attack = false;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRendererR = attackR.GetComponent<SpriteRenderer>();
        spriteRendererL = attackL.GetComponent<SpriteRenderer>();
        if (enemyScr != null) SetEnemyType(enemyScr);
    }

    private IDamagable CheckAttack(Vector2 t)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(t, range);
        foreach (Collider2D collider in colliders)
        {
            IDamagable id = collider.gameObject.GetComponent<IDamagable>();
            if (collider.CompareTag("Player") && id != null)
            {
                return id;
            }
        }
        return null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (attack) return;
        IDamagable id = collision.gameObject.GetComponent<IDamagable>();
        if (collision.gameObject.CompareTag("Player") && id != null)
        {
            if (!spriteRenderer.flipX)
            {
                StartCoroutine(Attack(id, spriteRendererR, attackR.transform));
            }
            else
            {
                StartCoroutine(Attack(id, spriteRendererL, attackL.transform));
            }
        }
    }
    private IEnumerator Attack(IDamagable idP, SpriteRenderer sr, Transform t)
    {
        attack = true;
        sr.enabled = true;
        attackAudioSource.Play();
        yield return new WaitForSeconds(attackSpeed);
        if (idP == CheckAttack(t.position)) idP.TakeDamage(damage, null);
        attack = false;
        sr.enabled = false;
    }

    public void DisableAttackImg()
    {
        spriteRendererR.enabled = false;
        spriteRendererL.enabled = false;
    }

    public void SetEnemyType(EnemyScriptableObject so)
    {
        enemyScr = so;
        damage = enemyScr.damage;
        range = enemyScr.attackRange;
        attackSpeed = enemyScr.attackSpeed;
        attackAudioSource.clip = enemyScr.attackAudio;
    }
}
