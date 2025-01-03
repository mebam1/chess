﻿using Command;
using System.Collections.Generic;
using System.Linq;
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
        kingPinner = null;
        System.Array.Clear(attackMap, 0, attackMap.Length);

        foreach (var offset in Offsets)
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
        //var attackers = opposite.GetAttackers(x, y);

        foreach (var offset in Offsets)
        {
            int xPos = offset.x + x, yPos = offset.y + y;
            if (IsInsideOfBoard(xPos, yPos)
                && friendly.LocationMap[xPos, yPos] == null
                && opposite.GetAttackers(xPos, yPos).Count == 0)
                availables.Add(new Vector2Int(xPos, yPos));
        }


        /*
        if (attackers.Count > 0)
        {
            foreach (var attacker in attackers)
            {
                for (int i = availables.Count - 1; i >= 0; --i)
                {
                    var a = availables[i];
                    var pinLine = GetLine(a.x, a.y, attacker.x, attacker.y);
                    var cell = new Vector2Int(x, y);
                    if (pinLine.Contains(cell))
                        availables.Remove(a);
                }
            }
        }
        */

        foreach (var available in availables)
        {
            IGameCommand cmd = opposite.LocationMap[available.x, available.y] == null ?
                new Move(this, available.x, available.y) :
                new Attack(this, opposite.LocationMap[available.x, available.y]);
            availableCommands.Add(available, cmd);
        }


        // 캐슬링 조건 1. 왕이 이전에 움직인 적 없을 것

        if (IsFirstMove)
        {
            foreach (var rook in friendly.GetList<RookMovement>())
            {
                // 캐슬링 조건 2. 캐슬링 대상이 될 룩은 이전에 움직인 적 없을 것.
                // 캐슬링 조건 3. 캐슬링 대상이 될 룩은 왕과 같은 행에 있을 것.
                if (rook != null && rook.IsFirstMove && rook.y == y)
                {
                    bool hasThreaten = false;
                    bool hasBlockers = false;
                    bool isKingSideCastling = x < rook.x;
                    int xOffset = isKingSideCastling ? 1 : -1;

                    // 캐슬링 조건 4. 왕의 위치로부터 룩 방향으로 0, 1, 2칸만큼 떨어진 칸이 공격당하고 있지 않을 것
                    // (참고) 룩은 공격받고 있어도 캐슬링이 가능하다.
                    for (int i = 0; i <= 2; ++i)
                    {
                        int castlingKingPath = xOffset * i + x;
                        if (opposite.GetAttackers(castlingKingPath, y).Count > 0)
                        {
                            hasThreaten = true;
                            break;
                        }
                    }

                    // 캐슬링 조건 5. 룩과 왕 사이에 아무 기물도 없을 것.
                    for (int i = 1; i < Mathf.Abs(rook.x - x); ++i)
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
                        var castling = new Move(this, cell.x, y, new Move(rook, rook.x - xOffset * (isKingSideCastling ? 2 : 3), y));
                        availables.Add(cell);
                        availableCommands.Add(cell, castling);
                    }
                }
            }
        }
    }
}