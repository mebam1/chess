using System.Collections.Generic;
using UnityEngine;

public class PawnMovement : BaseMovement
{
    public PawnMovement(PieceColor color, int x, int y) : base(color, x, y) { }

    public override void UpdateMap()
    {
        System.Array.Clear(attackMap, 0, attackMap.Length);
        int yOffset = Color == PieceColor.WHITE ? 1 : -1;
    }
}
