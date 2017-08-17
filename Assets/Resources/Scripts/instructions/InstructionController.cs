using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InstructionController : GameController
{
    Instruction[] instructions;
    List<int[]> scriptedMoves;
    int miscIndex, instructionIndex, moveIndex;
    Instruction currentInstruction;
    Text infoText;
    Slider slider;

    private void Start()
    {
        DisableAllButtons();
        InitializeScriptedMoves();
        InitializeInstructions();

        infoText = GameObject.Find("Info Text").GetComponent<Text>();

        instructionIndex = -1;
        slider = GameObject.Find("Progress Slider").GetComponent<Slider>();
        miscIndex = 0;
        moveIndex = 0;
        Next();
    }

    /// <summary>
    /// Advance into the next instruction
    /// </summary>
    public void Next()
    {
        if (instructionIndex == instructions.Length - 1)
        {
            return; // cannot go next if there is no next instruction
        }

        if (currentInstruction != null)
        {
            currentInstruction.AdvanceOut();
        }
        instructionIndex++;
        slider.value = instructionIndex;

        currentInstruction = instructions[instructionIndex];
        currentInstruction.AdvanceIn();
        infoText.text = currentInstruction.Info;

        previewTimer = previewTime;
        confirmTimer = confirmTime;
    }

    void DisableAllButtons()
    {
        Button[] buttons = GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            button.enabled = false;
        }
    }

    void InitializeScriptedMoves()
    {
        scriptedMoves = new List<int[]>();
        scriptedMoves.Add(new int[] { 0, 1 });
        scriptedMoves.Add(new int[] { 0, 0 });
        scriptedMoves.Add(new int[] { 1, 1 });
        scriptedMoves.Add(new int[] { 1, 0 });
        scriptedMoves.Add(new int[] { 0, 0 });
        scriptedMoves.Add(new int[] { 2, 1 });
        scriptedMoves.Add(new int[] { 0, 0 });
        scriptedMoves.Add(new int[] { 1, 0 });
        scriptedMoves.Add(new int[] { 1, 0 });
        scriptedMoves.Add(new int[] { 2, 0 });
        scriptedMoves.Add(new int[] { 0, 0 });
        scriptedMoves.Add(new int[] { 0, 2 });
        scriptedMoves.Add(new int[] { 0, 0 });
        scriptedMoves.Add(new int[] { 0, 0 });
        scriptedMoves.Add(new int[] { 2, 0 });
        scriptedMoves.Add(new int[] { 0, 0 });
        scriptedMoves.Add(new int[] { 1, 2 });
        scriptedMoves.Add(new int[] { 0, 0 });
        scriptedMoves.Add(new int[] { 2, 2 });
        scriptedMoves.Add(new int[] { 2, 2 });
        scriptedMoves.Add(new int[] { 1, 1 });
        scriptedMoves.Add(new int[] { 2, 0 });
        scriptedMoves.Add(new int[] { 2, 2 });
        scriptedMoves.Add(new int[] { 0, 0 });
    }

    void InitializeInstructions()
    {
        instructions = new Instruction[12];

        instructions[0] = new Instruction(
            "Ultimate Tic-Tac-Toe is a game with 81 spots...",
            PreviewAll,
            delegate () { },
            delegate () { },
            delegate () { previewTime = 0.2f; },
            delegate () { }
        );

        instructions[1] = new Instruction(
            "...on nine local games.",
            PreviewRelative,
            delegate () { previewTime = 0.5f; },
            delegate () { },
            delegate () { },
            delegate () { }
        );

        instructions[2] = new Instruction(
            "Playing a given spot sends the next player to the " +
            "relative local game, outlined in that player's color...",
            BlinkPreviewTopLeftSpots,
            delegate () { Game.Preview(null); },
            delegate () { },
            delegate () { },
            delegate () { }
            );

        instructions[3] = new Instruction(
            "For example, X playing in the top-left spot of " +
            "any local game sends O to the top-left local game.",
            BlinkPreviewTopLeftSpots,
            delegate () { },
            delegate () { Game.Play(1, 1, 0, 0); },
            delegate () { },
            delegate () { }
        );

        instructions[4] = new Instruction(
            "Now O must play in the top-left local game, " +
            "sending X to a new game",
            PreviewTopLeftSpots,
            delegate () { },
            delegate () { },
            delegate () { },
            delegate () { }
        );

        instructions[5] = new Instruction(
            "Players take turns playing on any open spot.",
            PlayScriptedMoveOrRandom,
            delegate () { Game.Preview(null); },
            delegate () { },
            delegate () { },
            delegate () { }
        );
    }

    private void Update()
    {
        if (currentInstruction != null)
        {
            currentInstruction.Act();
        }
    }

    /// <summary>
    /// Preview a spot based on the current index
    /// Cycle through each spot in order, top to bottom, left to right
    /// </summary>
    void PreviewAll()
    {
        miscIndex %= 81;
        int boardRow = miscIndex / 27;
        int boardCol = (miscIndex / 9) % 3;
        int spotRow = (miscIndex / 3) % 3;
        int spotCol = miscIndex % 3;

        Game.Preview(boardRow, boardCol, spotRow, spotCol);

        IncrementIndex();
    }

    /// <summary>
    /// Preview each relative spot of each board in order
    /// Top-left spot of top-left board, top-mid spot of top-mid board, etc.
    /// To be called as instruction action
    /// </summary>
    void PreviewRelative()
    {
        miscIndex %= 9;
        int row = miscIndex / 3;
        int col = miscIndex % 3;
        Game.Preview(row, col, row, col);

        IncrementIndex();
    }

    /// <summary>
    /// Preview and play a random spot, 
    /// just as though two random AIs were playing
    /// </summary>
    void PlayScriptedMoveOrRandom()
    {
        if (Game.HasNextMove)
        {
            if (confirmTimer <= 0)
            {
                Game.Confirm();
                confirmTimer += confirmTime;
            }
            else
            {
                confirmTimer -= Time.deltaTime;
            }
        }
        else
        {
            if (previewTimer <= 0)
            {
                // preview next scripted move and increment
                if(0 > moveIndex || moveIndex >= scriptedMoves.Count)
                {
                    // play random
                }
                else
                {
                    int[] move = scriptedMoves[moveIndex];
                    Location loc = Game.ActiveGame.Loc;
                    Game.Preview(loc.Row, loc.Col, move[0], move[1]);
                    moveIndex++;
                }
                previewTimer += previewTime;
            }
            else
            {
                previewTimer -= Time.deltaTime;
            }
        }
    }

    /// <summary>
    /// Preview top left spot of each board
    /// </summary>
    void BlinkPreviewTopLeftSpots()
    {
        miscIndex %= 18;

        if (miscIndex % 2 == 1)
        {
            Game.Preview(null); // preview nothing to show disappearing outline
        }
        else
        {
            int row = miscIndex / 6;
            int col = (miscIndex / 2) % 3;
            Game.Preview(row, col, 0, 0); // preview top-left of each board
        }

        IncrementIndex();
    }

    void PreviewTopLeftSpots()
    {
        miscIndex %= 9;
        int row = miscIndex / 3;
        int col = miscIndex % 3;
        Game.Preview(0, 0, row, col);

        IncrementIndex();
    }

    void IncrementIndex()
    {
        if (previewTimer <= 0)
        {
            miscIndex++;
            previewTimer += previewTime;
        }
        else
        {
            previewTimer -= Time.deltaTime;
        }
    }
}
