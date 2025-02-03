using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Scriptable Objects/Enemy")]
public class EnemyScriptableObject : ScriptableObject
{
    public float speed;
    public float attackSpeed;
    public float attackRange;
    public int damage;
    public RuntimeAnimatorController animator;
    public int maxHealth;
    public int coins;
    public int score;

    public AudioClip deathAudio;
    public AudioClip attackAudio;
}
