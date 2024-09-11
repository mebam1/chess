using System.Collections.Generic;
using UnityEngine;

public class ChessRule : Singleton<ChessRule>
{
    // enum PieceColor is { WHITE = 0, BLACK = 1 }
    BaseMovement.PieceColor turn = BaseMovement.PieceColor.BLACK;
    PieceSet[] pieceSets = new PieceSet[2] { new(BaseMovement.PieceColor.WHITE), new(BaseMovement.PieceColor.BLACK) };
    BaseMovement[,] locationMap = new BaseMovement[8, 8];
    public BaseMovement[,] LocationMap => locationMap;
    public PieceSet GetPieceSet(BaseMovement.PieceColor color) => pieceSets[(int)color];
    public PieceSet GetOppositePieceSet(BaseMovement.PieceColor color) => pieceSets[((int)color + 1) % 2];
    public BaseMovement.PieceColor Turn => turn;
    public enum GameWinner { WHITE, BLACK, DRAW }

    public delegate void GameEndHandler(GameWinner winner);
    public event GameEndHandler OnGameEnded;


    protected override void Init()
    {
        base.Init();
        shouldDestroyOnSceneChange = true;
    }


    public void UpdateMap()
    {
        pieceSets[0].UpdateLocationMap();
        pieceSets[1].UpdateLocationMap();
        for (int i = 0; i < 8; ++i)
            for(int j = 0;j<8;++j)
                locationMap[i, j] = pieceSets[0].LocationMap[i, j] ?? pieceSets[1].LocationMap[i, j];

        pieceSets[0].UpdateMap();
        pieceSets[1].UpdateMap();

        pieceSets[0].UpdateAvailables();
        pieceSets[1].UpdateAvailables();
    }

    public void StartNewTurn()
    {
        turn = BaseMovement.PieceColor.WHITE == turn ? BaseMovement.PieceColor.BLACK : BaseMovement.PieceColor.WHITE;
        UpdateMap();
        var turnPlayer = GetPieceSet(turn);
        var opposite = GetOppositePieceSet(turn);
        var king = turnPlayer.King;
        int checker = opposite.GetAttackers(king.x, king.y).Count;

        if (turnPlayer.AvailableCount == 0)
        {
            GameWinner winner;
            if (checker == 0) // 스테일메이트 -> 무승부
                winner = GameWinner.DRAW;
            else // 체크메이트 -> turnPlayer가 아닌 상대방의 승리
                winner = turn == BaseMovement.PieceColor.BLACK ? GameWinner.WHITE : GameWinner.BLACK;
            OnGameEnded?.Invoke(winner);
        }
    }

    
}
