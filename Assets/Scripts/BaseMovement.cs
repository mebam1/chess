using Command;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public abstract class BaseMovement
{
    #region variables
    public enum PieceColor { WHITE, BLACK, COUNT };
    PieceColor color;
    public PieceColor Color => color;
    public int x;
    public int y;

    protected bool[,] attackMap = new bool[8, 8]; // 가려지지 않아 공격할 수 있는 칸
    protected List<Vector2Int> availables = new(); // 이 기물이 이동 가능한 칸. 캐슬링, 폰 두칸전진등을 반영함.
    protected Dictionary<Vector2Int, IGameCommand> availableCommands = new(); // availables의 좌표를 통해 실행할 작업을 제공한다.
    protected BaseMovement kingPinner;
    public bool[, ] AttackMap => attackMap;
    public bool IsFirstMove = true;
    public BaseMovement KingPinner => kingPinner;
    public IReadOnlyDictionary<Vector2Int, IGameCommand> AvailableCommands => availableCommands;
    public IReadOnlyList<Vector2Int> Availables => availables;
    public int AvailableCount => availables.Count;
    #endregion

    #region Methods
    public BaseMovement(PieceColor color, int x, int y)
    {
        this.color = color;
        this.x = x;
        this.y = y;
    }

    public abstract void UpdateMap();

    public virtual void UpdateAvailables()
    {
        availables.Clear();
        availableCommands.Clear();

        var friendly = ChessRule.Instance.GetPieceSet(Color);
        var opposite = ChessRule.Instance.GetOppositePieceSet(Color);

        int kx = friendly.King.x, ky = friendly.King.y;
        var kingAttackers = opposite.GetAttackers(kx, ky);
        if (kingAttackers.Count == 0)
        {
            // 체크가 아닐경우 핀 검사만 해주면 된다.
            var (_, thisAttacker) = opposite.KingPinners.FirstOrDefault(x => x.Item1 == this);

            if (thisAttacker == null) // 내가 pinner가 아닌 경우.
            {
                for (int i = 0; i < 8; ++i)
                    for (int j = 0; j < 8; ++j)
                        if (CanMoveOrAttack(i, j))
                            AddMoveOrAttackCommand(i, j, opposite, friendly);
            }
            else // 내가 pinner인 경우.
            {
                var pinLine = GetLine(kx, ky, thisAttacker.x, thisAttacker.y);
                pinLine.Remove(new Vector2Int(x, y));

                foreach (var cell in pinLine)
                {
                    if (CanMoveOrAttack(cell.x, cell.y))
                    {
                        // Debug.Log($"[{cell.x}, {cell.y}]");
                        AddMoveOrAttackCommand(cell.x, cell.y, opposite, friendly);
                    }   
                }

            }
        }
        else if (kingAttackers.Count == 1)
        {
            BaseMovement kingAttacker = kingAttackers[0];
            var (_, thisAttacker) = opposite.KingPinners.FirstOrDefault(x => x.Item1 == this);
            var checkLine = GetLine(kx, ky, kingAttacker.x, kingAttacker.y);
            if (thisAttacker == null) // 내가 pinner가 아닌 경우.
            {
                foreach (var cell in checkLine)
                {
                    if (CanMoveOrAttack(cell.x, cell.y))
                        AddMoveOrAttackCommand(cell.x, cell.y, opposite, friendly);
                }
            }
            else // 내가 pinner인 경우.
            {
                var pinLine = GetLine(kx, ky, thisAttacker.x, thisAttacker.y);
                pinLine.Remove(new Vector2Int(x, y));
                // 내가 pinner이면 checkLine - pinLine의 교집합 내에서만 움직일 수 있다.
                var intersect = pinLine.Intersect(checkLine);
                foreach (var cell in intersect)
                    if (CanMoveOrAttack(cell.x, cell.y))
                        AddMoveOrAttackCommand(cell.x, cell.y, opposite, friendly);
            }
        }

        // 체크가 2개 이상이면 왕 밖에 못움직임. KingMovement에서 override할 것.
    }

    protected bool CanMoveOrAttack(int x, int y) => attackMap[x, y] && 
        (ChessRule.Instance.LocationMap[x, y] == null || Color != ChessRule.Instance.LocationMap[x, y].Color);
    protected void AddMoveOrAttackCommand(int x, int y, PieceSet opposite, PieceSet friendly)
    {
        var cell = new Vector2Int(x, y);
        availables.Add(cell);
        IGameCommand command;
        if (opposite.LocationMap[x, y] == null)
            command = new Move(this, x, y);
        else
            command = new Attack(this, opposite.LocationMap[x, y]);
        availableCommands.Add(cell, command);
    }


    /// <returns>P1은 열고 P2는 닫은 구간의 직선을 반환. 즉 (p1, p2]를 반환함. </returns>
    protected List<Vector2Int> GetLine(int x1, int y1, int x2, int y2)
    {
        // 한 직선상에 있음을 보장
        //Debug.Assert(x1 == x2 || y1 == y2 || Mathf.Abs(x1 - x2) == Mathf.Abs(y1 - y2));
        
        List<Vector2Int> ret = new();

        bool isStraightLine = x1 == x2 || y1 == y2 || Mathf.Abs(x1 - x2) == Mathf.Abs(y1 - y2);
        if (!isStraightLine)
        {
            ret.Add(new Vector2Int(x2, y2));
            return ret;
        }

        int xOffset = x1 < x2 ? 1 : (x1 == x2 ? 0 : -1);
        int yOffset = y1 < y2 ? 1 : (y1 == y2 ? 0 : -1);

        int xPos = x1, yPos = y1;
        while (xPos != x2 || yPos != y2)
        {
            xPos += xOffset;
            yPos += yOffset;
            ret.Add(new Vector2Int(xPos, yPos));
        }

        return ret;
    }

    protected bool IsInsideOfBoard(int xPos, int yPos) => xPos >= 0 && yPos >= 0 && xPos < 8 && yPos < 8;
    #endregion

    #region Visualize
    VisualPiece visual;
    public void BindVisualizer(VisualPiece visual)
    {
        this.visual = visual;
        visual.SetPositionImmediately(x, y);
    }

    public void SetVisualPositionImmediately(int x, int y) => visual.SetPositionImmediately(x, y);
    public void MoveVisual(Vector2Int cell) => visual.Move(cell);
    public void RemoveVisual()
    {
        visual.Die();
        visual = null;
    }

    
    #endregion
}
