using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace CrazyTicTacToe
{
    [System.Serializable] 
    public class ReplayTurnEntity
    { 
       public int turnCount;
       public int whoTurn;
       public int boardID;
       public int buttonIndex;

        public ReplayTurnEntity(int turnCount, int whoTurn, int boardID, int buttonIndex)
        {
            this.turnCount = turnCount;
            this.whoTurn = whoTurn;
            this.boardID = boardID;
            this.buttonIndex = buttonIndex;
        }

    }

public class ReplayManager : MonoBehaviour
    {
        public List<ReplayTurnEntity> replayTurns = new List<ReplayTurnEntity>();

        public static ReplayManager instance;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            ShowSaves();
        }

        // read data from db
        // pick a session
        // replay the turns fron the start
        // ignore all current inputs
        // speed it up
        // show the end result / winner

        public void ShowSaves()
        {
            GameManager.instance.replayList.Clear();

            using var connection = new SqliteConnection(GameManager.instance.dbName);
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM sqlite_master";

                    using (IDataReader savesReader = command.ExecuteReader())
                    {
                        while (savesReader.Read())
                        {
                            string replays = savesReader.GetString(1);
                            GameManager.instance.replayList.Add(replays);
                            Debug.Log(replays);
                        }

                        savesReader.Close();

                    }

                }

                connection.Close();
            }
        }

        public void LoadGameStates(int _replayID)
        {
            using var connection = new SqliteConnection(GameManager.instance.dbName);
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM SaveGame_" + _replayID;

                    using(IDataReader replayReader = command.ExecuteReader())
                    {
                        while (replayReader.Read())
                        {
                            ReplayTurnEntity replayIdentiy = new ReplayTurnEntity(replayReader.GetInt32(0),replayReader.GetInt32(1),replayReader.GetInt32(2),replayReader.GetInt32(3));
                            replayTurns.Add(replayIdentiy); 


                            Debug.Log(replayReader.GetInt32(0) + " " + replayReader.GetInt32(1) + " " + replayReader.GetInt32(2) + " " + replayReader.GetInt32(3)); 
                        }

                        replayReader.Close();
 
                    }

                }

                connection.Close();
            }
        }

        public void ReplayGame(int _replayID)
        {
            GameManager.instance.isReplay = true;
            GameManager.instance.BlockInput.SetActive(true);
            replayTurns.Clear();
            LoadGameStates(_replayID);
            GameManager.instance.GameSetup();
            StartCoroutine(ReplayGameCoroutine());
        }

        public IEnumerator ReplayGameCoroutine()
        {
            for (int i = 0; i < replayTurns.Count; i++)
            {
                GameManager.instance.miniboard[replayTurns[i].boardID].GetComponent<GameController>().TicTacToeButton(replayTurns[i].buttonIndex);

                yield return new WaitForSeconds(0.2f);
            }
            GameManager.instance.isReplay = false;
            GameManager.instance.BlockInput.SetActive(false);
        }

    }
}
