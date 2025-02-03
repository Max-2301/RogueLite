using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }

    private int score = 0;

    [SerializeField] private TextMeshProUGUI scoreText, roundText;

    private PlayerInputManager playerInputManager;

    [SerializeField] private GameObject playerUIContainer;
    [SerializeField] private GameObject playerUI;

    private List<GameObject> playerUIElements = new();
    private void Awake()
    {
        instance = this;
        playerInputManager = PlayerManager.instance.GetComponent<PlayerInputManager>();
    }
    private void OnEnable()
    {
        playerInputManager.onPlayerJoined += AddPlayerUI;
        playerInputManager.onPlayerLeft += RemovePlayerUI;
    }
    private void OnDisable()
    {
        playerInputManager.onPlayerJoined -= AddPlayerUI;
        playerInputManager.onPlayerLeft -= RemovePlayerUI;
    }
    public void AddScore(int ammount)
    {
        score++;
        scoreText.text = score.ToString();
    }

    public void SetRoundText(int round)
    {

    }

    public void AddPlayerUI(PlayerInput input)
    {
        GameObject uie = Instantiate(playerUI, playerUIContainer.transform);
        playerUIElements.Add(uie);
        PlayerUIComponents uic = uie.GetComponent<PlayerUIComponents>();
        input.transform.GetComponent<Inventory>().SetUI(uic.buyText, uic.buyImage, uic.gunText, uic.gunImage, uic.meleeIamge, uic.upgrades.GetComponentsInChildren<Image>().ToList(), uic.coinText);
        input.transform.GetComponent<PlayerHealth>().SetUI(uic.hearts.GetComponent<SetHearts>());
        uic.playerText.text = "P" + playerUIElements.Count.ToString();
    }

    public void RemovePlayerUI(PlayerInput input)
    {
        List<GameObject> players = PlayerManager.instance.GetPlayers();
        for (int i = 0; i < players.Count; i++)
        {
            if (input.gameObject == players[i]) Destroy(playerUIElements[i]);
        }
    }
}
