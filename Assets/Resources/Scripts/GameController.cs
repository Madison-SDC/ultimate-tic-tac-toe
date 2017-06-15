using UnityEngine;

public class GameController : MonoBehaviour
{
    GlobalGame game;
    public float previewTime, previewTimer, confirmTime, confirmTimer;

    public GlobalGame Game
    {
        get { return game; }
        set { game = value; }
    }

    public void Confirm()
    {
        game.Confirm();
    }

    public void Undo()
    {
        game.Undo();
    }

    public void Redo()
    {
        game.Redo();
    }

    public void Reset()
    {
        game.Reset();
    }

    void Update()
    {
        if (game.ActivePlayer() is AI)
        {
            if(game.GameOver() && game.P1 is AI && game.P2 is AI)
            {
                if (Game.Winner != null)
                {
                    game.Reset();
                }
                return;
            }

            if(game.HasNextMove)
            {
                if (confirmTimer <= 0)
                {
                    Game.Confirm();
                    confirmTimer = confirmTime;
                    return;
                }
                else
                {
                    confirmTimer -= Time.deltaTime;
                    return;
                }
            }
            else
            {
                if (previewTimer <= 0)
                {
                    Game.Preview(((AI)game.ActivePlayer()).BestMove());
                    previewTimer = previewTime;
                    return;
                }
                else
                {
                    previewTimer -= Time.deltaTime;
                    return;
                }
            }
        }
        else
        {
            confirmTimer = confirmTime;
            previewTimer = previewTime;
        }
    }
}
