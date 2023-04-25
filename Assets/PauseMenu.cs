using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class PauseMenu : MonoBehaviour
{

    public GameObject pauseMenu;
    
    public void Update()
    {
        PauseMenuTrigger();
    }


    public void ToMainMenu()
    {
        SceneManager.LoadScene(0);
        PhotonNetwork.LeaveRoom();
    }

    void PauseMenuTrigger()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            pauseMenu.SetActive(!pauseMenu.activeInHierarchy);
        }
        
    }
}
