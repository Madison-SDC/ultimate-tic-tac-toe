using UnityEngine;
using UnityEngine.UI;

public class InstructionController : GameController
{
    Instruction[] instructions;
    int instructionIndex;
    Instruction currentInstruction;
    Text infoText;
    int index; // for miscellaneous cycles

    private void Start()
    {
        DisableAllButtons();
        InitializeInstructions();
        instructionIndex = 0;
        currentInstruction = instructions[instructionIndex];
        infoText = GameObject.Find("Info Text").GetComponent<Text>();
        infoText.text = currentInstruction.Info;
    }

    void DisableAllButtons()
    {
        Button[] buttons = GetComponentsInChildren<Button>();
        foreach(Button button in buttons)
        {
            button.enabled = false;
        }
    }

    void InitializeInstructions()
    {
        instructions = new Instruction[12];

        instructions[0] = new Instruction(
            Game,
            "Ultimate Tic-Tac-Toe is a game with 81 spots",
            PreviewRandom,
            delegate () { },
            delegate () { },
            delegate () { },
            delegate () { }
        );

        instructions[1] = new Instruction(
            Game,
            "On nine local boards",
            PreviewRelative,
            delegate () { },
            delegate () { },
            delegate () { },
            delegate () { }
        );
    }

    private void Update()
    {
        currentInstruction.Act();
    }

    /// <summary>
    /// Change game to preview a random spot based on previewTime
    /// Or decrement timer
    /// To be called continuously as an instruction action
    /// </summary>
    void PreviewRandom()
    {
        if(previewTimer <= 0)
        {
            Game.Preview(
                Random.Range(0, 3),
                Random.Range(0, 3),
                Random.Range(0, 3),
                Random.Range(0, 3)
            );
            previewTimer = previewTime;
        }
        else
        {
            previewTimer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Preview each relative spot of each board in order
    /// Top-left spot of top-left board, top-mid spot of top-mid board, etc.
    /// To be called as instruction action
    /// </summary>
    void PreviewRelative()
    {
        index %= 9;
        int row, col;
        row = index / 3;
        col = index % 3;
        Game.Preview(row, col, row, col);

        if(previewTimer <= 0)
        {
            index++;
            previewTimer = previewTime;
        }
        else
        {
            previewTimer -= Time.deltaTime;
        }
    }
}
