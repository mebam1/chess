using UnityEngine;

public class GridSelector : Singleton<GridSelector>
{
    public delegate void GridSelectionHandler(Vector2Int selectedGridPosition);
    public event GridSelectionHandler OnSelected;
    Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            var ray = mainCam.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, LayerMask.GetMask("GRID")))
            {
                Vector3 point = hit.point;
                var selectedGrid = new Vector2Int(Mathf.RoundToInt(point.x), Mathf.RoundToInt(point.z));
                OnSelected?.Invoke(selectedGrid);
            }
        }
    }

    public void StartNewTurn()
    {
        var turn = ChessRule.Instance.Turn;
        var rot = (turn == BaseMovement.PieceColor.WHITE) ? new Vector3(90f, 0f, 0f) : new Vector3(90f, 180f, 0f);
        mainCam.transform.rotation = Quaternion.Euler(rot);
    }
}
