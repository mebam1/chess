using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BishopMovement : LinearMovement
{
    public BishopMovement(PieceColor color, int x, int y) : base(color, x, y) { }
    readonly Vector2Int[] offsets = {
        new(1, -1), new(1, 1), new(-1, 1), new(-1, -1)
    };

    public override IReadOnlyList<Vector2Int> Offsets => offsets;
}
