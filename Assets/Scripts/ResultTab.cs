using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultTab : MonoBehaviour
{
    [SerializeField] Button retry;
    [SerializeField] Button exitGame;
    [SerializeField] Image winnerImg;
    [SerializeField] TextMeshProUGUI winnerTxt;
    

    void Start()
    {
        ChessRule.Instance.OnGameEnded -= OnGameEnded;
        ChessRule.Instance.OnGameEnded += OnGameEnded;
        gameObject.SetActive(false);
        retry.onClick.AddListener(Retry);
        exitGame.onClick.AddListener(Quit);
    }

    void OnGameEnded(ChessRule.GameWinner winner)
    {
        gameObject.SetActive(true);
        ChessRule.Instance.OnGameEnded -= OnGameEnded;
        var userColor = PhotonNetwork.IsMasterClient ? BaseMovement.PieceColor.WHITE : BaseMovement.PieceColor.BLACK;

        switch (winner)
        {
            case ChessRule.GameWinner.WHITE:
                winnerImg.color = Color.white;
                break;
            case ChessRule.GameWinner.BLACK:
                winnerImg.color = new Color(0.1960784f, 0.1960784f, 0.1960784f); // grey
                break;
            case ChessRule.GameWinner.DRAW:
                winnerImg.color = Color.clear;
                winnerTxt.SetText("Draw");
                return;
        }

        try
        {
            if ((int)userColor == (int)winner)
                winnerTxt.SetText($"Winner: {PhotonNetwork.NickName}");
            else
                winnerTxt.SetText($"Winner: {PhotonNetwork.PlayerListOthers[0].NickName}");
        }
        catch(System.IndexOutOfRangeException indexException)
        {
            Debug.LogWarning(indexException);
        }
    }

    void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void Retry()
    {
        StartCoroutine(ReStartScene());
    }

    IEnumerator ReStartScene()
    {
        PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
        while (!PhotonNetwork.IsMasterClient)
            yield return null;

        Debug.Log($"[ReStartScene] PlayerCount: {PhotonNetwork.CurrentRoom.PlayerCount}");
        PhotonNetwork.LoadLevel("Loading");
    }
}
