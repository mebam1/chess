using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RoomIndicator : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI roomNameTMP;
    [SerializeField] Button button;

    public void SetName(string name) => roomNameTMP.SetText(name);
    public void AddListener(UnityAction call) => button.onClick.AddListener(call);
}
