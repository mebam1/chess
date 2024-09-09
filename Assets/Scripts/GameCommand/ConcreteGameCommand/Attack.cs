using UnityEngine;
namespace Command
{
    public class Attack : IGameCommand
    {
        BaseMovement attacker;
        BaseMovement victim;
        int toX, toY;
        int fromX, fromY;

        public Attack(BaseMovement attacker, BaseMovement victim)
        {
            this.attacker = attacker;
            this.victim = victim;
            toX = victim.x;
            toY = victim.y;
            fromX = attacker.x;
            fromY = attacker.y;
        }

        public string Info => $"[{attacker}] attacked [{victim}] ({toX}, {toY})";

        public void Do()
        {
            throw new System.NotImplementedException();
        }

        public void UnDo()
        {
            throw new System.NotImplementedException();
        }
    }
}
