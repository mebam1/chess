using UnityEngine;
namespace Command
{
    public interface IGameCommand
    {
        void Do();
        void UnDo();
        string Info {  get; }
    }
}
