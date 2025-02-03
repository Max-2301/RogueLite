using NavMeshPlus.Components;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyBehaviour : MonoBehaviour, IDamagable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject target;
    private NavMeshAgent agent;
    private NavMeshModifier modifier;
    private List<GameObject> players = new();

    private float speed;
    private RuntimeAnimatorController animatorController;
    private int maxHealth;
    private int health;
    private int coins;
    private int score;

    private bool dead = false;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private EnemyScriptableObject enemyScr;

    [SerializeField] private AudioSource deadAudioSource;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (enemyScr != null ) SetEnemyType(enemyScr);

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = speed;
        StartCoroutine(SetTarget());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!dead)
        {
            if (target != null)
            {
                agent.SetDestination(target.transform.position);
                if (agent.velocity.x < 0) spriteRenderer.flipX = true;
                else spriteRenderer.flipX = false;
            }
            else target = CalculateClosedPlayers();
        }
    }
    private GameObject CalculateClosedPlayers()
    {
        players = PlayerManager.instance.GetPlayers();
        GameObject player = null;
        float dist = float.PositiveInfinity;
        foreach (GameObject p in players)
        {
            if (p.GetComponent<PlayerController>().GetMovementStatus() != PlayerController.MovementStatus.dead)
            {
                float newDist = Mathf.Abs(Vector2.Distance(transform.position, p.transform.position));
                if (newDist < dist) { dist = newDist; player = p; }
            }
        }
        return player;
    }
    private IEnumerator SetTarget()
    {
        while (true)
        {
            target = CalculateClosedPlayers();
            yield return new WaitForSeconds(2);
        }
    }

    public void TakeDamage(int damage, GameObject shotBy)
    {
        health -= damage;
        if (health < 0 && !dead)
        {
            dead = true;
            animator.SetBool("Dead", dead);
            deadAudioSource.Play();
            target = null;
            StopCoroutine(SetTarget());
            shotBy.GetComponent<Inventory>().AddCoins(coins);
            agent.enabled = false;
            GetComponent<Rigidbody2D>().totalForce = Vector2.zero;
            GetComponent<BoxCollider2D>().enabled = false;
            EnemyAttack attackSc = GetComponent<EnemyAttack>();
            attackSc.StopAllCoroutines();
            attackSc.DisableAttackImg();
            UIManager.instance.AddScore(score);
            Destroy(gameObject, 5);
        }
        else
        {
            animator.SetTrigger("Hit");
            GetComponent<SpriteRenderer>().sortingOrder = 1;
            shotBy.GetComponent<Inventory>().AddCoins(1);
        }
    }

    public void SetEnemyType(EnemyScriptableObject so)
    {
        enemyScr = so;
        speed = enemyScr.speed;
        animatorController = enemyScr.animator;
        maxHealth = enemyScr.maxHealth;
        health = maxHealth;
        coins = enemyScr.coins;
        score = enemyScr.score;
        deadAudioSource.clip = enemyScr.deathAudio;
        animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = animatorController;
    }
}
