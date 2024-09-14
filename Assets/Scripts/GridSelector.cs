using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GridSelector : Singleton<GridSelector>
{
    public delegate void GridSelectionHandler(Vector2Int selectedGridPosition);
    public event GridSelectionHandler OnSelected;
    BaseMovement.PieceColor userColor; // 0 : white, 1: black
    PhotonView photonView;
    Camera mainCam;
    

    void Start()
    {
        mainCam = Camera.main;
        userColor = PhotonNetwork.IsMasterClient ? BaseMovement.PieceColor.WHITE : BaseMovement.PieceColor.BLACK;
        photonView = GetComponent<PhotonView>();

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
        if(Input.GetMouseButtonDown(0) && userColor == ChessRule.Instance.Turn) // userColor == ChessRule.Instance.Turn --> for multiplayer.
        {
            var ray = mainCam.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, LayerMask.GetMask("GRID")))
            {
                Vector3 point = hit.point;
                var selectedGrid = new Vector2Int(Mathf.RoundToInt(point.x), Mathf.RoundToInt(point.z));
                if (IsInsideOfBoard(selectedGrid))
                {
                    OnSelected?.Invoke(selectedGrid);
                    photonView.RPC(nameof(InvokeSelectedEventRPC), RpcTarget.Others, selectedGrid.x, selectedGrid.y);
                }
            }
        }
    }

    public void StartNewTurn()
    {

    }

    bool IsInsideOfBoard(Vector2Int pos) => pos.x >= 0 && pos.y >= 0 && pos.x < 8 && pos.y < 8;

    
    [PunRPC]
    public void InvokeSelectedEventRPC(int x, int y)
    {
        OnSelected?.Invoke(new Vector2Int(x, y));
    }
    
}
