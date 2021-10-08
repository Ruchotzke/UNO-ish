using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    #region SINGLETON
    public static NetworkManager Instance
    {
        get
        {
            if (_singleton == null) _singleton = GameObject.FindObjectOfType<NetworkManager>();
            return _singleton;
        }
    }
    private static NetworkManager _singleton;
    #endregion

    [Header("DEBUG")]
    public TextMeshProUGUI roomText;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        PhotonNetwork.ConnectUsingSettings();
    }

    #region CALLBACKS
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        if (PhotonNetwork.CountOfRooms == 0)
        {
            PhotonNetwork.CreateRoom("DEBUG_ROOM");
        }
        else
        {
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
        PhotonNetwork.NickName = "Player " + PhotonNetwork.CurrentRoom.PlayerCount;
        photonView.RPC("OnRoomUpdate", RpcTarget.All);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        OnRoomUpdate();
    }

    #endregion

    #region SYNC

    [PunRPC]
    public void OnRoomUpdate()
    {
        roomText.text = "<b>" + PhotonNetwork.NickName + "</b> in room <b>" + PhotonNetwork.CurrentRoom.Name + "</b>\n";

        foreach(var player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            roomText.text += " - " + player.NickName + "\n";
        }
    }

    [PunRPC]
    public void CardPlayed(Photon.Realtime.Player player, simulation.Card playedCard)
    {
        /* First find the player being referenced */
        simulation.Player activePlayer = GameManager.Instance.Players.Find((simulation.Player p) => p.player == player);
        if (activePlayer == null) Debug.LogError("Unable to find referenced player " + player.NickName);

        /* Next remove the card from their hand */
        int cardIndex = activePlayer.hand.IndexOf(playedCard);
        if (cardIndex == -1) Debug.LogError("Unable to find card in player hand to play.");
        activePlayer.hand.RemoveAt(cardIndex);

        /* Next add the card to the played pile */
        GameManager.Instance.Discard.Add(playedCard);
    }


    #endregion
}
