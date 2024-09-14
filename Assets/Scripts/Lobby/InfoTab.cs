using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;
using System.Collections.Generic;

public class InfoTab : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField userNameInputField;
    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] RectTransform scrollviewContent;
    [SerializeField] Button pRoomIndicator;
    string roomName = string.Empty;
    Dictionary<string, GameObject> roomIndicators = new();


    void Start()
    {
        SetNickName();
        SetRoomName();
    }

    public void SetNickName()
    {
        string nickName = string.IsNullOrEmpty(userNameInputField.text) ? PlayerPrefs.GetString("USER_NAME", "USER_UNKNOWN") : userNameInputField.text;
        userNameInputField.SetTextWithoutNotify(nickName);
        PlayerPrefs.SetString("USER_NAME", nickName);
        PhotonNetwork.NickName = nickName;
        Debug.Log($"[SetNickName] {nickName}");
    }

    public void SetRoomName()
    {
        roomName = roomNameInputField.text;
    }

    public void CreateRoom()
    {
        RoomOptions roomOption = new() { MaxPlayers = 2, IsOpen = true, IsVisible = true };
        PhotonNetwork.CreateRoom(roomName, roomOption);
    }

    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.JoinLobby();
    }

    #region Photon

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log(roomList.Count);

        foreach (var roomInfo in roomList)
        {
            Debug.Log(roomInfo.Name);
            bool hasKey = roomIndicators.ContainsKey(roomInfo.Name);
            if (roomInfo.RemovedFromList && hasKey)
            {
                var target = roomIndicators[roomInfo.Name];
                roomIndicators.Remove(roomInfo.Name);
                Destroy(target);
            }
            else if(!hasKey && !roomInfo.RemovedFromList)
            {
                var target = Instantiate(pRoomIndicator, scrollviewContent);
                string thisRoom = roomInfo.Name;
                target.onClick.AddListener(()=>PhotonNetwork.JoinRoom(thisRoom));
                roomIndicators.Add(roomInfo.Name, target.gameObject);
            }
        }
    }
    #endregion
}
