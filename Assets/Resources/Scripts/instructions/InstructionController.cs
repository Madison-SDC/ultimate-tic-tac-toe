using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InstructionController : GameController
{
    Instruction[] instructions;
    int instructionIndex;
    Instruction currentInstruction;
    Text infoText;
    int index; // for miscellaneous cycles
    Slider slider;

    private void Start()
    {
        DisableAllButtons();
        InitializeInstructions();

        infoText = GameObject.Find("Info Text").GetComponent<Text>();

        instructionIndex = -1;
        slider = GameObject.Find("Progress Slider").GetComponent<Slider>();
        index = 0;
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
            "...For example, Player X playing in the top-left spot of " +
            "any local game sends Player O to the top-left local game",
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
        index %= 81;
        int boardRow = index / 27;
        int boardCol = (index / 9) % 3;
        int spotRow = (index / 3) % 3;
        int spotCol = index % 3;

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
        index %= 9;
        int row = index / 3;
        int col = index % 3;
        Game.Preview(row, col, row, col);

        IncrementIndex();
    }

    /// <summary>
    /// Preview and play a random spot, 
    /// just as though two random AIs were playing
    /// </summary>
    void PlayRandom()
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
                // preview random spot
                List<Spot> spots = Game.AvailableSpots;
                if (spots.Count > 0)
                {
                    Game.Preview(spots[Random.Range(0, spots.Count)]);
                }
                else
                {
                    previewTimer = float.MaxValue; // no more checks
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
        index %= 18;

        if (index % 2 == 1)
        {
            Game.Preview(null); // preview nothing
        }
        else
        {
            int row = index / 6;
            int col = (index / 2) % 3;
            Game.Preview(row, col, 0, 0); // preview top-left of each board
        }

        IncrementIndex();
    }

    void PreviewTopLeftSpots()
    {
        index %= 9;
        int row = index / 3;
        int col = index % 3;
        Game.Preview(0, 0, row, col);

        IncrementIndex();
    }

    void IncrementIndex()
    {
        if (previewTimer <= 0)
        {
            index++;
            previewTimer += previewTime;
        }
        else
        {
            previewTimer -= Time.deltaTime;
        }
    }
}
