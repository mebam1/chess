using Photon.Realtime;
using Photon.Pun;
using UnityEngine;
using System.Collections;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    readonly string version = "0.1";

    public void Connect()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = version;
        //Debug.Log($"Photon SendRate: {PhotonNetwork.SendRate}");
        PhotonNetwork.ConnectUsingSettings();
    }

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnJoinedLobby()
    {
        //Debug.Log($"[OnJoinedLobby] PhotonNetwork.InLobby: {PhotonNetwork.InLobby}");
    }
    public override void OnConnectedToMaster()
    {
        //Debug.Log($"[OnConnectedToMaster] PhotonNetwork.InLobby: {PhotonNetwork.InLobby}");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //Debug.Log($"[OnJoinRandomFailed] code({returnCode}): {message}");

    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        //Debug.Log($"[OnCreateRoomFailed] code({returnCode}): {message}");
    }

    public override void OnCreatedRoom()
    {
        //Debug.Log($"[OnCreatedRoom] New Room Name: {PhotonNetwork.CurrentRoom.Name}");
    }

    public override void OnJoinedRoom()
    {
        /*
        Debug.Log($"[OnJoinedRoom] Joined Room Name: {PhotonNetwork.CurrentRoom.Name}\n" +
            $"Player Count: {PhotonNetwork.CurrentRoom.PlayerCount}\n" +
            $"In Room: {PhotonNetwork.InRoom}");
        */
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        /*
        Debug.Log($"[OnPlayerEnteredRoom] Entered Player Name: {newPlayer.NickName}\n" +
            $"Player Actor Number: {newPlayer.ActorNumber}");
        */
        if(PhotonNetwork.CurrentRoom.PlayerCount == 2 && PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("Main_Multi");
        }
    }
}
