using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

[System.Serializable]
public class VisualPieceSet
{
    [SerializeField] VisualPiece pKingVisual;
    [SerializeField] VisualPiece pQueenVisual;
    [SerializeField] VisualPiece pPawnVisual;
    [SerializeField] VisualPiece pBishopVisual;
    [SerializeField] VisualPiece pRookVisual;
    [SerializeField] VisualPiece pKnightVisual;

    VisualPiece kingVisual;
    List<VisualPiece> queenVisual = new();
    List<VisualPiece> pawnVisual = new();
    List<VisualPiece> bishopVisual = new();
    List<VisualPiece> rookVisual = new();
    List<VisualPiece> knightVisual = new();

    public void Init(BaseMovement.PieceColor color)
    {
        var pieceSet = ChessRule.Instance.GetPieceSet(color);
        kingVisual = GameObject.Instantiate(pKingVisual);
        pieceSet.King.BindVisualizer(kingVisual);

        for(int i = 0;i< PieceSet.INIT_QUEEN_NUM; i++)
        {
            var visual = GameObject.Instantiate(pQueenVisual);
            queenVisual.Add(visual);
            var piece = pieceSet.Queens[i];
            piece.BindVisualizer(visual);
        }

        for (int i = 0; i < PieceSet.INIT_PAWN_NUM; i++)
        {
            var visual = GameObject.Instantiate(pPawnVisual);
            pawnVisual.Add(visual);
            var piece = pieceSet.Pawns[i];
            piece.BindVisualizer(visual);
        }

        for (int i = 0; i < PieceSet.INIT_BISHOP_NUM; i++)
        {
            var visual = GameObject.Instantiate(pBishopVisual);
            bishopVisual.Add(visual);
            var piece = pieceSet.Bishops[i];
            piece.BindVisualizer(visual);
        }

        for (int i = 0; i < PieceSet.INIT_ROOK_NUM; i++)
        {
            var visual = GameObject.Instantiate(pRookVisual);
            rookVisual.Add(visual);
            var piece = pieceSet.Rooks[i];
            piece.BindVisualizer(visual);
        }

        for (int i = 0; i < PieceSet.INIT_KNIGHT_NUM; i++)
        {
            var visual = GameObject.Instantiate(pKnightVisual);
            knightVisual.Add(visual);
            var piece = pieceSet.Knights[i];
            piece.BindVisualizer(visual);
        }


        //kingVisual.OnDie += x => kingVisual = null;

        /*
        foreach (var piece in pawnVisual)
            piece.OnDie += x => pawnVisual.Remove(x);
        foreach (var piece in bishopVisual)
            piece.OnDie += x => bishopVisual.Remove(x);
        foreach (var piece in rookVisual)
            piece.OnDie += x => rookVisual.Remove(x);
        foreach (var piece in knightVisual)
            piece.OnDie += x => knightVisual.Remove(x);
        */
    }

    public void StartNewTurn()
    {
        kingVisual.OnNewTurnStart();
        queenVisual.ForEach(x => x.OnNewTurnStart());
        pawnVisual.ForEach(x => x.OnNewTurnStart());
        bishopVisual.ForEach(x => x.OnNewTurnStart());
        rookVisual.ForEach(x => x.OnNewTurnStart());
        knightVisual.ForEach(x => x.OnNewTurnStart());
    }
}
