using System.Collections.Generic;
using UnityEngine;

public abstract class LinearMovement : BaseMovement
{
    public LinearMovement(PieceColor color, int x, int y) : base(color, x, y) { }
    public abstract IReadOnlyList<Vector2Int> Offsets { get; }

    public override void UpdateMap()
    {
        kingPinner = null;
        System.Array.Clear(attackMap, 0, attackMap.Length);

        for (int i = 0; i < Offsets.Count; i++)
        {
            bool passed = false;
            int xPos = x;
            int yPos = y;
            BaseMovement pinnerCandidate = null;

            while (true)
            {
                xPos += Offsets[i].x;
                yPos += Offsets[i].y;

                if (!IsInsideOfBoard(xPos, yPos)) break;
                var encountered = ChessRule.Instance.LocationMap[xPos, yPos];

                if (encountered == null)
                {
                    if (!passed)
                        attackMap[xPos, yPos] = true;
                }
                else
                {
                    if (!passed)
                    {
                        attackMap[xPos, yPos] = true;
                        passed = true;
                        if (encountered is not KingMovement)
                        {
                            // King은 그대로 무시하고 통과한다.
                            pinnerCandidate = encountered;
                        }
                    }
                    else
                    {
                        if (encountered is KingMovement && encountered.Color == pinnerCandidate.Color)
                            kingPinner = pinnerCandidate;
                        break;
                    }
                }
            }
        }
    }
}
