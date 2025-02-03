using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    public PlayerInputManager playerInputManager;

    public List<GameObject> players;
    public List<Transform> spawnPoints;

    [SerializeField] private List<RuntimeAnimatorController> animators;

    private int deathAmmount = 0;

    public delegate void AllPlayersDied();
    public event AllPlayersDied onAllPlayersDied;
    private void Awake()
    {
        instance = this;
        playerInputManager = GetComponent<PlayerInputManager>();
    }
    private void Start()
    {
        //SetSpawnPoints();
        //MoveAllPlayersToSpawnPoint();
    }

    private void SetSpawnPoints()
    {
        spawnPoints.Clear();
        Transform spwawnPoints = GameObject.Find("SpawnPoints").transform;
        for (int i = 0; i < spwawnPoints.childCount; i++)
        {
            spawnPoints.Add(spwawnPoints.GetChild(i).transform);
        }
    }
    private void MoveAllPlayersToSpawnPoint()
    {
        for (int i = 0; i < players.Count; i++)
        {
            SetPlayerSpawnPositions(players[i].GetComponent<PlayerInput>(), i);
        }
    }
    private void SetPlayerSpawnPositions(PlayerInput player, int spawnPointI)
    {
        player.transform.position = spawnPoints[spawnPointI].position;
        player.transform.rotation = spawnPoints[spawnPointI].rotation;
    }

    //public void OnPlayerConnected(NetworkPlayer player)
    //{

    //}

    public void OnEnable()
    {
        playerInputManager.onPlayerJoined += AddPlayer;
        playerInputManager.onPlayerLeft += RemovePlayer;
    }

    public void OnDisable()
    {
        playerInputManager.onPlayerJoined -= AddPlayer;
        playerInputManager.onPlayerLeft -= RemovePlayer;
    }

    public void AddPlayer(PlayerInput player)
    {
        if (players.Count > 4)
        {
            return;
        }
        players.Add(player.gameObject);
        player.GetComponent<PlayerHealth>().OnPlayerDeath += AddDeath;
        player.GetComponent<Animator>().runtimeAnimatorController = animators[players.Count - 1];
        if (players.Count == 1)
        {
            player.transform.position = transform.position;
        }
        else player.transform.position = players[0].transform.position;
        //SetPlayerSpawnPositions(player, players.Count - 1);
        if (players.Count <= 1)
        {
            //set main player controls for ui etc
        }
    }
    public void RemovePlayer(PlayerInput player)
    {
        if (players.Count <= 1)
        {
        }
        players.Remove(player.gameObject);
        player.GetComponent<PlayerHealth>().OnPlayerDeath -= AddDeath;
        Destroy(player.gameObject);
    }
    public List<GameObject> GetPlayers()
    {
        return players;
    }
    public void AddDeath(bool dead)
    {
        if (dead)
        {
            deathAmmount++;
            if (deathAmmount > players.Count) onAllPlayersDied.Invoke();
        }
        else deathAmmount--;
    }

    public void Restart()
    {
        Debug.Log("restart");
        SceneManager.LoadScene(0);
    }
}