using Photon.Pun;
using simulation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region SINGLETON
    public static GameManager Instance
    {
        get
        {
            if(_singleton == null) _singleton = GameObject.FindObjectOfType<GameManager>();
            return _singleton;
        }
    }
    private static GameManager _singleton;
    #endregion

    #region PROPERTIES
    public List<Card> Deck = new List<Card>();          /* The deck of unknown cards */
    public List<Card> Discard = new List<Card>();       /* The face up deck of used cards */

    public List<Player> Players = new List<Player>();   /* The players in this game */
    int currPlayer = 0;                                 /* The ID of the player whose turn it is currently */
    int playerIncrement = 1;                            /* The change in index when the turn is over (+1 or -1)*/
    #endregion

    #region SETTINGS
    public int StartingHandSize = 6;
    #endregion

    [SerializeField] MeshRenderer CardGraphic;
    [SerializeField] Grid CardGrid;

    private void Start()
    {
        /* Our scene is loaded. */
        /* If we are the master, set up the game. Otherwise alert the master we are loaded in */
        if (PhotonNetwork.IsMasterClient)
        {
            /* Set up the game */
            InitializeGame();

            /* Generate a player for the host */
            NetworkManager.Instance.photonView.RPC("GameLoaded", RpcTarget.All, PhotonNetwork.LocalPlayer);
        }
        else
        {
            NetworkManager.Instance.photonView.RPC("GameLoaded", RpcTarget.OthersBuffered, PhotonNetwork.LocalPlayer);
        }
    }

    public void InitializeGame()
    {
        /* Generate the deck */
        GenerateDefaultDeck();
        Utilities.Shuffle(Deck);

        /* Select one card to add to the discard */
        Discard.Add(Deck[0]);
        Deck.RemoveAt(0);
    }

    private void DEBUG_DrawCards()
    {
        for (int y = 0, i = 0; y < 8; y++)
        {
            for (int x = 0; x < 14; x++, i++)
            {
                if (i >= Deck.Count) break;
                var instance = Instantiate(CardGraphic, CardGrid.transform);
                instance.transform.localPosition = CardGrid.CellToLocal(new Vector3Int(x, y, 0));
                instance.material.SetTexture("_BaseMap", CardGraphics.Instance.Graphics[Deck[i]]);
            }
        }
    }

    public void TakeTurn()
    {
        currPlayer += playerIncrement;
    }

    public void GenerateDefaultDeck()
    {
        Deck.Clear();
        
        /* First generate color specific */
        for(int colorindex = 0; colorindex < 4; colorindex++)
        {
            CardColor color = (CardColor)colorindex;

            /* Generate all numeric cards */
            for(int i = 0; i < 10; i++)
            {
                Deck.Add(new Card(color, CardType.NUMBER, i));
                if(i != 0) Deck.Add(new Card(color, CardType.NUMBER, i));
            }

            /* Generate specialty cards */
            Deck.Add(new Card(color, CardType.SKIP));
            Deck.Add(new Card(color, CardType.REVERSE));
            Deck.Add(new Card(color, CardType.DRAW_2));
            Deck.Add(new Card(color, CardType.SKIP));
            Deck.Add(new Card(color, CardType.REVERSE));
            Deck.Add(new Card(color, CardType.DRAW_2));
        }

        /* Generate wild cards */
        Deck.Add(new Card(CardColor.WILD, CardType.DRAW_4));
        Deck.Add(new Card(CardColor.WILD, CardType.WILD));
        Deck.Add(new Card(CardColor.WILD, CardType.DRAW_4));
        Deck.Add(new Card(CardColor.WILD, CardType.WILD));
        Deck.Add(new Card(CardColor.WILD, CardType.DRAW_4));
        Deck.Add(new Card(CardColor.WILD, CardType.WILD));
        Deck.Add(new Card(CardColor.WILD, CardType.DRAW_4));
        Deck.Add(new Card(CardColor.WILD, CardType.WILD));
    }
}