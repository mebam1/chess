using Command;
using System.Collections.Generic;
using UnityEngine;

public class KingMovement : BaseMovement
{
    public KingMovement(PieceColor color, int x, int y) : base(color, x, y) { }
    public Vector2Int[] Offsets =
    {
        new(1, 0), new(1, 1), new(0, 1), new(-1, 1), new(-1, 0), new(-1, -1), new(0, -1), new(1, -1)
    };


    public override void UpdateMap()
    {
        System.Array.Clear(attackMap, 0, attackMap.Length);

        foreach(var offset in Offsets)
        {
            int xPos = offset.x + x, yPos = offset.y + y;
            if (IsInsideOfBoard(xPos, yPos))
                attackMap[xPos, yPos] = true;
        }
    }

    public override void UpdateAvailables()
    {
        availables.Clear();
        availableCommands.Clear();

        var opposite = ChessRule.Instance.GetOppositePieceSet(Color);
        var friendly = ChessRule.Instance.GetPieceSet(Color);

        foreach (var offset in Offsets)
        {
            int xPos = offset.x + x, yPos = offset.y + y;
            if (IsInsideOfBoard(xPos, yPos) && friendly.LocationMap[xPos, yPos] == null && opposite.GetAttackers(xPos, yPos).Count == 0)
                AddMoveOrAttackCommand(xPos, yPos, opposite, friendly);
        }

        /*
        // 캐슬링 조건 1. 왕이 이전에 움직인 적 없을 것
        
        if (IsFirstMove)
        {
            foreach(var rook in friendly.Rooks)
            {
                // 캐슬링 조건 2. 캐슬링 대상이 될 룩은 이전에 움직인 적 없을 것.
                // 캐슬링 조건 3. 캐슬링 대상이 될 룩은 왕과 같은 행에 있을 것.
                if(rook != null && rook.IsFirstMove && rook.y == y)
                {
                    bool hasThreaten = false;
                    bool hasBlockers = false;
                    bool isKingSideCastling = x < rook.x;
                    int xOffset = isKingSideCastling ? 1 : -1;

                    // 캐슬링 조건 4. 왕의 위치로부터 룩 방향으로 0, 1, 2칸만큼 떨어진 칸이 공격당하고 있지 않을 것
                    // (참고) 룩은 공격받고 있어도 캐슬링이 가능하다.
                    for (int i = 0;i<=2;++i)
                    {
                        int castlingKingPath = xOffset * i + x;
                        if (opposite.GetAttackers(castlingKingPath, y).Count > 0)
                        {
                            hasThreaten = true;
                            break;
                        }
                    }
                    
                    // 캐슬링 조건 5. 룩과 왕 사이에 아무 기물도 없을 것.
                    for(int i = 1;i<Mathf.Abs(rook.x - x);++i)
                    {
                        int rookKingLine = xOffset * i + x;
                        if (ChessRule.Instance.LocationMap[rookKingLine, y] != null)
                        {
                            hasBlockers = true;
                            break;
                        }
                    }

                    if (!hasThreaten && !hasBlockers)
                    {
                        // 캐슬링 조건 만족.
                        var cell = new Vector2Int(x + xOffset * 2, y);

                        IGameCommand command = new Castling
                            (
                                new Move(this, cell.x, y),
                                new Move(this, rook.x - xOffset * (isKingSideCastling ? 2 : 3), y),
                                isKingSideCastling
                            );

                        availables.Add(cell);
                        availableCommands.Add(cell, command);
                    }
                }
            }
        }
        */
    }
}
