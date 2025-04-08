using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //[SerializeField] GameObject _optionsObject;
    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
    public void Options(GameObject option)
    {
        option.SetActive(true);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
