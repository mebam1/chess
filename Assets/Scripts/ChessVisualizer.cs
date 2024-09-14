using Command;
using Photon.Realtime;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ChessVisualizer : Singleton<ChessVisualizer>
{
    [SerializeField] GameObject pIndicator;
    [SerializeField] VisualPieceSet[] visualPieceSets;
    [SerializeField] PromotionTab promotionTab;
    IObjectPool<GameObject> indicatorPool;

    BaseMovement focusedPiece;
    List<GameObject> showingIndicators = new();
    public PromotionTab PromotionTab => promotionTab;

    void Start()
    {
        visualPieceSets[0].Init(BaseMovement.PieceColor.WHITE);
        visualPieceSets[1].Init(BaseMovement.PieceColor.BLACK); 
        GridSelector.Instance.OnSelected -= OnGridSelected;
        GridSelector.Instance.OnSelected += OnGridSelected;
        ChessRule.Instance.OnGameEnded -= OnGameEnded;
        ChessRule.Instance.OnGameEnded += OnGameEnded;
        StartNewTurn();
    }

    void OnGameEnded(ChessRule.GameWinner winner)
    {
        ChessRule.Instance.OnGameEnded -= OnGameEnded;
        GridSelector.Instance.OnSelected -= OnGridSelected;
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
            {
                HideAvailables();
                focusedPiece = null;
                GridSelector.Instance.OnSelected -= OnGridSelected;
                command.Last.OnDo += OnCommandEnd;
                command.Do();
            }
        }
        else
        {
            HideAvailables();
            focusedPiece = selected;
            ShowAvaliables();
        }
    }

    void OnCommandEnd()
    {
        StartNewTurn();
        GridSelector.Instance.OnSelected -= OnGridSelected;
        GridSelector.Instance.OnSelected += OnGridSelected;
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
