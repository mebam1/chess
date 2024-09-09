using UnityEngine;
namespace Command
{
    public class Castling : IGameCommand
    {
        Move kingMove;
        Move rookMove;
        bool isKingSide;

        public Castling(Move kingMove, Move rookMove, bool isKingSide)
        {
            this.kingMove = kingMove;
            this.rookMove = rookMove;
            this.isKingSide = isKingSide;
        }

        public string Info => $"[Castling {(isKingSide ? "0-0" : "0-0-0")}] {kingMove.Info} + {rookMove.Info}";

        public void Do()
        {
            kingMove.Do();
            rookMove.Do();
        }

        public void UnDo()
        {
            kingMove.UnDo();
            rookMove.UnDo();
        }
    }
}
