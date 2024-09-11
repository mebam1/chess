using System.Collections.Generic;
using UnityEngine;

public class KnightMovement : JumperMovement
{
    public KnightMovement(PieceColor color, int x, int y) : base(color, x, y) { }
    readonly Vector2Int[] offset =
    {
        new(2, 1),
        new(1, 2),
        new(-1, 2),
        new(-2, 1),
        new(-2, -1),
        new(-1, -2),
        new(1, -2),
        new(2, -1)
    };

    protected override IReadOnlyList<Vector2Int> JumpOffsets => offset;
}
