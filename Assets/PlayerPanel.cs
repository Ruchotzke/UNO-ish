using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI PlayerName;
    [SerializeField] RawImage pf_CardGraphic;
    [SerializeField] Transform CardContainer;
    [SerializeField] TextMeshProUGUI NumCardText;

    List<RawImage> cards = new List<RawImage>();

    public void UpdatePanel(simulation.Player player)
    {
        PlayerName.text = player.player.NickName;
        NumCardText.text = player.hand.Count.ToString() + " cards";

        if(cards.Count < player.hand.Count)
        {
            int diff = player.hand.Count - cards.Count;
            for (int i = 0; i < diff; i++)
            {
                /* Add a new card */
                var instance = Instantiate(pf_CardGraphic, CardContainer);
                cards.Add(instance);
                instance.color = Random.ColorHSV();
            }
        }
        else if(cards.Count > player.hand.Count)
        {
            int diff = cards.Count - player.hand.Count;
            for (int i = 0; i < diff; i++)
            {
                /* Remove a card */
                Destroy(cards[0].gameObject);
                cards.RemoveAt(0);
            }
        }
    }
}
