using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static float maxTime = 30.0f;
    public TMP_Text timerText;
    public static GameManager Instance;
    public int playerOneScore = 0;
    public int playerTwoScore = 0;
    public TMP_Text FinalScoreText;
    public TMP_Text playerOneText;
    public TMP_Text playerTwoText;
    public GameObject RoundFinishText;
    PlayerManager pm;
    PhotonView PV;
   

    private void Awake()
    {
        Instance = this;
        SetScoreText();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        StartMatch();
        SetScoreText();
    }

    void SetScoreText()
    {
        playerOneText.text = playerOneScore.ToString();
        playerTwoText.text = playerTwoScore.ToString();
    }

    public void StartMatch()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            CountDownTimer();
        }
    }
    void CountDownTimer()
    {
        if(maxTime > 0)
        {
            maxTime -= Time.deltaTime;
        }

        double b = Math.Round(maxTime, 1);

        timerText.text = b.ToString();

        if(maxTime <= 0)
        {
            StartCoroutine(RoundFinishDelay());
        }
    }

   
    IEnumerator RoundFinishDelay()
    {
        if (playerTwoScore > playerOneScore)
        {
            FinalScoreText.text = "PLAYER TWO WINS";
        }
        else if (playerTwoScore < playerOneScore)
        {
            FinalScoreText.text = "PLAYER ONE WINS";
        }
        else
        {
            FinalScoreText.text = "DRAW";
        }
        RoundFinishText.SetActive(true);

        yield return new WaitForSeconds(5f);

        RoundFinishText.SetActive(false);

        playerOneScore = 0;

        playerTwoScore = 0;

        SceneManager.LoadScene(1);

        maxTime = 30;
    }
}
