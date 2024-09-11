using UnityEngine;
namespace Command
{
    public class Attack : IGameCommand
    {
        public event IGameCommand.DoHandler OnDo;
        BaseMovement attacker;
        BaseMovement victim;
        int toX, toY;
        int fromX, fromY;
        IGameCommand afterAttack; // this chained command does not invoke StartNewTurn.
        bool isFirstMove;
        public IGameCommand Last => afterAttack?.Last ?? this;

        public Attack(BaseMovement attacker, BaseMovement victim, IGameCommand afterAttack = null)
        {
            this.attacker = attacker;
            this.victim = victim;
            toX = victim.x;
            toY = victim.y;
            fromX = attacker.x;
            fromY = attacker.y;
            isFirstMove = attacker.IsFirstMove;
            this.afterAttack = afterAttack;
        }

        public string Info => $"[{attacker}] attacked [{victim}] ({toX}, {toY})";

        

        public void Do()
        {
            attacker.x = toX;
            attacker.y = toY;
            ChessRule.Instance.GetPieceSet(victim.Color).Remove(victim);
            victim.RemoveVisual();
            attacker.MoveVisual(new Vector2Int(toX, toY));
            isFirstMove = false;
            afterAttack?.Do();
            OnDo?.Invoke();
        }

        public void UnDo()
        {
            //throw new System.NotImplementedException();
            attacker.IsFirstMove = isFirstMove;
            // create victim visual at (toX, toY).
            afterAttack?.UnDo();
        }
    }
}
