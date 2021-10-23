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

    #endregion

    #region SYNC

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

            Card[] decks = new Card[GameManager.Instance.Players.Count * GameManager.Instance.StartingHandSize];
            List<Photon.Realtime.Player> players = new List<Photon.Realtime.Player>();
            int index = 0;
            foreach(var playerinstance in GameManager.Instance.Players)
            {
                players.Add(playerinstance.player);
                foreach(Card hand in playerinstance.hand)
                {
                    decks[index++] = hand;
                }
            }

            photonView.RPC("PlayersConnectedStartGame", RpcTarget.All, GameManager.Instance.Deck.ToArray(), players.ToArray(), decks);
        }
    }

    [PunRPC]
    public void PlayersConnectedStartGame(Card[] Deck, Photon.Realtime.Player[] players, Card[] cards)
    {
        /* If we aren't the host, initialize our decks, players, etc */
        GameManager.Instance.Deck = new List<Card>();
        GameManager.Instance.Deck.AddRange(Deck);

        for(int i = 0; i < players.Length; i++)
        {
            List<Card> handCopy = new List<Card>();
            for (int j = GameManager.Instance.StartingHandSize * i; j < GameManager.Instance.StartingHandSize * i + GameManager.Instance.StartingHandSize; j++)
            {
                handCopy.Add(cards[j]);
            }
            
            GameManager.Instance.Players.Add(new simulation.Player() { hand = handCopy, player = players[i] });
            if (players[i] == PhotonNetwork.LocalPlayer) GameManager.Instance.me = GameManager.Instance.Players[GameManager.Instance.Players.Count - 1];
        }


        GameManager.Instance.StartGame();
    }


    #endregion

    #region UTILITIES

    /// <summary>
    /// Generate a photon room for a new game. Will trigger a network
    /// callback when generated.
    /// </summary>
    /// <param name="roomName">The name of the room to create.</param>
    public void CreateRoom(string roomName)
    {
        RoomOptions options = new RoomOptions();

        PhotonNetwork.CreateRoom(roomName, options);
    }

    /// <summary>
    /// Join a room by name. Will trigger a network
    /// callback when joined.
    /// </summary>
    /// <param name="roomName">The roomname to join.</param>
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    /// <summary>
    /// Change the scene - RPC to change the scene on all people in the room.
    /// </summary>
    /// <param name="sceneName">The new scene to open.</param>
    [PunRPC]
    public void ChangeScene(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }
    #endregion
}
