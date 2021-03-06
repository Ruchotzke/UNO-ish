using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiscardDragTarget : DragTarget
{
    private RawImage image;

    private void Start()
    {
        image = GetComponent<RawImage>();
    }

    public override bool OnDragInto(GameObject dragObject)
    {
        CardGraphic cg = dragObject.GetComponent<CardGraphic>();

        if (cg == null) return false;

        image.texture = CardGraphics.Instance.Graphics[cg.card];
        NetworkManager.Instance.photonView.RPC("CardPlayed", Photon.Pun.RpcTarget.Others, GameManager.Instance.me.player, cg.card);
        GameManager.Instance.me.hand.Remove(cg.card);
        GameManager.Instance.DisplayDeck();

        return true;
    }
}
