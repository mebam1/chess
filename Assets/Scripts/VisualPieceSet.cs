using System;
using System.Collections.Generic;
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
    List<VisualPiece> visuals = new();
    Dictionary<Type, VisualPiece> prefabDict;

    public void Init(BaseMovement.PieceColor color)
    {
        InitPrefabDict();
        var pieceSet = ChessRule.Instance.GetPieceSet(color);
        kingVisual = GameObject.Instantiate(pKingVisual);
        pieceSet.King.BindVisualizer(kingVisual);
        visuals.Add(kingVisual);

        for(int i = 0;i< PieceSet.INIT_QUEEN_NUM; i++)
        {
            var visual = GameObject.Instantiate(pQueenVisual);
            visuals.Add(visual);
            var piece = pieceSet.GetList<QueenMovement>()[i];
            piece.BindVisualizer(visual);
        }

        for (int i = 0; i < PieceSet.INIT_PAWN_NUM; i++)
        {
            var visual = GameObject.Instantiate(pPawnVisual);
            visuals.Add(visual);
            var piece = pieceSet.GetList<PawnMovement>()[i];
            piece.BindVisualizer(visual);
        }

        for (int i = 0; i < PieceSet.INIT_BISHOP_NUM; i++)
        {
            var visual = GameObject.Instantiate(pBishopVisual);
            visuals.Add(visual);
            var piece = pieceSet.GetList<BishopMovement>()[i];
            piece.BindVisualizer(visual);
        }

        for (int i = 0; i < PieceSet.INIT_ROOK_NUM; i++)
        {
            var visual = GameObject.Instantiate(pRookVisual);
            visuals.Add(visual);
            var piece = pieceSet.GetList<RookMovement>()[i];
            piece.BindVisualizer(visual);
        }

        for (int i = 0; i < PieceSet.INIT_KNIGHT_NUM; i++)
        {
            var visual = GameObject.Instantiate(pKnightVisual);
            visuals.Add(visual);
            var piece = pieceSet.GetList<KnightMovement>()[i];
            piece.BindVisualizer(visual);
        }

        foreach (var v in visuals)
            v.OnDie += Remove;
    }

    void Remove(VisualPiece x) => visuals.Remove(x);

    void InitPrefabDict()
    {
        prefabDict = new()
        {
            { typeof(KingMovement), pKingVisual },
            { typeof(QueenMovement), pQueenVisual },
            { typeof(PawnMovement), pPawnVisual }, 
            { typeof(BishopMovement), pBishopVisual },
            { typeof(RookMovement), pRookVisual },
            { typeof(KnightMovement), pKnightVisual },
        };
    }

    public void StartNewTurn()
    {
        foreach (var v in visuals)
        {
            Debug.Assert(v != null);
            v.OnNewTurnStart();
        }
    }

    public void BindVisualizer(BaseMovement piece)
    {
        var prefab = prefabDict[piece.GetType()];
        var visual = GameObject.Instantiate(prefab);
        visuals.Add(visual);
        visual.OnDie += Remove;
        piece.BindVisualizer(visual);
    }
}
