using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections;

public class PromotionTab : MonoBehaviour
{
    [SerializeField] Button selectQueen;
    [SerializeField] Button selectRook;
    [SerializeField] Button selectBishop;
    [SerializeField] Button selectKnight;

    [SerializeField] Image[] promotionPiecesImage;

    public delegate void PromotionSelectionHandler(BaseMovement createdInstance);
    public event PromotionSelectionHandler OnSelected;
    BaseMovement.PieceColor color;
    PhotonView photonView;
    bool isNetWorkGame;

    public void SetColor(BaseMovement.PieceColor color)
    {
        this.color = color;
        foreach(var piece in promotionPiecesImage)
            piece.color = color == BaseMovement.PieceColor.WHITE ? Color.white : new Color(0.1960784f, 0.1960784f, 0.1960784f);
    }

    void Awake()
    {
        isNetWorkGame = GameMode.Instance.Mode == GameMode.GameModeType.Photon;
        selectQueen.onClick.AddListener(SelectQueen_onClick);
        selectBishop.onClick.AddListener(SelectBishop_onClick);
        selectRook.onClick.AddListener(SelectRook_onClick);
        selectKnight.onClick.AddListener(SelectKnight_onClick);
        photonView = GetComponent<PhotonView>();
    }

    void SelectQueen_onClick()
    {
        var queen = new QueenMovement(color, 0, 0);
        OnSelected?.Invoke(queen);
        if (isNetWorkGame)
            photonView.RPC(nameof(SelectQueen_RPC), RpcTarget.Others, null);
    }

    void SelectRook_onClick()
    {
        var rook = new RookMovement(color, 0, 0);
        OnSelected?.Invoke(rook);
        if (isNetWorkGame)
            photonView.RPC(nameof(SelectRook_RPC), RpcTarget.Others, null);
    }

    void SelectBishop_onClick()
    {
        var bishop = new BishopMovement(color, 0, 0);
        OnSelected?.Invoke(bishop);
        if (isNetWorkGame)
            photonView.RPC(nameof(SelectBishop_RPC), RpcTarget.Others, null);
    }

    void SelectKnight_onClick()
    {
        var knight = new KnightMovement(color, 0, 0);
        OnSelected?.Invoke(knight);
        if (isNetWorkGame)
            photonView.RPC(nameof(SelectKnight_RPC), RpcTarget.Others, null);
    }

    #region Multiplayer
    [PunRPC]
    void SelectKnight_RPC()
    {
        if (OnSelected == null)  // if network is too slow
            Invoke(nameof(SelectKnight_onClick), 0.1f);
        else
            SelectKnight_onClick();
    }

    [PunRPC]
    void SelectBishop_RPC()
    {
        if (OnSelected == null)  // if network is too slow
            Invoke(nameof(SelectBishop_onClick), 0.1f);
        else
            SelectBishop_onClick();
    }

    [PunRPC]
    void SelectRook_RPC()
    {
        if (OnSelected == null)  // if network is too slow
            Invoke(nameof(SelectRook_onClick), 0.1f);
        else
            SelectRook_onClick();
    }

    [PunRPC]
    void SelectQueen_RPC()
    {
        if (OnSelected == null)  // if network is too slow
            Invoke(nameof(SelectQueen_onClick), 0.1f);
        else
            SelectQueen_onClick();
    }

    #endregion
}
