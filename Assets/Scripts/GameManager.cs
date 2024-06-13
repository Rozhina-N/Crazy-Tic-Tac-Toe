using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;
using Mono.Data.Sqlite;
using System.Text.RegularExpressions;
using System;


namespace CrazyTicTacToe
{

    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        public int whoTurn; // 0 = x & 1 = o
        public int turnCount; // counts the number of turns played (min for final is 15)
        public int[] miniboardID; // board id
        public int whichBoard; // which board is active
        public int[] isTied; // check if the game is tied
        public int GameID = 1; // game id

        public string dbName;

        public GameObject[] turnIcons; // displays whos turn it is
        public GameObject[] winningBoard; // shows the winner (and buttons)
        public GameObject endPanel; // button highlight fix
        public GameObject[] miniboard; //playable boards
        public GameObject mainGrid; // main grid
        public GameObject startPanel; // start panel
        public GameObject homeUI; // home UI
        public GameObject ReplayUI; // empty UI sprite
        public GameObject buttonPrefab; // button prefab
        public GameObject BlockInput; // block input

        public AudioSource buttonClickAudio; // button click sound
        public AudioSource playerWinAudio; // player win sound
        public AudioSource playerLossAudio; // player loss sound

        public Sprite gridHighlight; // highlight the miniboard
        public Sprite[] miniboardWinner; // miniboard winner sprite
        public Sprite Empty; // empty sprite
        
        public GameObject QuitButton; // quit button

        public bool allActive = false;
        public bool isReplay = false;

        public List<string> replayList = new List<string>();

