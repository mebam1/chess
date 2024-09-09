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
    }
}
