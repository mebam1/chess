using UnityEngine;
using System.Collections;
using Photon.Pun;

public class GotoMain : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.2f);
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.LoadLevel("Main");
    }
}
