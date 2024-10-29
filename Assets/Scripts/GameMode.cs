using UnityEngine;

public class GameMode : Singleton<GameMode>
{
    public enum GameModeType { Local, Photon }
    [SerializeField] GameModeType mode;
    public GameModeType Mode => mode;

    protected override void Init()
    {
        shouldDestroyOnSceneChange = true;
        base.Init();
    }
}
