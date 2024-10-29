using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.EventSystems;

public class GridSelector : Singleton<GridSelector>
{
    public delegate void GridClickHandler(Vector2Int selectedGridPosition);
    public delegate void PieceDragEndHandler(Vector2Int dragStartedPosition, Vector2Int dragEndedPosition);
    public event PieceDragEndHandler OnPieceDragEnded;
    public event GridClickHandler OnGridClicked;

    BaseMovement.PieceColor userColor = BaseMovement.PieceColor.WHITE; // 0 : white, 1: black
    PhotonView photonView;
    Camera mainCam;
    public bool CanTouchScreen => GameMode.Instance.Mode == GameMode.GameModeType.Local || userColor == ChessRule.Instance.Turn;

    void Start()
    {
        bool willUsePhoton = GameMode.Instance.Mode == GameMode.GameModeType.Photon;
        mainCam = Camera.main;
        if (willUsePhoton)
        {
            photonView = GetComponent<PhotonView>();
            userColor = PhotonNetwork.IsMasterClient ? BaseMovement.PieceColor.WHITE : BaseMovement.PieceColor.BLACK;
        }

        var rot = (userColor == BaseMovement.PieceColor.WHITE) ? new Vector3(90f, 0f, 0f) : new Vector3(90f, 180f, 0f);
        mainCam.transform.rotation = Quaternion.Euler(rot);
    }

    protected override void Init()
    {
        shouldDestroyOnSceneChange = true;
        base.Init();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && CanTouchScreen)
            OnClick();
    }

    public void StartNewTurn()
    {

    }

    bool IsInsideOfBoard(Vector2Int pos) => pos.x >= 0 && pos.y >= 0 && pos.x < 8 && pos.y < 8;

    void OnClick()
    {
        var ray = mainCam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, LayerMask.GetMask("GRID")))
        {
            Vector3 point = hit.point;
            var selectedGrid = new Vector2Int(Mathf.RoundToInt(point.x), Mathf.RoundToInt(point.z));
            if (IsInsideOfBoard(selectedGrid))
            {
                OnGridClicked?.Invoke(selectedGrid);
                if (GameMode.Instance.Mode == GameMode.GameModeType.Photon)
                    photonView.RPC(nameof(InvokeSelectedEventRPC), RpcTarget.Others, selectedGrid.x, selectedGrid.y);
            }
        }
    }

    public void OnEndDrag(Vector3 dragStartedPosition, Vector3 dragEndedPosition)
    {
        var startGrid = new Vector2Int(Mathf.RoundToInt(dragStartedPosition.x), Mathf.RoundToInt(dragStartedPosition.z));
        var endGrid = new Vector2Int(Mathf.RoundToInt(dragEndedPosition.x), Mathf.RoundToInt(dragEndedPosition.z));
        if (IsInsideOfBoard(startGrid) && IsInsideOfBoard(endGrid))
        {
            OnPieceDragEnded?.Invoke(startGrid, endGrid);
            if (GameMode.Instance.Mode == GameMode.GameModeType.Photon)
                photonView.RPC(nameof(InvokeEndDragEventRPC), RpcTarget.Others, startGrid.x, startGrid.y, endGrid.x, endGrid.y);
        }
    }

    [PunRPC]
    public void InvokeSelectedEventRPC(int x, int y)
    {
        OnGridClicked?.Invoke(new Vector2Int(x, y));
    }

    [PunRPC]
    public void InvokeEndDragEventRPC(int x1, int y1, int x2, int y2)
    {
        OnPieceDragEnded?.Invoke(new Vector2Int(x1, y1), new Vector2Int(x2, y2));
    }
}
