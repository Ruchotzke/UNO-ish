using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    #region SINGLETON
    public static CanvasController Instance
    {
        get
        {
            if (_singleton == null) _singleton = GameObject.FindObjectOfType<CanvasController>();
            return _singleton;
        }
    }
    private static CanvasController _singleton;
    #endregion

    [Header("Prefabs")]
    public PlayerPanel pf_PlayerPanel;

    [Header("Serialized Fields")]
    public Transform PlayerContainer;

    private Dictionary<simulation.Player, PlayerPanel> playerPanels = new Dictionary<simulation.Player, PlayerPanel>();

    public void InitializePlayers(List<simulation.Player> players)
    {
        /* Destroy any panels which exist */
        foreach(var element in playerPanels)
        {
            Destroy(element.Value);
        }
        playerPanels.Clear();

        /* Generate a new panel for each other player. Order them according to normal order of play */
        int meIndex = GameManager.Instance.Players.IndexOf(GameManager.Instance.me);
        for(int i = 0; i < players.Count - 1; i++)
        {
            int actualIndex = (meIndex + 1 + i) % players.Count;
            var player = players[actualIndex];
            var panel = Instantiate(pf_PlayerPanel, PlayerContainer);
            playerPanels.Add(player, panel);
            panel.UpdatePanel(player);
        }
        
    }

    public void UpdatePlayer(simulation.Player player)
    {
        if (playerPanels.ContainsKey(player))
        {
            playerPanels[player].UpdatePanel(player);
        }
    }
}
