using UnityEngine;

public class VisualPiece : MonoBehaviour
{
    //public delegate void DieHandler(VisualPiece diedVisualPiece);
    //public event DieHandler OnDie;

    public void Move(Vector2Int cell)
    {
        Vector3 target = new Vector3(cell.x, transform.position.y, cell.y);

        // TODO: 나중에 애니메이션 넣기.
        transform.position = target;
    }

    public void SetPositionImmediately(int x, int y)
    {
        Vector3 target = new Vector3(x, transform.position.y, y);
        transform.position = target;
    }


    public void Die()
    {
        //OnDie?.Invoke(this);
        Destroy(gameObject);
    }

    public void OnNewTurnStart()
    {
        var rot = new Vector3(transform.rotation.eulerAngles.x, ChessRule.Instance.Turn == BaseMovement.PieceColor.WHITE ? 0f : 180f, 0f);
        transform.rotation = Quaternion.Euler(rot);
    }
}
