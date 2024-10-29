using Photon.Pun;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ChessDebugger : MonoBehaviour
{ 
    /// <summary>Reload this scene.</summary>
    public void StartNewGame()
    {
        bool isNetWorkGame = GameMode.Instance.Mode == GameMode.GameModeType.Photon;
        if (isNetWorkGame)
        {
            StartCoroutine(ReStartScene());
        }
        else
        {
            int current = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(current); 
        }
    }

    IEnumerator ReStartScene()
    {
        PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
        while (!PhotonNetwork.IsMasterClient)
            yield return null;

        //Debug.Log($"[ReStartScene] PlayerCount: {PhotonNetwork.CurrentRoom.PlayerCount}");
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.LoadLevel("Loading");
    }
}
