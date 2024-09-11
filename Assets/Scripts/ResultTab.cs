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

        switch (winner)
        {
            case ChessRule.GameWinner.WHITE:
                winnerImg.color = Color.white;
                winnerTxt.SetText("White wins");
                break;
            case ChessRule.GameWinner.BLACK:
                winnerImg.color = new Color(0.1960784f, 0.1960784f, 0.1960784f); // grey
                winnerTxt.SetText("Black wins");
                break;
            case ChessRule.GameWinner.DRAW:
                winnerImg.color = Color.clear;
                winnerTxt.SetText("Draw");
                break;
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

    void Retry() => SceneManager.LoadScene(1);
}
