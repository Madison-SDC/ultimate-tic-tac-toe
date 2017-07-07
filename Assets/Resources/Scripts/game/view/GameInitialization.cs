using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script goes on the same GameObject as the GlobalGameUI
/// Conditions for GameObject:
/// Has 9 children that have GameUIs
/// Each child has 9 children that are SpotUIs
/// Child 0 is top left
/// Children and grandchildren are ordered left to right, top to bottom
/// </summary>
class GameInitialization : MonoBehaviour
{
    GlobalGame globalGame;

    public bool menu;

    /// <summary>
    /// Create all scripts and set up their references to the UI
    /// </summary>
    private void Awake()
    {
        InitializeGames();
        InitializeButtons();
        InitializeText();
    }

    public void InitializeGames()
    {
        int spotUIIndex = 0;
        int localGameUIIndex = 1; // first GameUI is global UI
        SpotUI[] spotUIs = GetComponentsInChildren<SpotUI>();
        GameUI[] gameUIs = GetComponentsInChildren<GameUI>();
        LocalGame[,] localGames = new LocalGame[3, 3];

        for (int boardRow = 0; boardRow < 3; boardRow++)
        {
            for (int boardCol = 0; boardCol < 3; boardCol++)
            {
                // a grid of spots represents each local board
                Spot[,] spots = new Spot[3, 3];

                for (int spotRow = 0; spotRow < 3; spotRow++)
                {
                    for (int spotCol = 0; spotCol < 3; spotCol++)
                    {
                        // each spot
                        SpotUI spotUI = spotUIs[spotUIIndex];
                        Spot spot = new Spot(
                            new Location(spotRow, spotCol), null, true);

                        spotUI.Spot = spot;

                        spots[spotRow, spotCol] = spot;
                        spotUIIndex++;
                    }
                }

                // each local game
                Location loc = new Location(boardRow, boardCol);
                LocalGame localGame = new LocalGame(spots, true, loc);
                localGames[boardRow, boardCol] = localGame;
                gameUIs[localGameUIIndex].Game = localGame;
                localGameUIIndex++;
            }
        }

        Player p1 = menu ? Settings.NewPlayer(true, true) : Settings.p1;
        Player p2 = menu ? Settings.NewPlayer(false, true) : Settings.p2;
        
        // initialize the global game, its view and its controller
        GlobalGame game = new GlobalGame(localGames, true, p1, p2, true);

        if(p1 is AI) { ((AI)p1).Game = game; }
        if(p2 is AI) { ((AI)p2).Game = game; }

        GetComponent<GameUI>().Game = game;
        GetComponent<GameController>().Game = game;
        globalGame = game;
    }

    void InitializeButtons()
    {
        // Confirm button
        {
            GameObject confirmButton = GameObject.Find("Confirm Button");
            if (confirmButton != null)
            {
                ButtonView confirmView = confirmButton.GetComponent<ButtonView>();
                globalGame.CanConfirmChanged += confirmView.OnValueChanged;
                confirmView.OnValueChanged(globalGame, new BoolEventArgs(false));
            }
        }

        // Undo button
        {
            GameObject undoButton = GameObject.Find("Undo Button");
            if (undoButton != null)
            {
                ButtonView undoView = undoButton.GetComponent<ButtonView>();
                globalGame.CanUndoChanged += undoView.OnValueChanged;
                undoView.OnValueChanged(globalGame, new BoolEventArgs(false));
            }
        }

        // Redo button
        {
            GameObject redoButton = GameObject.Find("Redo Button");
            if (redoButton != null)
            {
                ButtonView redoView = redoButton.GetComponent<ButtonView>();
                globalGame.CanRedoChanged += redoView.OnValueChanged;
                redoView.OnValueChanged(globalGame, new BoolEventArgs(false));
            }
        }

        // Reset button
        {
            GameObject resetButton = GameObject.Find("Reset Button");
            if (resetButton != null)
            {
                ButtonView resetView = resetButton.GetComponent<ButtonView>();
                globalGame.CanUndoChanged += resetView.OnValueChanged;
                resetView.OnValueChanged(globalGame, new BoolEventArgs(false));
            }
        }
    }

    void InitializeText()
    {
        GameObject statusGo = GameObject.Find("Status Text");
        if (statusGo == null) { return; }
        StatusText statusText = statusGo.GetComponent<StatusText>();
        statusText.Game = globalGame;
    }
}
