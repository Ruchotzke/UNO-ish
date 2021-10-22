using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;
using ExitGames.Client.Photon;
using simulation;

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

        PhotonPeer.RegisterType(typeof(simulation.Card), (byte)'Z', Utilities.SerializeCard, Utilities.DeserializeCard);
    }

    #region CALLBACKS
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    //public override void OnJoinedLobby()
    //{
    //    if (PhotonNetwork.CountOfRooms == 0)
    //    {
    //        PhotonNetwork.CreateRoom("MAINROOM");
    //    }
    //    else
    //    {
    //        PhotonNetwork.JoinRandomRoom();
    //    }
    //}

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
        PhotonNetwork.NickName = "Player " + PhotonNetwork.CurrentRoom.PlayerCount;
        photonView.RPC("OnRoomUpdate", RpcTarget.All);
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
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
    public void CardPlayed(Photon.Realtime.Player player, Card playedCard)
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

    [PunRPC]
    public void GameLoaded(Photon.Realtime.Player player)
    {
        /* Only the master client handles starting the game */
        if (!PhotonNetwork.IsMasterClient) return;

        /* Generate a new player for this new player */
        simulation.Player newPlayer = new simulation.Player();
        newPlayer.player = player;

        /* Generate a hand for this player */
        for(int i = 0; i < GameManager.Instance.StartingHandSize; i++)
        {
            if (GameManager.Instance.Deck.Count <= 0) Debug.LogError("Ran out of cards to distribute.");
            newPlayer.hand.Add(GameManager.Instance.Deck[0]);
            GameManager.Instance.Deck.RemoveAt(0);
        }

        /* Add the player to the list of players */
        GameManager.Instance.Players.Add(newPlayer);

        /* If all of the players loaded into the scene, we can start the game */
        if(GameManager.Instance.Players.Count == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            Debug.Log("All players connected. Starting the game.");
        }
    }


    #endregion
}
