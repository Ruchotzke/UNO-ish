using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class MenuController : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    [Header("Menus")]
    public GameObject MainMenu;
    public GameObject CreateRoom;
    public GameObject LobbyBrowser;
    public GameObject Lobby;

    [Header("MainMenu")]
    public Button JoinButton;
    public Button CreateButton;

    [Header("CreateRoom")]
    public TMP_InputField HostName;
    public TMP_InputField RoomName;

    [Header("LobbyBrowser")]
    public Transform RoomContainer;
    public Button pf_RoomButton;
    public TextMeshProUGUI RoomDescription;
    public TMP_InputField PlayerName;
    public Button JoinRoom;

    [Header("Lobby")]
    public TextMeshProUGUI LobbyName;
    public TextMeshProUGUI NumPlayers;
    public TextMeshProUGUI BlueList;
    public TextMeshProUGUI RedList;
    public TextMeshProUGUI ObserverList;
    public Button ReadyButton;
    public Button StartGame;

    private List<RoomInfo> currRoomList = new List<RoomInfo>();
    List<GameObject> currRoomObjects = new List<GameObject>();
    private RoomInfo selectedRoom = null;


    #region UTILITIES
    public void OpenMenu(GameObject toOpen)
    {
        /* First close all menus */
        MainMenu.SetActive(false);
        CreateRoom.SetActive(false);
        LobbyBrowser.SetActive(false);
        Lobby.SetActive(false);

        /* Open the correct menu */
        toOpen.SetActive(true);
    }
    #endregion

    #region NETWORK CALLBACKS
    public override void OnConnectedToMaster()
    {
        /* Once we connect, we can start using the application */
        CreateButton.interactable = true;
        JoinButton.interactable = true;
    }

    public override void OnJoinedRoom()
    {
        /* We joined a new room - display it */
        OpenMenu(Lobby);
        photonView.RPC("UpdateLobby", RpcTarget.All);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateLobby();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        this.currRoomList = roomList;
    }
    #endregion

    #region INPUT CALLBACKS
    public void OnMainMenuJoin()
    {
        OpenMenu(LobbyBrowser);
    }

    public void OnMainMenuCreate()
    {
        OpenMenu(CreateRoom);
    }

    public void OnCreateRoomCreate()
    {
        string hostName = HostName.text;
        string roomName = RoomName.text;
        PhotonNetwork.NickName = hostName;
        NetworkManager.Instance.CreateRoom(roomName);
    }

    public void OnRefreshBrowser()
    {
        UpdateLobbyBrowser();
    }

    public void OnJoinRoomButton()
    {
        PhotonNetwork.NickName = PlayerName.text;
        NetworkManager.Instance.JoinRoom(selectedRoom.Name);
    }

    public void OnReadyButton()
    {

    }

    public void OnStartGameButton()
    {
        //hide the room
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        //load the game scene
        NetworkManager.Instance.photonView.RPC("ChangeScene", RpcTarget.All, "Main Game");
    }
    #endregion

    #region UI

    [PunRPC]
    public void UpdateLobby()
    {
        //if we are the host we can start the game
        StartGame.interactable = PhotonNetwork.IsMasterClient;

        //display all players in the lobby
        LobbyName.text = PhotonNetwork.CurrentRoom.Name;
        NumPlayers.text = PhotonNetwork.CurrentRoom.PlayerCount + " Players";

        BlueList.text = "";
        RedList.text = "";
        ObserverList.text = "";
        foreach(var player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            if (player.IsMasterClient)
            {
                BlueList.text += "- " + player.NickName + "\n";
            }
            else
            {
                RedList.text += "- " + player.NickName + "\n";
            }
        }

    }

    public void UpdateLobbyBrowser()
    {
        /* Get rid of the current rooms */
        foreach(GameObject lobbyObject in currRoomObjects)
        {
            Destroy(lobbyObject);
        }
        currRoomObjects.Clear();

        /* Generate a new button for each lobby */
        foreach (var room in currRoomList)
        {
            var button = Instantiate(pf_RoomButton, RoomContainer);
            currRoomObjects.Add(button.gameObject);
            button.GetComponentInChildren<TextMeshProUGUI>().text = room.Name + "   (" + room.PlayerCount + "/" + room.MaxPlayers + ")";
            button.onClick.AddListener(delegate
            {
                selectedRoom = room;
                RoomDescription.text = "<b><u>" + selectedRoom.Name + "</b></u>\n";
                RoomDescription.text += "   (" + room.PlayerCount + "/" + room.MaxPlayers + ") Players";
            });
        }
    }

    #endregion
}
