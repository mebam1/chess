using Photon.Realtime;
using Photon.Pun;
using UnityEngine;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    readonly string version = "0.1";


    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = version;
        Debug.Log($"PhotonNetwork.SendRate: {PhotonNetwork.SendRate}");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log($"[OnJoinedLobby] PhotonNetwork.InLobby: {PhotonNetwork.InLobby}");
        PhotonNetwork.JoinRandomRoom();
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log($"[OnConnectedToMaster] PhotonNetwork.InLobby: {PhotonNetwork.InLobby}");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"[OnJoinRandomFailed] code({returnCode}): {message}");
        RoomOptions roomOption = new() { MaxPlayers = 2, IsOpen = true, IsVisible = true };
        PhotonNetwork.CreateRoom("My Room", roomOption);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log($"[OnCreateRoomFailed] code({returnCode}): {message}");
    }

    public override void OnCreatedRoom()
    {
        Debug.Log($"[OnCreatedRoom] New Room Name: {PhotonNetwork.CurrentRoom.Name}");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"[OnJoinedRoom] Joined Room Name: {PhotonNetwork.CurrentRoom.Name}\n" +
            $"Player Count: {PhotonNetwork.CurrentRoom.PlayerCount}\n" +
            $"In Room: {PhotonNetwork.InRoom}");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"[OnPlayerEnteredRoom] Entered Player Name: {newPlayer.NickName}\n" +
            $"Player Actor Number: {newPlayer.ActorNumber}");

        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel("Main");
    }
}
