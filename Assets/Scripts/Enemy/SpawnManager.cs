using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using static SpawnManager;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instance;

    [System.Serializable]
    public class Round
    {
        public string name;
        public List<EnemyScriptableObject> enemys;
    }
    [System.Serializable]
    public class Enemys
    {
        [HideInInspector] public string name = "Round Types";
        public List<Round> roundTypes;
    }
    public Enemys enemys = new();
    [SerializeField] private GameObject basicEnemy;

    private int round = 0;
    [SerializeField, Tooltip("ammount spawned for first round")] private int spawnAmmount =  5;
    [SerializeField, Tooltip("% increase in ammount spawned per round")]private float spawnIncrease = 1.25f;
    [SerializeField] int maxSpawnAmmount = 50;

    [SerializeField] private GameObject spawnedEnemysParent;
    [SerializeField] private GameObject spawnPointsParent;
    private List<Transform> spawnPoints = new();
    private List<Transform> activeSpawnPoints = new();
    [SerializeField] private bool visualisePoints = true;
    [SerializeField] private float checkTimer = 1;

    [SerializeField] private float spawnableRadius = 100;
    [SerializeField] private float minSpawnableRadius = 10;

    [SerializeField] private float spawnTime = 4;
    [SerializeField] private float minSpawnTime = 0.25f;
    [SerializeField, Tooltip("% increase in ammount spawned per round")] private float spawnTimeDecrease = 0.1f;

    [SerializeField] private AudioSource normalAudioSource;
    [SerializeField] private AudioSource specialAudioSource;

    [SerializeField] private TextMeshProUGUI roundText;

    int players = 0;
    private void Start()
    {
        AddSpawnPoints(0);
        StartCoroutine(SetSpawnPoints(spawnPoints, activeSpawnPoints));
        round = 1;
        StartRound();
    }

    private void OnEnable()
    {
        PlayerManager.instance.onAllPlayersDied += StopSpawning;
        PlayerManager.instance.playerInputManager.onPlayerJoined += AddPlayer;
    }
    private void OnDisable()
    {
        PlayerManager.instance.onAllPlayersDied -= StopSpawning;
        PlayerManager.instance.playerInputManager.onPlayerJoined -= AddPlayer;
    }

    private void StartRound()
    {
        if (round % 5 != 0)
        {
            if (round < 4)
            {
                normalAudioSource.Play();
                StartCoroutine(SpawnRound(2));
            }
            else
            {
                normalAudioSource.Play();
                StartCoroutine(SpawnRound(0));
            }
        }
        else
        {
            specialAudioSource.Play();
            StartCoroutine(SpawnSpecialRound(1));
        }
    }

    private IEnumerator SpawnRound(int roundType)
    {
        yield return new WaitUntil(() => players > 0);
        yield return new WaitForSeconds(5);
        for (int i = 0; i < spawnAmmount * players/2; i++)
        {
            yield return new WaitForSeconds(spawnTime);
            yield return new WaitUntil(() => spawnedEnemysParent.transform.childCount < maxSpawnAmmount);
            Transform spawnPoint = GetSpawnPoint();
            GameObject enemy = Instantiate(basicEnemy, spawnPoint.position, spawnPoint.rotation, spawnedEnemysParent.transform);
            int enemyI = Random.Range(0, enemys.roundTypes[roundType].enemys.Count);
            enemy.GetComponent<EnemyAttack>().SetEnemyType(enemys.roundTypes[roundType].enemys[enemyI]);
            enemy.GetComponent<EnemyBehaviour>().SetEnemyType(enemys.roundTypes[roundType].enemys[enemyI]);
        }
        StartCoroutine(CheckAllKilled());
    }


    private IEnumerator SpawnSpecialRound(int roundType)
    {
        int players = PlayerManager.instance.players.Count;
        yield return new WaitForSeconds(5);
        Transform spawnPointBoss = GetSpawnPoint();
        GameObject boss = Instantiate(basicEnemy, spawnPointBoss.position, spawnPointBoss.rotation, spawnedEnemysParent.transform);
        int bossI = 0;
        boss.GetComponent<EnemyAttack>().SetEnemyType(enemys.roundTypes[roundType].enemys[bossI]);
        boss.GetComponent<EnemyBehaviour>().SetEnemyType(enemys.roundTypes[roundType].enemys[bossI]);
        while (boss != null)
        {
            yield return new WaitForSeconds(spawnTime);
            yield return new WaitUntil(() => spawnedEnemysParent.transform.childCount < maxSpawnAmmount);
            Transform spawnPoint = GetSpawnPoint();
            GameObject enemy = Instantiate(basicEnemy, spawnPoint.position, spawnPoint.rotation, spawnedEnemysParent.transform);
            int enemyI = 1;
            enemy.GetComponent<EnemyAttack>().SetEnemyType(enemys.roundTypes[roundType].enemys[enemyI]);
            enemy.GetComponent<EnemyBehaviour>().SetEnemyType(enemys.roundTypes[roundType].enemys[enemyI]);
        }
        StartCoroutine(CheckAllKilled());
    }
    private IEnumerator CheckAllKilled()
    {
        while (spawnedEnemysParent.transform.childCount > 0)
        {
            yield return new WaitForFixedUpdate();
        }
        RoundEnded();
        StartRound();
    }

    private void RoundEnded()
    {
        round++;
        roundText.text = "Lv. " + round.ToString();
        spawnTime *= spawnTimeDecrease;
        if (spawnTime < minSpawnTime) spawnTime = minSpawnTime;
        spawnAmmount = (int)((float)spawnAmmount * spawnIncrease);
        if (spawnAmmount > maxSpawnAmmount) spawnAmmount = maxSpawnAmmount;
    }

    private Transform GetSpawnPoint()
    {
        return activeSpawnPoints[Random.Range(0, activeSpawnPoints.Count)];
    }

    private IEnumerator SetSpawnPoints(List<Transform> points, List<Transform> activePoints)
    {
        foreach (Transform t in points)
        {
            yield return new WaitForFixedUpdate();
            bool pointShouldBeActive = false;  // Track whether the point should be active
            bool anyPlayerTooClose = false;    // Flag to track if any player is too close
            bool allPlayersTooFar = true;      // Flag to track if all players are too far

            foreach (GameObject player in PlayerManager.instance.GetPlayers())
            {
                float dst = Vector2.Distance(t.transform.position, player.transform.position);

                if (dst < minSpawnableRadius)
                {
                    // If any player is too close, deactivate the spawn point
                    anyPlayerTooClose = true;
                    break;  // No need to check further players if one is too close
                }

                if (dst < spawnableRadius)
                {
                    // If any player is within the valid spawnable range, activate the point
                    pointShouldBeActive = true;
                }

                if (dst > spawnableRadius)
                {
                    // Track if the player is too far to activate the point
                    allPlayersTooFar = allPlayersTooFar && true;
                }
                else
                {
                    allPlayersTooFar = false;  // At least one player is within range
                }
            }

            // Deactivate if any player is too close or all players are too far
            if (anyPlayerTooClose || allPlayersTooFar)
            {
                if (activePoints.Contains(t))
                {
                    activePoints.Remove(t);  // Deactivate spawn point
                }
            }
            else if (pointShouldBeActive && !activePoints.Contains(t))
            {
                // Activate the spawn point if it's not already active
                activePoints.Add(t);
            }
        }
        ShowActiveSpawners(points, activePoints);
        yield return new WaitForSeconds(checkTimer);
        StartCoroutine(SetSpawnPoints(points, activePoints));
    }
    private void ShowActiveSpawners(List<Transform> points, List<Transform> activePoints)
    {
        foreach (Transform t in points)
        {
            if (!visualisePoints)
            {
                t.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            }
            else
            {
                t.gameObject.GetComponent<SpriteRenderer>().enabled = true;
                if (activePoints.Contains(t))
                {
                    t.gameObject.GetComponent<Renderer>().material.color = Color.green;
                }
                else
                {
                    t.gameObject.GetComponent<Renderer>().material.color = Color.red;
                }
            }
        }
    }

    private void AddSpawnPoints(int room)
    {
        for (int i = 0; i < spawnPointsParent.transform.GetChild(room).childCount; i++)
        {
            spawnPoints.Add(spawnPointsParent.transform.GetChild(room).GetChild(i));
        }
    }

    public void StopSpawning()
    {
        StopAllCoroutines();
    }

    public void AddPlayer(PlayerInput player)
    {
        Debug.Log("playerAdded");
        if (PlayerManager.instance.GetPlayers().Count < 4) players++;
    }
}
