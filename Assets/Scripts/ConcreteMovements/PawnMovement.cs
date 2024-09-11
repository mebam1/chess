using Command;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PawnMovement : BaseMovement
{
    public PawnMovement(PieceColor color, int x, int y) : base(color, x, y) { }
    bool enPassable = false;
    bool prevIsFirstMove = true;

    public override void UpdateMap()
    {
        kingPinner = null;
        System.Array.Clear(attackMap, 0, attackMap.Length);
        int yOffset = (Color == PieceColor.WHITE) ? 1 : -1;
        var opposite = ChessRule.Instance.GetPieceSet(Color);

        // 왼쪽 위 + 오른쪽 위
        if (IsInsideOfBoard(x - 1, y + yOffset))
            attackMap[x - 1, y + yOffset] = true;
        if (IsInsideOfBoard(x + 1, y + yOffset))
            attackMap[x + 1, y + yOffset] = true;

        // IsFirstMove가 true -> false로밖에 변하지 않기 때문에,
        // 직전 턴에 이동을 수행한 경우에만 참이 된다.
        enPassable = !IsFirstMove && prevIsFirstMove; 
        prevIsFirstMove = IsFirstMove;
    }

    public override void UpdateAvailables()
    {
        //throw new System.NotImplementedException();
        availables.Clear();
        availableCommands.Clear();
        int yUB = (Color == PieceColor.WHITE) ? 7 : 0; // y Upper Bound.
        int yOffset = (Color == PieceColor.WHITE) ? 1 : -1; // y move dir.
        var opposite = ChessRule.Instance.GetOppositePieceSet(Color);
        var friendly = ChessRule.Instance.GetPieceSet(Color);
        int kx = friendly.King.x, ky = friendly.King.y;
        var kingAttackers = opposite.GetAttackers(kx, ky);

        Vector2Int front = new Vector2Int(x, y + yOffset);
        Vector2Int frontLeft = new Vector2Int(x - 1, y + yOffset);
        Vector2Int frontRight = new Vector2Int(x + 1, y + yOffset);
        Vector2Int frontFront = new Vector2Int(x, y + yOffset * 2);
        bool isFrontEmpty = IsInsideOfBoard(front.x, front.y) && ChessRule.Instance.LocationMap[front.x, front.y] == null;

        if (kingAttackers.Count == 0)
        {
            var (_, thisAttacker) = opposite.KingPinners.FirstOrDefault(x => x.Item1 == this);
            if (thisAttacker == null) // 내가 pinner가 아닌 경우.
            {
                if (isFrontEmpty)
                {
                    availables.Add(front);
                    Promotion promotionCommand = (front.y == yUB) ? new Promotion(this, front.x, front.y, x, y) : null;
                    availableCommands.Add(front, new Move(this, front.x, front.y, promotionCommand));
                }

                if(isFrontEmpty && IsFirstMove && IsInsideOfBoard(frontFront.x, frontFront.y) && ChessRule.Instance.LocationMap[frontFront.x, frontFront.y] == null)
                {
                    availables.Add(frontFront);
                    Promotion promotionCommand = (frontFront.y == yUB) ? new Promotion(this, frontFront.x, frontFront.y, x, y) : null;
                    availableCommands.Add(frontFront, new Move(this, frontFront.x, frontFront.y, promotionCommand));
                }

                // 대각선 공격. 
                CheckDiagonalAttack_InsideBoard(frontLeft);
                CheckDiagonalAttack_InsideBoard(frontRight);
            }
            else // 내가 pinner인 경우.
            {
                var pinLine = GetLine(kx, ky, thisAttacker.x, thisAttacker.y);
                if (isFrontEmpty && pinLine.Contains(front))
                {
                    availables.Add(front);
                    Promotion promotionCommand = (front.y == yUB) ? new Promotion(this, front.x, front.y, x, y) : null;
                    availableCommands.Add(front, new Move(this, front.x, front.y, promotionCommand));
                }

                if(isFrontEmpty && IsFirstMove && pinLine.Contains(frontFront) && ChessRule.Instance.LocationMap[frontFront.x, frontFront.y] == null)
                {
                    availables.Add(frontFront);
                    Promotion promotionCommand = (frontFront.y == yUB) ? new Promotion(this, frontFront.x, frontFront.y, x, y) : null;
                    availableCommands.Add(frontFront, new Move(this, front.x, front.y, promotionCommand));
                }

                CheckDiagonalAttack_LineContains(frontLeft, pinLine);
                CheckDiagonalAttack_LineContains(frontRight, pinLine);
            }
        }
        else if(kingAttackers.Count == 1)
        {
            BaseMovement kingAttacker = kingAttackers[0];
            var (_, thisAttacker) = opposite.KingPinners.FirstOrDefault(x => x.Item1 == this);
            var checkLine = GetLine(kx, ky, kingAttacker.x, kingAttacker.y);

            if (thisAttacker == null) // 내가 pinner가 아닌 경우.
            {
                if (isFrontEmpty && checkLine.Contains(front))
                {
                    availables.Add(front);
                    Promotion promotionCommand = (front.y == yUB) ? new Promotion(this, front.x, front.y, x, y) : null;
                    availableCommands.Add(front, new Move(this, front.x, front.y, promotionCommand));
                }

                if(isFrontEmpty && IsFirstMove && checkLine.Contains(frontFront) && ChessRule.Instance.LocationMap[frontFront.x, frontFront.y] == null)
                {
                    availables.Add(frontFront);
                    Promotion promotionCommand = (frontFront.y == yUB) ? new Promotion(this, frontFront.x, frontFront.y, x, y) : null;
                    availableCommands.Add(frontFront, new Move(this, frontFront.x, frontFront.y, promotionCommand));
                }

                CheckDiagonalAttack_LineContains(frontLeft, checkLine);
                CheckDiagonalAttack_LineContains(frontRight, checkLine);
            }
            else
            {
                var pinLine = GetLine(kx, ky, thisAttacker.x, thisAttacker.y);
                pinLine.Remove(new Vector2Int(x, y));
                var intersect = pinLine.Intersect(checkLine).ToList();

                if (isFrontEmpty && intersect.Contains(front))
                {
                    availables.Add(front);
                    Promotion promotionCommand = (front.y == yUB) ? new Promotion(this, front.x, front.y, x, y) : null;
                    availableCommands.Add(front, new Move(this, front.x, front.y, promotionCommand));
                }

                if (isFrontEmpty && IsFirstMove && intersect.Contains(frontFront) && ChessRule.Instance.LocationMap[frontFront.x, frontFront.y] == null)
                {
                    availables.Add(frontFront);
                    Promotion promotionCommand = (frontFront.y == yUB) ? new Promotion(this, frontFront.x, frontFront.y, x, y) : null;
                    availableCommands.Add(frontFront, new Move(this, frontFront.x, frontFront.y, promotionCommand));
                }

                CheckDiagonalAttack_LineContains(frontLeft, intersect);
                CheckDiagonalAttack_LineContains(frontRight, intersect);
            }
        }

        void CheckDiagonalAttack_InsideBoard(Vector2Int pos)
        {
            if (IsInsideOfBoard(pos.x, pos.y))
            {
                Promotion promotionCommand = (pos.y == yUB) ? new Promotion(this, pos.x, pos.y, x, y) : null;
                if (opposite.LocationMap[pos.x, pos.y] != null)
                {
                    availables.Add(pos);
                    availableCommands.Add(pos, new Attack(this, opposite.LocationMap[pos.x, pos.y], promotionCommand));
                }
                else if (opposite.LocationMap[pos.x, y] is PawnMovement enPassantTarget && enPassantTarget.enPassable)
                {
                    // en passant
                    availables.Add(pos);
                    availableCommands.Add(pos, new Attack(this, enPassantTarget, new Move(this, pos.x, pos.y, promotionCommand)));
                }
            }
        }

        void CheckDiagonalAttack_LineContains(Vector2Int pos, List<Vector2Int> line)
        {
            if (line.Contains(pos))
            {
                Promotion promotionCommand = (pos.y == yUB) ? new Promotion(this, pos.x, pos.y, x, y) : null;
                if (opposite.LocationMap[pos.x, pos.y] != null)
                {
                    availables.Add(pos);
                    availableCommands.Add(pos, new Attack(this, opposite.LocationMap[pos.x, pos.y], promotionCommand));
                }
                else if (opposite.LocationMap[pos.x, y] is PawnMovement enPassantTarget && enPassantTarget.enPassable)
                {
                    // en passant
                    availables.Add(pos);
                    availableCommands.Add(pos, new Attack(this, enPassantTarget, new Move(this, pos.x, pos.y, promotionCommand)));
                }
            }
        }
    }


}
