using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ChessVisualizer : Singleton<ChessVisualizer>
{
    [SerializeField] GameObject pIndicator;
    [SerializeField] VisualPieceSet[] visualPieceSets;
    IObjectPool<GameObject> indicatorPool;

    //BaseMovement.PieceColor turn = BaseMovement.PieceColor.BLACK;
    BaseMovement focusedPiece;
    List<GameObject> showingIndicators = new();

    void Start()
    {
        visualPieceSets[0].Init(BaseMovement.PieceColor.WHITE);
        visualPieceSets[1].Init(BaseMovement.PieceColor.BLACK); 
        GridSelector.Instance.OnSelected -= OnGridSelected;
        GridSelector.Instance.OnSelected += OnGridSelected;
        StartNewTurn();
    }

    protected override void Init()
    {
        base.Init();
        shouldDestroyOnSceneChange = true;
        indicatorPool = new ObjectPool<GameObject>
        (
            () => Instantiate(pIndicator), // Create
            x => x.SetActive(true), // On Get
            x => x.SetActive(false), // On Release
            x => Destroy(x), // On Destroy
            maxSize: 32
        );
    }

    void StartNewTurn()
    {
        focusedPiece = null;
        ChessRule.Instance.StartNewTurn();
        GridSelector.Instance.StartNewTurn();
        visualPieceSets[0].StartNewTurn();
        visualPieceSets[1].StartNewTurn();
    }

    void OnGridSelected(Vector2Int selectedGridPosition)
    {
        var currentTurnOwnerPieceSet = ChessRule.Instance.GetPieceSet(ChessRule.Instance.Turn);
        var selected = currentTurnOwnerPieceSet.LocationMap[selectedGridPosition.x, selectedGridPosition.y];

        if(selected == null) // 아군 기물을 선택하지 않은 경우.
        {
            if (focusedPiece != null && focusedPiece.AvailableCommands.TryGetValue(selectedGridPosition, out var command))
            {
                HideAvailables();
                focusedPiece = null;
                command.Do();
                Debug.Log("command do : " + command.Info);
                StartNewTurn();
            }
        }
        else
        {
            HideAvailables();
            focusedPiece = selected;
            Debug.Log($"Selected {focusedPiece}");
            ShowAvaliables();
        }
    }

    void ShowAvaliables()
    {
        Debug.Assert(focusedPiece != null);
        foreach(var cell in focusedPiece.Availables)
        {
            var indicator = indicatorPool.Get();
            indicator.transform.position = new Vector3(cell.x, 3f, cell.y);
            showingIndicators.Add(indicator);
        }
    }

    void HideAvailables()
    {
        foreach(var indicator in showingIndicators)
            indicatorPool.Release(indicator);
        showingIndicators.Clear();
    }
}
