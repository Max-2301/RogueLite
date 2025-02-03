using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteRenderer meleeSpriteRenderer;

    private PlayerInput playerInput;
    [SerializeField] private InputActionReference moveRef, sprintRef;
    private InputAction move, sprint;
    private Vector2 moveDirection = Vector2.zero;
    public enum MovementStatus
    {
        standing,
        walking,
        running,
        dead
    }
    private MovementStatus movementStatus = MovementStatus.standing;
    [SerializeField] private float runSpeed, walkSpeed;
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        move = playerInput.actions[moveRef.name];
        sprint = playerInput.actions[sprintRef.name];
    }
    private void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnEnable()
    {
        GetComponent<PlayerHealth>().OnPlayerDeath += SetDead;
    }
    private void OnDisable()
    {
        GetComponent<PlayerHealth>().OnPlayerDeath -= SetDead;
    }
    private void Update()
    {
        UpdateMovementStatus();
        UpdateRunStatus();
        UpdateAnimation(movementStatus);
        UpdateMovement(moveDirection.x, moveDirection.y, ReturnSpeed(movementStatus));
    }
    private float ReturnSpeed(MovementStatus status)
    {
        float speed = 0;
        switch (status)
        {
            case MovementStatus.standing:
                speed = 0;
                break;
            case MovementStatus.walking:
                speed = walkSpeed;
                break;
            case MovementStatus.running:
                speed = runSpeed;
                break;
            case MovementStatus.dead:
                speed = 0;
                break;
        }
        return speed;
    }

    private void UpdateMovementStatus()
    {
        if (movementStatus == MovementStatus.dead) { moveDirection = Vector2.zero; return; }
        if (move.ReadValue<Vector2>() != Vector2.zero)
        {
            //check if player is not pressing run button
            if (movementStatus != MovementStatus.running)
            {
                movementStatus = MovementStatus.walking;
            }
            moveDirection = move.ReadValue<Vector2>();
            //flip player sprite to look towards direction
            if (moveDirection.x < 0)
            {
                spriteRenderer.flipX = true;
                meleeSpriteRenderer.flipX = true;
                //meleeSpriteRenderer.gameObject.transform.rotation = Quaternion.Euler(0, 0, 35);
            }
            else if (moveDirection.x > 0)
            {
                spriteRenderer.flipX = false;
                meleeSpriteRenderer.flipX = false;
                //meleeSpriteRenderer.gameObject.transform.rotation = Quaternion.Euler(0, 0, -35);
            }
        }
        else
        {
            movementStatus = MovementStatus.standing;
            moveDirection = Vector2.zero;
        }
    }
    public void UpdateRunStatus()
    {
        if (sprint.ReadValue<float>() > 0)
        {
            //check if player is pressing move buttons
            if (movementStatus == MovementStatus.walking)
            {
                movementStatus = MovementStatus.running;
            }
        }
        else
        {
            if (movementStatus == MovementStatus.running)
            {
                movementStatus = MovementStatus.walking;
            }
        }
    }
    private void UpdateMovement(float xInput, float yInput, float speed)
    {
        transform.position += new Vector3(xInput * speed * Time.deltaTime, yInput * speed * Time.deltaTime);
    }
    private void UpdateAnimation(MovementStatus status)
    {
        switch (status) 
        {
            case MovementStatus.standing:
                SetAnim("Standing");
                break;
            case MovementStatus.walking:
                SetAnim("Walking");
                break;
            case MovementStatus.running:
                SetAnim("Running");
                break;
            case MovementStatus.dead:
                SetAnim("Dead");
                break;
            default:
                SetAnim("Standing");
                break;
        }
    }
    private void SetAnim(string animationParameter)
    {
        foreach(AnimatorControllerParameter p in animator.parameters)
        {
            if (p.name.Equals(animationParameter))
            {
                animator.SetBool(animationParameter, true);
            }
            else
            {
                animator.SetBool(p.name, false);
            }
        }
    }
    public MovementStatus GetMovementStatus()
    {
        return movementStatus;
    }

    public void SetDead(bool dead)
    {
        Debug.Log(dead);
        if (dead) movementStatus = MovementStatus.dead;
        else { movementStatus = MovementStatus.standing; Debug.Log("revived"); }
    }
}
