using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BishopMovement : BaseMovement
{
    public BishopMovement(PieceColor color, int x, int y) : base(color, x, y) { }
    Vector2Int[] offsets =
{
        new(1, -1), new(1, 1), new(-1, 1), new(-1, -1)
    };

    public override void UpdateMap()
    {
        kingPinner = null;
        System.Array.Clear(attackMap, 0, attackMap.Length);

        for (int i = 0;i< offsets.Length; i++)
        {
            bool passed = false;
            int xPos = x;
            int yPos = y;
            BaseMovement pinnerCandidate = null;

            while(true)
            {
                xPos += offsets[i].x;
                yPos += offsets[i].y;

                if (!IsInsideOfBoard(xPos, yPos)) break;
                var encountered = ChessRule.Instance.LocationMap[xPos, yPos];
                if (encountered == null)
                {
                    if (!passed)
                        attackMap[xPos, yPos] = true;
                }
                else
                {
                    if(!passed)
                    {
                        attackMap[xPos, yPos] = encountered.Color != Color; // 적이면 상대 기물위치까지 공격맵에 포함시킨다.
                        passed = true;
                        pinnerCandidate = encountered;
                    }
                    else
                    {
                        if (encountered is KingMovement && encountered.Color != Color)
                        {
                            kingPinner = pinnerCandidate;
                            Debug.Log($"KINGPINNER = ({kingPinner.x}, {kingPinner.y})");
                        }

                        break;
                    }
                }
            }
        }
    }
}
