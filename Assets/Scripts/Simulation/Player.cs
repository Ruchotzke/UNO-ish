using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace simulation
{
    public class Player
    {
        public List<Card> hand = new List<Card>();
        public Photon.Realtime.Player player;
    }
}