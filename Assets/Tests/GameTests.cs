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

    [Test]
    public void TestMiniboardSetup()
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
    }

    [Test]
    public void TestTicTacToeButton_ValidMove()
    {
        gameManager.whoTurn = 0;
        gameManager.whichBoard = -1;
        gameManager.allActive = true;
        gameController.boardID = 0;

        gameController.TicTacToeButton(0);

        Assert.AreEqual(1, gameController.turnCount);
        Assert.IsFalse(gameController.tictactoeSpaces[0].interactable);
        Assert.AreEqual(gameManager.whoTurn + 1, gameController.markedSpaces[0]);
        Assert.AreEqual(1, gameManager.turnCount);
        Assert.AreEqual(1, gameManager.whoTurn);
        Assert.AreEqual(0, gameManager.whichBoard);
    }

    [Test]
    public void TestTicTacToeButton_WinningMove()
    {
        gameManager.whoTurn = 0;
        gameManager.whichBoard = -1;
        gameManager.allActive = true;
        gameController.boardID = 0;

        gameController.markedSpaces[0] = 1;
        gameController.markedSpaces[1] = 1;

        gameController.TicTacToeButton(2);

        Assert.IsTrue(gameController.winnercheck());
    }

    [Test]
    public void TestTicTacToeButton_InvalidMove_BoardNotActive()
    {
        gameManager.whoTurn = 0;
        gameManager.whichBoard = 1;
        gameManager.allActive = false;
        gameController.boardID = 0;

        gameController.TicTacToeButton(0);

        Assert.AreEqual(0, gameController.turnCount);
        Assert.IsTrue(gameController.tictactoeSpaces[0].interactable);
        Assert.AreEqual(-100, gameController.markedSpaces[0]);
    }

    [Test]
    public void TestTicTacToeButton_MoveToFullMiniBoard()
    {
        gameManager.whoTurn = 0;
        gameManager.whichBoard = -1;
        gameManager.allActive = true;
        gameController.boardID = 0;

        for (int i = 0; i < gameController.markedSpaces.Length; i++)
        {
            gameController.markedSpaces[i] = 1;
            gameController.tictactoeSpaces[i].interactable = false;
        }

        gameController.TicTacToeButton(0);

        Assert.AreEqual(0, gameController.turnCount);
    }

}