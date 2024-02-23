using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public TMP_InputField seedInput;

    private void Start()
    {
        seedInput.text = PlayerPrefs.GetInt("Seed").ToString();
    }

    public void OnUpdateSeed()
    {
        PlayerPrefs.SetInt("Seed", int.Parse(seedInput.text));
    }

    public void OnPlayButton()
    {
        SceneManager.LoadScene("Game");
    }
}
