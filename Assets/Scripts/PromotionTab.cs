using UnityEngine;
using UnityEngine.UI;

public class PromotionTab : MonoBehaviour
{
    [SerializeField] Button selectQueen;
    [SerializeField] Button selectRook;
    [SerializeField] Button selectBishop;
    [SerializeField] Button selectKnight;

    public delegate void PromotionSelectionHandler(BaseMovement createdInstance);
    public event PromotionSelectionHandler OnSelected;
    BaseMovement.PieceColor color;

    public void SetColor(BaseMovement.PieceColor color) => this.color = color;

    void Awake()
    {
        selectQueen.onClick.AddListener(SelectQueen_onClick);
        selectBishop.onClick.AddListener(SelectBishop_onClick);
        selectRook.onClick.AddListener(SelectRook_onClick);
        selectKnight.onClick.AddListener(SelectKnight_onClick);
    }

    void SelectQueen_onClick()
    {
        var queen = new QueenMovement(color, 0, 0);
        OnSelected?.Invoke(queen);
    }

    void SelectRook_onClick()
    {
        var rook = new RookMovement(color, 0, 0);
        OnSelected?.Invoke(rook);
    }

    void SelectBishop_onClick()
    {
        var bishop = new BishopMovement(color, 0, 0);
        OnSelected?.Invoke(bishop);
    }

    void SelectKnight_onClick()
    {
        var knight = new KnightMovement(color, 0, 0);
        OnSelected?.Invoke(knight);
    }
}
