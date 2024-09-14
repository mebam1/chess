using DG.Tweening;
using UnityEngine;
using Photon.Pun;

public class VisualPiece : MonoBehaviour
{
    public delegate void DieHandler(VisualPiece diedVisualPiece);
    public event DieHandler OnDie;
    Tween tween;

    public void SetRotation()
    {
        // white: no rotation, black: 180 degree rotation
        var rot = new Vector3(transform.rotation.eulerAngles.x,
            PhotonNetwork.IsMasterClient ? 0f : 180f, 0f);
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
}
