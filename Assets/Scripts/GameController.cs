using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    public int turnCount; // counts the number of turns played
    public int[] markedSpaces; // id which space is marked by which player

    public GameObject[] turnIcons; // displays whos turn it is
    public Sprite[] playIcons; // 0 = x icon and 1 = o icon
    public Button[] tictactoeSpaces; //playable spaces

    public int boardID = -1;

    void Start()
    {
        MiniboardSetup();
    }

    public void MiniboardSetup()
    {
        for (int i = 0; i < tictactoeSpaces.Length; i++)
        {
            tictactoeSpaces[i].interactable = true;
            tictactoeSpaces[i].GetComponent<Image>().sprite = GameManager.instance.Empty;

        }
        for (int i = 0; i < markedSpaces.Length; i++)
        {
            markedSpaces[i] = -100;
        }

    }

    public void TicTacToeButton(int buttonIndex)
    {
        if (GameManager.instance.whichBoard != boardID && GameManager.instance.allActive == false)
            return;

        tictactoeSpaces[buttonIndex].image.sprite = playIcons[GameManager.instance.whoTurn];
        tictactoeSpaces[buttonIndex].interactable = false;
        turnCount++;

        markedSpaces[buttonIndex] = GameManager.instance.whoTurn + 1;

        bool isWinner = winnercheck();

        if (isWinner == true)
        {
            GameManager.instance.MiniboardWinner(boardID);

            for (int i = 0; i < tictactoeSpaces.Length; i++)
            {
                tictactoeSpaces[i].interactable = false;
                tictactoeSpaces[i].gameObject.SetActive(false);


            }
        }

        GameManager.instance.whichBoard = buttonIndex;

        if (turnCount == 9 && isWinner == false)
        {
            Tie();
        }

        GameManager.instance.allActive = false;
        GameManager.instance.SwitchTurn();


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
            if (solutions[i] == 3 * (GameManager.instance.whoTurn + 1))
            {
                return true;

            }
        }
        return false;
    }

    void Tie()
    {
        GameManager.instance.isTied[boardID] = 1;
        GameManager.instance.PlayLossSound();
    }
}
