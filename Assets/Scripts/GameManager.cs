using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int level;
    public int baseSeed;
    private int prevRoomPlayerHealth;
    private int prevRoomPlayerCoin;

    private Player player;

    public static GameManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        level = 1;
        baseSeed = PlayerPrefs.GetInt("Seed");
        Random.InitState(baseSeed);
        Generation.instance.Generate();
        UI.instance.UpdateLevelText(level);

        player = FindObjectOfType<Player>();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void GoToNextLevel()
    {
        prevRoomPlayerHealth = player.curHp;
        prevRoomPlayerCoin = player.coins;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "Game")
        {
            Destroy(gameObject);
            return;
        }

        player = FindObjectOfType<Player>();
        level++;
        baseSeed++;

        Generation.instance.Generate();

        player.curHp = prevRoomPlayerHealth;
        player.coins = prevRoomPlayerCoin;

        UI.instance.UpdateHealth(prevRoomPlayerHealth);
        UI.instance.UpdateCoinText(prevRoomPlayerCoin);
        UI.instance.UpdateLevelText(level);
    }
}