        private void Awake()
        {
            instance = this;
            dbName = "URI=file:" + Application.dataPath + "/Saves.db";
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        public void StartGame()
        {
            startPanel.SetActive(true);
        }

        public void GameSetup()
        {
            homeUI.SetActive(false);
            startPanel.SetActive(false);
            ReplayUI.SetActive(false);
            mainGrid.SetActive(true);

            turnCount = 0;
            turnIcons[0].SetActive(true);
            turnIcons[1].SetActive(false);
            allActive = true;

            for (int i = 0; i < miniboard.Length; i++)
            {
                miniboard[i].SetActive(true); // Enable the miniboard
                miniboard[i].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                miniboard[i].GetComponent<Image>().sprite = gridHighlight;
            }

            miniboardID = new int[9];
            for (int i = 0; i < miniboardID.Length; i++)
            {
                miniboardID[i] = -100;
            }

            if (isReplay == false)
            {
                CreateDB();
            }
        }

        public void SwitchTurn()
        {
            winnercheck();
            TieCheck();

            turnCount++;

            for (int i = 0; i < miniboard.Length; i++)
            {
                if (miniboard[i].GetComponent<Image>().sprite == gridHighlight)
                {
                    miniboard[i].GetComponent<Image>().sprite = Empty;

                }
            }

            if (miniboard[whichBoard].GetComponent<Image>().sprite == Empty && isTied[whichBoard] == 0)
            {
                miniboard[whichBoard].GetComponent<Image>().sprite = gridHighlight;

            }

            else
            {
                for (int i = 0; i < miniboard.Length; i++)
                {
                    if (miniboard[i].GetComponent<Image>().sprite == Empty && isTied[i] == 0)
                    {
                        miniboard[i].GetComponent<Image>().sprite = gridHighlight;
                        allActive = true;
                    }
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

        public void MiniboardWinner(int winnerIndex)
        {
            miniboardID[winnerIndex] = whoTurn + 1;

            if (whoTurn == 0)
            {
                miniboard[winnerIndex].GetComponent<Image>().sprite = miniboardWinner[0];
                PlayWinSound();
            }
            else if (whoTurn == 1)
            {
                miniboard[winnerIndex].GetComponent<Image>().sprite = miniboardWinner[1];
                PlayWinSound();
            }
        }
        public void CreateDB()
        {
            IncrementNumber(replayList.LastOrDefault());

            using (var connection = new SqliteConnection(dbName))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "CREATE TABLE IF NOT EXISTS SaveGame_" + GameID + "(TurnCount INTEGER, WhoTurn INTEGER, BoardID INTEGER, ButtonIndex INTEGER)";
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        public void SaveGameState(int sTurnCount, int sWhoTurn, int sBoardID, int sButtonIndex)
        {
            using var connection = new SqliteConnection(dbName);
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO SaveGame_" + GameID + " (TurnCount, WhoTurn, BoardID, ButtonIndex) VALUES (@TurnCount, @WhoTurn, @BoardID, @ButtonIndex)";
                    command.Parameters.AddWithValue("@TurnCount", sTurnCount);
                    command.Parameters.AddWithValue("@WhoTurn", sWhoTurn);
                    command.Parameters.AddWithValue("@BoardID", sBoardID);
                    command.Parameters.AddWithValue("@ButtonIndex", sButtonIndex);
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        public void IncrementNumber(string input)
        {
            var regex = new Regex(@"(\d+)$");

            if (input == null)
            {
                GameID = 1;
                return;
            }

            var match = regex.Match(input);
            
            if (match.Success)
            {
                int number = int.Parse(match.Value);
                GameID = number + 1;
            }

            else
            {
                throw new ArgumentException("The input string does not end with a number.");
            }
        }

        public void PlayButtonSound()
        {
            buttonClickAudio.Play();
        }

        public void PlayWinSound()
        {
            playerWinAudio.Play();
        }

        public void PlayLossSound()
        {
            playerLossAudio.Play();
        }

        public bool winnercheck()
        {
            int s1 = miniboardID[0] + miniboardID[1] + miniboardID[2];
            int s2 = miniboardID[3] + miniboardID[4] + miniboardID[5];
            int s3 = miniboardID[6] + miniboardID[7] + miniboardID[8];
            int s4 = miniboardID[0] + miniboardID[3] + miniboardID[6];
            int s5 = miniboardID[1] + miniboardID[4] + miniboardID[7];
            int s6 = miniboardID[2] + miniboardID[5] + miniboardID[8];
            int s7 = miniboardID[0] + miniboardID[4] + miniboardID[8];
            int s8 = miniboardID[2] + miniboardID[4] + miniboardID[6];

            var solutions = new int[] { s1, s2, s3, s4, s5, s6, s7, s8 };

            for (int i = 0; i < solutions.Length; i++)
            {
                if (solutions[i] == 3 * (whoTurn + 1))
                {
                    WinnerDisplay();
                    return true;

                }
            }
            return false;
        }

        public void WinnerDisplay()
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

        public void TieCheck()
        {
            if (isTied.All(x => x == 1))
            {
                turnIcons[0].SetActive(false);
                turnIcons[1].SetActive(false);

                endPanel.gameObject.SetActive(true);
                winningBoard[2].SetActive(true);
                playerLossAudio.Play();
            }

        }

        public void Rematch()
        {
            GameSetup();
            for (int i = 0; i < winningBoard.Length; i++)
            {
                winningBoard[i].SetActive(false);
            }
            for (int i = 0; i < miniboard.Length; i++)
            {
                miniboard[i].GetComponent<GameController>().MiniboardSetup();
            }

            if (whoTurn == 1)
            {
                turnIcons[0].SetActive(false);
                turnIcons[1].SetActive(true);
            }

            else if (whoTurn == 0)
            {
                turnIcons[0].SetActive(true);
                turnIcons[1].SetActive(false);
            }

            endPanel.gameObject.SetActive(false);
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public void Home()
        {
            isReplay = true;
            GameSetup();

            for (int i = 0; i < winningBoard.Length; i++)
            {
                winningBoard[i].SetActive(false);
            }
            for (int i = 0; i < miniboard.Length; i++)
            {
                miniboard[i].GetComponent<GameController>().MiniboardSetup();
            }

            homeUI.SetActive(true);
            mainGrid.SetActive(false);
            startPanel.SetActive(false);
            endPanel.gameObject.SetActive(false);
            turnIcons[0].SetActive(false);
            turnIcons[1].SetActive(false);
            isReplay = false;
        }

        public void More()
        {
            homeUI.SetActive(false);
            ReplayUI.SetActive(true);

            foreach (Transform child in ReplayUI.transform)
            {
                Destroy(child.gameObject);
            }

            ReplayManager.instance.ShowSaves();

            foreach (string str in replayList)
            {
                GameObject button = Instantiate(buttonPrefab, ReplayUI.transform);
                button.GetComponentInChildren<Text>().text = str;
                button.GetComponent<Button>().onClick.AddListener(() => ReplayManager.instance.ReplayGame(replayList.IndexOf(str) + 1));
                
            }


        }

    }
}
