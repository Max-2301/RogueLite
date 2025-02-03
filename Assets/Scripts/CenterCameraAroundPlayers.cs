using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CenterCameraAroundPlayers : MonoBehaviour
{
    List<GameObject> players = new();
    [SerializeField] private float minDist = -10;

    PlayerInputManager playerInputManager;
    
    [SerializeField] private float lerpSpeed = 5f;

    private int deadPlayers = 0;
    private void Awake()
    {
        playerInputManager = PlayerManager.instance.GetComponent<PlayerInputManager>();
    }
    private void OnEnable()
    {
        playerInputManager.onPlayerJoined += AddPlayer;
        playerInputManager.onPlayerLeft += RemovePlayer;
    }
    private void OnDisable()
    {
        playerInputManager.onPlayerJoined -= AddPlayer;
        playerInputManager.onPlayerLeft -= RemovePlayer;
    }

    private void Update()
    {
        if (players.Count == 0 || deadPlayers >= players.Count) return;

        Vector3 totalPlayersPos = Vector2.zero;
        foreach (GameObject player in players)
        {
            if (player == null) continue;  // Skip null players

            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (!playerHealth.GetDeath())  // Ensure PlayerHealth exists and the player is alive
            {
                totalPlayersPos += player.transform.position;
            }
        }
        //if (deadPlayers >= players.Count) totalPlayersPos = transform.position;
        if (players.Count > 0)
        {
            totalPlayersPos /= players.Count - deadPlayers;
            totalPlayersPos.z = minDist;
            // Lerp the camera position towards the target position
            transform.position = Vector3.Lerp(transform.position, totalPlayersPos, lerpSpeed * Time.deltaTime);
        }
    }

    public void AddPlayer(PlayerInput input)
    {
        players.Add(input.gameObject);
        input.gameObject.GetComponent<PlayerHealth>().OnPlayerDeath += AddDeath;
    }

    public void RemovePlayer(PlayerInput input)
    {
        players.Remove(input.gameObject);
        input.gameObject.GetComponent<PlayerHealth>().OnPlayerDeath -= AddDeath;
    }

    public void AddDeath(bool dead)
    {
        if (dead) deadPlayers++;
        else deadPlayers--;
    }
}
