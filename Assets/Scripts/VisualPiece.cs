using DG.Tweening;
using UnityEngine;
using Photon.Pun;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider))]
public class VisualPiece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public delegate void DieHandler(VisualPiece diedVisualPiece);
    public event DieHandler OnDie;
    Tween tween;
    Vector3 dragStartPosition;
    //BaseMovement.PieceColor userColor;
    int dragTargetLayer = 0;
    Camera mainCam;
    [SerializeField] BaseMovement.PieceColor color;

    void Awake()
    {
        //userColor = PhotonNetwork.IsMasterClient ? BaseMovement.PieceColor.WHITE : BaseMovement.PieceColor.BLACK;
        dragTargetLayer = LayerMask.GetMask("GRID");
        mainCam = Camera.main;
    }


    public void SetRotation()
    {
        // white: no rotation, black: 180 degree rotation
        Vector3 rot;
        if (GameMode.Instance.Mode == GameMode.GameModeType.Photon)
            rot = new Vector3(transform.rotation.eulerAngles.x, PhotonNetwork.IsMasterClient ? 0f : 180f, 0f);
        else
            rot = new Vector3(transform.rotation.eulerAngles.x, BaseMovement.PieceColor.WHITE == color ? 0f : 180f, 0f);
        transform.rotation = Quaternion.Euler(rot);
    }

    public void Move(Vector2Int cell)
    {
        tween?.Complete(true);
        Vector3 target = new Vector3(cell.x, transform.position.y, cell.y);
        tween = transform.DOMove(target, 0.1f).SetEase(Ease.OutQuad).OnComplete(() => tween = null);
    }

    public void SetPositionImmediately(int x, int y)
    {
        tween?.Complete(true);
        Vector3 target = new Vector3(x, transform.position.y, y);
        transform.position = target;
        tween = null;
    }


    public void Die()
    {
        tween?.Complete(true);
        OnDie?.Invoke(this);
        //Debug.Log($"I am dead: {gameObject.name}");
        Destroy(gameObject);
    }

    public void OnNewTurnStart()
    {

    }

    #region INPUT_DRAG
    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        bool isTweening = tween != null && tween.IsPlaying();
        if (isTweening || !GridSelector.Instance.CanTouchScreen) return;
        //Debug.Log("OnBeginDrag");
        transform.localScale *= 1.08f;
        dragStartPosition = transform.position;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        bool isTweening = tween != null && tween.IsPlaying();
        if (isTweening || !GridSelector.Instance.CanTouchScreen) return;
        //Debug.Log("OnDrag");
        FollowMousePosition();
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        bool isTweening = tween != null && tween.IsPlaying();
        if (isTweening || !GridSelector.Instance.CanTouchScreen) return;
        //Debug.Log("OnEndDrag");
        transform.localScale = Vector3.one;
        Vector3 dragEndPosition = transform.position;
        transform.position = dragStartPosition;
        GridSelector.Instance.OnEndDrag(dragStartPosition, dragEndPosition);
    }

    void FollowMousePosition()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, mainCam.farClipPlane, dragTargetLayer))
            transform.position = new Vector3(hit.point.x, transform.position.y, hit.point.z);
    }
    #endregion
}
