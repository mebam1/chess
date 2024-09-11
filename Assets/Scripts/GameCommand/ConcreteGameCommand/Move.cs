using System;
using UnityEngine;
namespace Command
{
    public class Move : IGameCommand
    {
        public event IGameCommand.DoHandler OnDo;
        BaseMovement mover;
        int toX, toY;
        int fromX, fromY;
        IGameCommand afterMove;
        bool isFirstMove;
        public IGameCommand Last => afterMove?.Last ?? this;

        public Move(BaseMovement mover, int x, int y, IGameCommand afterMove = null)
        {
            this.mover = mover;
            fromX = mover.x;
            fromY = mover.y;
            toX = x;
            toY = y;
            isFirstMove = mover.IsFirstMove;
            this.afterMove = afterMove;
        }

        public string Info => $"[{mover}] moved to ({toX}, {toY})";        

        public void Do()
        {
            mover.x = toX;
            mover.y = toY;
            mover.MoveVisual(new Vector2Int(toX, toY));
            mover.IsFirstMove = false;
            afterMove?.Do();
            OnDo?.Invoke();
        }

        public void UnDo()
        {
            mover.x = fromX;
            mover.y = fromY;
            mover.SetVisualPositionImmediately(fromX, fromY);
            mover.IsFirstMove = isFirstMove;
            afterMove?.UnDo();
        }
    }
}
