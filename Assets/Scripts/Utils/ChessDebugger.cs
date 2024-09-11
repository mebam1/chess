using UnityEngine;

public class ChessDebugger : MonoBehaviour
{
    public int x;
    public int y;

    public void PrintCell()
    {
        var t = ChessRule.Instance.LocationMap[x, y];
        foreach (var c in t.Availables)
            Debug.Log(c);
    }

    public void Print()
    {
        Debug.Log("UI DEBUG");
    }
}
