using Command;
using Photon.Realtime;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using System.Drawing;
using Unity.VisualScripting;

public class ChessVisualizer : Singleton<ChessVisualizer>
{
    [SerializeField] GameObject pIndicator;
    [SerializeField] VisualPieceSet[] visualPieceSets;
    [SerializeField] PromotionTab promotionTab;
    IObjectPool<GameObject> indicatorPool;

    BaseMovement focusedPiece;
    List<GameObject> showingIndicators = new();
    public PromotionTab PromotionTab => promotionTab;

    bool CanRecieveInput
    {
        set
        {
            GridSelector.Instance.OnGridClicked -= OnGridSelected;
            GridSelector.Instance.OnPieceDragEnded -= OnPieceEndDrag;

            if (value)
            {
                GridSelector.Instance.OnGridClicked += OnGridSelected;
                GridSelector.Instance.OnPieceDragEnded += OnPieceEndDrag;
            }
        }
    }

    void Start()
    {
        visualPieceSets[0].Init(BaseMovement.PieceColor.WHITE);
        visualPieceSets[1].Init(BaseMovement.PieceColor.BLACK); 
        ChessRule.Instance.OnGameEnded -= OnGameEnded;
        ChessRule.Instance.OnGameEnded += OnGameEnded;
        CanRecieveInput = true;
        StartNewTurn();
    }

    void OnGameEnded(ChessRule.GameWinner winner)
    {
        CanRecieveInput = false;
        ChessRule.Instance.OnGameEnded -= OnGameEnded;
    }

    protected override void Init()
    {
        shouldDestroyOnSceneChange = true;
        base.Init();
        
        indicatorPool = new ObjectPool<GameObject>
        (
            () => Instantiate(pIndicator), // Create
            x => x.SetActive(true), // On Get
            x => x.SetActive(false), // On Release
            x => Destroy(x), // On Destroy
            maxSize: 32
        );

        promotionTab.gameObject.SetActive(false);
    }

    void StartNewTurn()
    {
        focusedPiece = null;
        ChessRule.Instance.StartNewTurn();
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
                InvokeCommand(command);
        }
        else
        {
            // Refreshing Indicators
            HideAvailables();
            focusedPiece = selected;
            ShowAvaliables(); 
        }
    }

    void OnPieceEndDrag(Vector2Int dragStartedPosition, Vector2Int dragEndedPosition)
    {
        var totalPieceSet = ChessRule.Instance.LocationMap;
        //var currentTurnOwnerPieceSet = ChessRule.Instance.GetPieceSet(ChessRule.Instance.Turn);
        var selected = totalPieceSet[dragStartedPosition.x, dragStartedPosition.y];
        if (selected.Color != ChessRule.Instance.Turn) return;
        focusedPiece = selected;
        if (focusedPiece.AvailableCommands.TryGetValue(dragEndedPosition, out var command))
            InvokeCommand(command);
    }

    void InvokeCommand(IGameCommand cmd)
    {
        HideAvailables();
        focusedPiece = null;
        CanRecieveInput = false;
        cmd.Last.OnDo += OnCommandEnd;
        cmd.Do();
    }

    void OnCommandEnd()
    {
        StartNewTurn();
        CanRecieveInput = true;
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

    public VisualPieceSet GetPieceSet(BaseMovement.PieceColor color) => visualPieceSets[(int)color];
}
