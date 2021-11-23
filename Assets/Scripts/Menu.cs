using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{

  
    public void PlayLevel01()
    {
        SceneManager.LoadScene(1);
        PlayerPrefs.DeleteAll();
    }
    public void QuitGame()
    {
        Application.Quit();
    }


    public void Main() { Time.timeScale = 1; SceneManager.LoadScene(0); }
    public void Continue() { gameObject.SetActive(false); Time.timeScale = 1; }

    public void Pause()
    {
        Time.timeScale = 0;
        gameObject.SetActive(true);
    }


}
