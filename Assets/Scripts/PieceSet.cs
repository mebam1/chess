using System.Collections.Generic;
using UnityEngine;

public class PieceSet
{
    public const int INIT_QUEEN_NUM = 1;
    public const int INIT_PAWN_NUM = 8;
    public const int INIT_BISHOP_NUM = 2;
    public const int INIT_ROOK_NUM = 2;
    public const int INIT_KNIGHT_NUM = 2;

    BaseMovement king;
    List<BaseMovement> queens = new(INIT_QUEEN_NUM);
    List<BaseMovement> pawns = new(INIT_PAWN_NUM);
    List<BaseMovement> bishops = new(INIT_BISHOP_NUM);
    List<BaseMovement> rooks = new(INIT_ROOK_NUM);
    List<BaseMovement> knights = new(INIT_KNIGHT_NUM);

    public BaseMovement King => king;
    public IReadOnlyList<BaseMovement> Queens => queens;
    public IReadOnlyList<BaseMovement> Bishops => bishops;
    public IReadOnlyList<BaseMovement> Pawns => pawns;
    public IReadOnlyList<BaseMovement> Rooks => rooks;
    public IReadOnlyList<BaseMovement> Knights => knights;

    List<BaseMovement>[,] attackMap = new List<BaseMovement>[8, 8];
    BaseMovement[,] locationMap = new BaseMovement[8, 8];
    /// <summary>(pinner, attacker) 쌍</summary>
    List<(BaseMovement, BaseMovement)> kingPinners = new();
    public BaseMovement[,] LocationMap => locationMap;
    public IReadOnlyList<BaseMovement> GetAttackers(int x, int y) => attackMap[x, y];

    /// <summary>(pinner, attacker) 쌍</summary>
    public IReadOnlyList<(BaseMovement, BaseMovement)> KingPinners => kingPinners;

    public PieceSet(BaseMovement.PieceColor color)
    {
        for (int i = 0; i < 8; ++i)
            for (int j = 0; j < 8; ++j)
                attackMap[i, j] = new();
        int bottomY = GetProperY(0);
        int pawnY = GetProperY(1);
        king = new KingMovement(color, 4, bottomY);

        // TODO: concrete piece movement class implementation.
        for (int i = 0; i < INIT_PAWN_NUM; i++)
            pawns.Add(new BishopMovement(color, i, pawnY));
        queens.Add(new BishopMovement(color, 3, bottomY));
        bishops.Add(new BishopMovement(color, 2, bottomY));
        bishops.Add(new BishopMovement(color, 5, bottomY));
        rooks.Add(new BishopMovement(color, 0, bottomY));
        rooks.Add(new BishopMovement(color, 7, bottomY));
        knights.Add(new BishopMovement(color, 1, bottomY));
        knights.Add(new BishopMovement(color, 6, bottomY));
        int GetProperY(int yPos) => (color == BaseMovement.PieceColor.BLACK) ? (7 - yPos) : yPos;
    }

    public void UpdateMap()
    {
        kingPinners.Clear();
        king.UpdateMap();
        foreach (var queen in queens)
            queen.UpdateMap();
        foreach(var pawn in pawns)
            pawn.UpdateMap();
        foreach(var rook in rooks)
            rook.UpdateMap();
        foreach(var knight in knights)
            knight.UpdateMap();
        foreach(var bishop in bishops)
            bishop.UpdateMap();

        // queen, rook, bishop만 king pinner를 가질 수 있다.
        foreach (var queen in queens)
            if (queen.KingPinner != null)
                kingPinners.Add((queen.KingPinner, queen));

        foreach (var rook in rooks)
            if (rook.KingPinner != null)
                kingPinners.Add((rook.KingPinner, rook));

        foreach (var bishop in bishops)
            if (bishop.KingPinner != null)
                kingPinners.Add((bishop.KingPinner, bishop));


        for (int i = 0;i<8;++i)
        {
            for(int j = 0;j<8;++j)
            {
                attackMap[i, j].Clear();

                if (king.AttackMap[i, j])
                    attackMap[i, j].Add(king);
                foreach (var queen in queens)
                    if (queen.AttackMap[i, j])
                        attackMap[i, j].Add(queen);

                foreach (var pawn in pawns)
                    if (pawn.AttackMap[i, j])
                        attackMap[i, j].Add(pawn);

                foreach (var rook in rooks)
                    if (rook.AttackMap[i, j])
                        attackMap[i, j].Add(rook);

                foreach (var knight in knights)
                    if (knight.AttackMap[i, j])
                        attackMap[i, j].Add(knight);

                foreach (var bishop in bishops)
                    if (bishop.AttackMap[i, j])
                        attackMap[i, j].Add(bishop);
            }
        }
    }

    public void UpdateLocationMap()
    {
        System.Array.Clear(locationMap, 0, locationMap.Length);
        locationMap[king.x, king.y] = king;
        foreach (var queen in queens)
            locationMap[queen.x, queen.y] = queen;
        foreach (var pawn in pawns)
            locationMap[pawn.x, pawn.y] = pawn;
        foreach (var rook in rooks)
            locationMap[rook.x, rook.y] = rook;
        foreach (var knight in knights)
            locationMap[knight.x, knight.y] = knight;
        foreach (var bishop in bishops)
            locationMap[bishop.x, bishop.y] = bishop;
    }

    public void UpdateAvailables()
    {
        king.UpdateAvailables();
        foreach (var queen in queens)
            queen.UpdateAvailables();
        foreach (var pawn in pawns)
            pawn.UpdateAvailables();
        foreach (var rook in rooks)
            rook.UpdateAvailables();
        foreach (var knight in knights)
            knight.UpdateAvailables();
        foreach (var bishop in bishops)
            bishop.UpdateAvailables();
    }
}