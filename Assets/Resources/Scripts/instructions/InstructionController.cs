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
        InitializeInstructions();
        index = 0;
        currentInstruction = instructions[index];
        infoText = GameObject.Find("Info Text").GetComponent<Text>();
        infoText.text = currentInstruction.Info;
    }

    void InitializeInstructions()
    {
        instructions = new Instruction[12];

        instructions[0] = new Instruction(
            Game,
            "Ultimate Tic-Tac-Toe is a game with 81 spots",
            delegate () 
            {
                Game.Preview(
                    Random.Range(0, 3), 
                    Random.Range(0, 3), 
                    Random.Range(0, 3), 
                    Random.Range(0, 3)
                );
            },
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
}
