using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using CrazyTicTacToe;
using UnityEngine.UI;



[TestFixture]
public class GameTests
{
    private GameObject gameControllerObject;
    private GameController gameController;
    private GameManager gameManager;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        gameControllerObject = new GameObject();
        gameManager = gameControllerObject.AddComponent<GameManager>();
        gameController = gameControllerObject.AddComponent<GameController>();

        GameManager.instance = gameManager;

        gameManager.turnIcons = new GameObject[2];
        gameController.playIcons = new Sprite[2];
        gameController.tictactoeSpaces = new Button[9];
        gameController.markedSpaces = new int[9];

        for (int i = 0; i < gameController.tictactoeSpaces.Length; i++)
        {
            var buttonObject = new GameObject();
            gameController.tictactoeSpaces[i] = buttonObject.AddComponent<Button>();
            gameController.tictactoeSpaces[i].image = buttonObject.AddComponent<Image>();
        }

        yield return null;
    }


    [UnityTest]
    public IEnumerator TestMiniboardSetup()
    {
        gameController.MiniboardSetup();

        Assert.AreEqual(0, gameController.turnCount);
        for (int i = 0; i < gameController.tictactoeSpaces.Length; i++)
        {
            Assert.IsTrue(gameController.tictactoeSpaces[i].interactable);
            Assert.AreEqual(gameManager.Empty, gameController.tictactoeSpaces[i].image.sprite);
        }
        for (int i = 0; i < gameController.markedSpaces.Length; i++)
        {
            Assert.AreEqual(-100, gameController.markedSpaces[i]);
        }

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestTicTacToeButton_InvalidMove_BoardNotActive()
    {
        gameManager.whoTurn = 0;
        gameManager.whichBoard = 1;
        gameManager.allActive = false;
        gameController.boardID = 0;

        gameController.TicTacToeButton(0);

        Assert.AreEqual(0, gameController.turnCount);
        Assert.IsTrue(gameController.tictactoeSpaces[0].interactable);
        Assert.AreEqual(-100, gameController.markedSpaces[0]);

        yield return null;
    }
}