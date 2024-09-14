using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QueenMovement : LinearMovement
{
    // used for ease of implementation.
    RookMovement rookInternal;
    BishopMovement bishopInternal;

    public QueenMovement(PieceColor color, int x, int y) : base(color, x, y)
    {
        rookInternal = new RookMovement(color, x, y);
        bishopInternal = new BishopMovement(color, x, y);
    }

    public override IReadOnlyList<Vector2Int> Offsets => rookInternal.Offsets.Concat(bishopInternal.Offsets).ToList();

    public override void UpdateMap()
    {
        kingPinner = null;
        System.Array.Clear(attackMap, 0, attackMap.Length);

        rookInternal.x = x;
        rookInternal.y = y;
        bishopInternal.x = x;
        bishopInternal.y = y;
        rookInternal.UpdateMap();
        bishopInternal.UpdateMap();

        for (int i = 0; i < 8; ++i)
            for (int j = 0; j < 8; ++j)
                attackMap[i, j] = rookInternal.AttackMap[i, j] || bishopInternal.AttackMap[i, j];
        kingPinner = rookInternal.KingPinner ?? bishopInternal.KingPinner;
    }
}
