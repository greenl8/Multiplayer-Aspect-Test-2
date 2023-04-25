using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;
    GameObject controller;
    public bool playerOne;
    public bool playerTwo;
    public TMP_Text whichTeamMine;
    GameManager GM;

    private void Awake()
    {
        GM = FindObjectOfType<GameManager>();
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        if(PV.IsMine)
        {
            this.CreateController();
            this.SetTeam();
            this.WhichTeamUi();
        }
    }

    void Update()
    {
       
    }

    void WhichTeamUi()
    {
        if(playerOne)
        {
            whichTeamMine.text = "PLAYER ONE";
        }
        else if(playerTwo)
        {
            whichTeamMine.text = "PLAYER TWO";
        }
    }

    void SetTeam()
    {
        int playerCount = PhotonNetwork.CountOfPlayers;
        if (playerCount == 0 || playerCount % 2 == 0)
        {
            playerOne = true;
        }
        else
        {
            playerTwo = true;
        }
    }
    
    void CreateController()
    {
        if(playerTwo)
        {
            GM.playerOneScore++;
        }
        else if(playerOne)
        {
            GM.playerTwoScore++;
        }
        
        Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint();
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID });
    }

    public void Die()
    {
        PhotonNetwork.Destroy(controller);
        CreateController();
    }
}
