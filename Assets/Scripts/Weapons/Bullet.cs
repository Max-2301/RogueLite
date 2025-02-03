using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed;
    protected float deleteTime;
    private Rigidbody2D rb;
    protected GameObject player;
    protected int dmg;
    private void Update()
    {
        transform.position += transform.up * speed * Time.deltaTime;
    }
    protected virtual void Start()
    {
        Destroy(gameObject, deleteTime);
    }
    public void SetPlayer(GameObject player)
    {
        this.player = player;
    }
    public GameObject GetPlayer() {  return player; }
    public void AdjustSpeed(float ammount)
    {
        speed += ammount;
    }
    public void SetDeleteTime(float deleteTime)
    {
        this.deleteTime = deleteTime; 
    }
    public void SetDamage(int d)
    {
        dmg = d;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<IDamagable>()?.TakeDamage(dmg, player);
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }
}
