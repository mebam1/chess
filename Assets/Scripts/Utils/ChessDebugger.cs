using Photon.Pun;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ChessDebugger : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("Scene Start!");
    }

    public void StartNewGame()
    {
        StartCoroutine(ReStartScene());
    }

    IEnumerator ReStartScene()
    {
        PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
        while (!PhotonNetwork.IsMasterClient)
            yield return null;

        Debug.Log($"[ReStartScene] PlayerCount: {PhotonNetwork.CurrentRoom.PlayerCount}");
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.LoadLevel("Loading");
    }
}
