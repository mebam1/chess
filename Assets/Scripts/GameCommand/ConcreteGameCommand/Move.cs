using UnityEngine;
namespace Command
{
    public class Move : IGameCommand
    {
        BaseMovement mover;
        int toX, toY;
        int fromX, fromY;

        public Move(BaseMovement mover, int x, int y)
        {
            this.mover = mover;
            fromX = mover.x;
            fromY = mover.y;
            toX = x;
            toY = y;
        }

        public string Info => $"[{mover}] moved to ({toX}, {toY})";

        public void Do()
        {
            mover.x = toX;
            mover.y = toY;
            mover.MoveVisual(new Vector2Int(toX, toY));
        }

        public void UnDo()
        {
            mover.x = fromX;
            mover.y = fromY;
            mover.SetVisualPositionImmediately(fromX, fromY);
        }
    }
}
