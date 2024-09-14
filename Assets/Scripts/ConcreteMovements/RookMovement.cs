using System.Collections.Generic;
using UnityEngine;

public class RookMovement : LinearMovement
{
    public RookMovement(PieceColor color, int x, int y) : base(color, x, y) { }
    readonly Vector2Int[] offsets = {
        new(1, 0), new(0, 1), new(-1, 0), new(0, -1)
    };
    public override IReadOnlyList<Vector2Int> Offsets => offsets;
}
