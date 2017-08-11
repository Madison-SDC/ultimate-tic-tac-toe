using UnityEngine;
using UnityEngine.UI;

public class InstructionController : GameController
{
    Instruction[] instructions;
    int index;
    Instruction currentInstruction;
    Text infoText;

    private void Start()
    {
        DisableAllButtons();
        InitializeInstructions();
        index = 0;
        currentInstruction = instructions[index];
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
}
