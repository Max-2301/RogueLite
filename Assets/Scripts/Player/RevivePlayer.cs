using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class RevivePlayer : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    private PlayerInput playerInput;

    [SerializeField] private InputActionReference reviveRef;
    private InputAction revive;

    [SerializeField] private float reviveRange = 1;
    private bool playerDetected = false;

    private Transform playerTransform;
    private float reviveTimer = 0;
    [SerializeField] private float reviveTime = 3;
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        revive = playerInput.actions[reviveRef.name];
    }
    private void OnEnable()
    {
        revive.started += Revive;
        revive.canceled += StopRevive;
    }
    private void Update()
    {
        if (playerDetected && Vector2.Distance(playerTransform.position, transform.position) < reviveRange)
        {
            reviveTimer += Time.deltaTime;
            if (reviveTimer > reviveTime)
            {
                playerTransform.GetComponent<PlayerHealth>().Revive();
                playerDetected = false;
                playerTransform = null;
                reviveTimer = 0;
            }
        }
        else if (playerTransform != null) 
        {
            playerDetected = false;
            playerTransform = null;
            reviveTimer = 0;
        }
    }
    public void Revive(InputAction.CallbackContext context)
    {
        if (!playerDetected)
        {
            audioSource.Play();
            float dist = reviveRange;
            foreach (GameObject Player in PlayerManager.instance.GetPlayers())
            {
                if (Player.gameObject != this.gameObject && Player.GetComponent<PlayerHealth>().GetDeath())
                {
                    float newDist = Vector2.Distance(Player.transform.position, transform.position);
                    if (newDist < dist)
                    {
                        dist = newDist;
                        playerTransform = Player.transform;
                    }
                }
            }
            if (playerTransform != null) playerDetected = true;
        }

    }

    public void StopRevive(InputAction.CallbackContext context)
    {
        audioSource.Stop(); reviveTimer = 0; playerDetected = false; playerTransform = null;
    }
}
