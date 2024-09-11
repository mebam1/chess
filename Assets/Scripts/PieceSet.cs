using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PieceSet
{
    public const int INIT_QUEEN_NUM = 1;
    public const int INIT_PAWN_NUM = 0; //8;
    public const int INIT_BISHOP_NUM = 0; //2;
    public const int INIT_ROOK_NUM = 2;
    public const int INIT_KNIGHT_NUM = 0; //2;

    BaseMovement king;
    List<BaseMovement> pieces = new(1 + INIT_QUEEN_NUM + INIT_PAWN_NUM + INIT_BISHOP_NUM + INIT_ROOK_NUM + INIT_KNIGHT_NUM);

    public BaseMovement King => king;
    public IReadOnlyList<BaseMovement> GetList<T>() where T : BaseMovement => pieces.Where(x => x is T).ToList();

    public IReadOnlyList<BaseMovement> Pieces => pieces;
    int availableCount = 0;
    public int AvailableCount => availableCount;


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
        pieces.Add(king);
        //for (int i = 0; i < INIT_PAWN_NUM; i++)
            //pieces.Add(new PawnMovement(color, i, pawnY));
        pieces.Add(new QueenMovement(color, 3, bottomY));
        //pieces.Add(new BishopMovement(color, 2, bottomY));
        //pieces.Add(new BishopMovement(color, 5, bottomY));
        pieces.Add(new RookMovement(color, 0, bottomY));
        pieces.Add(new RookMovement(color, 7, bottomY));
        //pieces.Add(new KnightMovement(color, 1, bottomY));
        //pieces.Add(new KnightMovement(color, 6, bottomY));


        int GetProperY(int yPos) => (color == BaseMovement.PieceColor.BLACK) ? (7 - yPos) : yPos;
    }

    public void UpdateMap()
    {
        kingPinners.Clear();
        foreach(var p in pieces)
            p.UpdateMap();

        // 반드시 전체 UpdateMap을 수행한후에 KingPinner를 가져와야 한다
        foreach (var p in pieces)
            if (p.KingPinner != null)
                kingPinners.Add((p.KingPinner, p));

        for (int i = 0;i<8;++i)
        {
            for(int j = 0;j<8;++j)
            {
                attackMap[i, j].Clear();
                foreach(var p in pieces)
                    if (p.AttackMap[i, j])
                        attackMap[i, j].Add(p);
            }
        }
    }

    public void UpdateLocationMap()
    {
        System.Array.Clear(locationMap, 0, locationMap.Length);
        foreach (var p in pieces)
            locationMap[p.x, p.y] = p;
    }

    public void UpdateAvailables()
    {
        availableCount = 0;
        foreach (var p in pieces)
        {
            p.UpdateAvailables();
            availableCount += p.AvailableCount;
        }
    }

    public void Remove(BaseMovement piece)
    {
        Debug.Assert(piece != king); // 킹이 먹히기 전에 체크메이트로 패배한다. 따라서 킹은 직접 공격받을 수 없다.
        pieces.Remove(piece);
    }

    public void Add(BaseMovement piece)
    {
        Debug.Assert(piece != king);
        pieces.Add(piece);
    }
}