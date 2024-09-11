using UnityEngine;
namespace Command
{
    public interface IGameCommand
    {
        public delegate void DoHandler();
        public event DoHandler OnDo;
        public IGameCommand Last { get; }

        void Do();
        void UnDo();
        string Info {  get; }
    }
}
