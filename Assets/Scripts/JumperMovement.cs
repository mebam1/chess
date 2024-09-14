using System.Collections.Generic;
using UnityEngine;

public abstract class JumperMovement : BaseMovement
{
    public JumperMovement(PieceColor color, int x, int y) : base(color, x, y) { }
    public abstract IReadOnlyList<Vector2Int> JumpOffsets { get; }

    public override void UpdateMap()
    {
        kingPinner = null;
        System.Array.Clear(attackMap, 0, attackMap.Length);

        foreach(var jump in JumpOffsets)
        {
            Vector2Int cell = new Vector2Int(x + jump.x, y + jump.y);
            if (IsInsideOfBoard(cell.x, cell.y))
                attackMap[cell.x, cell.y] = true;
        }
    }
}
