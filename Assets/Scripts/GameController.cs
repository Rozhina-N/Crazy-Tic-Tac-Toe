using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public int whoTurn; // 0 = x & 1 = o
    public int turnCount; // counts the number of turns played (min for final is 15)
    public GameObject[] turnIcons; // displays whos turn it is
    public Sprite[] playIcons; // 0 = x icon and 1 = o icon
    public Button[] tictactoeSpaces; //playable spaces
    public int[] markedSpaces; // id which space is marked by which player
    public GameObject[] winningBoard; // shows the winner (and buttons)
    public GameObject endPanel; // button highlight fix
    public AudioSource buttonClickAudio; // button click sound
    public AudioSource playerWinAudio; // player win sound
    public AudioSource playerLossAudio; // player loss sound

    // Start is called before the first frame update
    void Start()
    {
        GameSetup();
    }

    void GameSetup()
    {
/*        whoTurn = 0;*/
        turnCount = 0;
        turnIcons[0].SetActive(true);
        turnIcons[1].SetActive(false);

        for(int i =  0; i < tictactoeSpaces.Length;  i++)
        {
            tictactoeSpaces[i].interactable = true;
            tictactoeSpaces[i].GetComponent<Image>().sprite = null;

        }
        for(int i = 0; i < markedSpaces.Length; i++)
        {
            markedSpaces[i] = -100;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TicTacToeButton(int WhichNumber)
    {
        tictactoeSpaces[WhichNumber].image.sprite = playIcons[whoTurn];
        tictactoeSpaces[WhichNumber].interactable = false;
        turnCount++;

        markedSpaces[WhichNumber] = whoTurn + 1;

        if (turnCount > 4)
        {
            bool isWinner = winnercheck();
            if (turnCount == 9 && isWinner == false)
            {
                Tie();
            }
        }

        if (whoTurn == 0)
        {
            whoTurn = 1;
            turnIcons[0].SetActive(false);
            turnIcons[1].SetActive(true);
        }
        else
        {
            whoTurn = 0;
            turnIcons[0].SetActive(true);
            turnIcons[1].SetActive(false);
        }
    }

    public void PlayButtonSound()
    {
        buttonClickAudio.Play();
    }

    bool winnercheck()
    {
        int s1 = markedSpaces[0] + markedSpaces[1] + markedSpaces[2];
        int s2 = markedSpaces[3] + markedSpaces[4] + markedSpaces[5];
        int s3 = markedSpaces[6] + markedSpaces[7] + markedSpaces[8];
        int s4 = markedSpaces[0] + markedSpaces[3] + markedSpaces[6];
        int s5 = markedSpaces[1] + markedSpaces[4] + markedSpaces[7];
        int s6 = markedSpaces[2] + markedSpaces[5] + markedSpaces[8];
        int s7 = markedSpaces[0] + markedSpaces[4] + markedSpaces[8];
        int s8 = markedSpaces[2] + markedSpaces[4] + markedSpaces[6];

        var solutions = new int[] { s1, s2, s3, s4, s5, s6, s7, s8 };

        for (int i = 0; i < solutions.Length; i++)
        {
            if (solutions[i] == 3 * (whoTurn + 1))
            {
                WinnerDisplay(i);
                return true;

            }
        }
        return false;
    }

    void WinnerDisplay(int indexIn)
    {
        turnIcons[0].SetActive(false);
        turnIcons[1].SetActive(false);

        endPanel.gameObject.SetActive(true);

        if (whoTurn == 0)
        {
            winningBoard[0].SetActive(true);
            playerWinAudio.Play();
        }
        else if (whoTurn == 1)
        {
            winningBoard[1].SetActive(true);
            playerWinAudio.Play();
        }

    }
    void Tie()
    {
        turnIcons[0].SetActive(false);
        turnIcons[1].SetActive(false);

        endPanel.gameObject.SetActive(true);
        winningBoard[2].SetActive(true);
        playerLossAudio.Play();
    }

    public void Rematch()
    {
        GameSetup();
        for (int i = 0; i < winningBoard.Length; i++)
        {
            winningBoard[i].SetActive(false);
        }

        if (whoTurn == 1)
        {
/*            whoTurn = 0;*/
            turnIcons[0].SetActive(false);
            turnIcons[1].SetActive(true);
        }
        
        else if (whoTurn == 0)
        {
/*            whoTurn = 1;*/
            turnIcons[0].SetActive(true);
            turnIcons[1].SetActive(false);
        }

        endPanel.gameObject.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
