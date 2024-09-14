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
        IGameCommand afterAttack;
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
            isFirstMove = attacker.IsFirstMove;
            afterAttack?.Do();
            OnDo?.Invoke();
        }

        public void UnDo()
        {
            attacker.IsFirstMove = isFirstMove;
            attacker.x = fromX;
            attacker.y = fromY;
            attacker.SetVisualPositionImmediately(fromX, fromY);
            victim.x = toX;
            victim.y = toY;
            var pieceSet = ChessVisualizer.Instance.GetPieceSet(victim.Color);
            pieceSet.BindVisualizer(victim);
            afterAttack?.UnDo();
        }
    }
}
