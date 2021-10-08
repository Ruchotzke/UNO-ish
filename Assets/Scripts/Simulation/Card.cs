using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace simulation
{
    /// <summary>
    /// A card representation. Struct for easier networking.
    /// </summary>
    public struct Card
    {
        public int value;
        public CardColor color;
        public CardType type;

        public Card(CardColor color, CardType type, int value = 0)
        {
            this.value = value;
            this.color = color;
            this.type = type;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(Card)) return false;

            Card other = (Card)obj;
            return this.value == other.value && this.color == other.color && this.type == other.type;
        }

        public override string ToString()
        {
            return "{ " + color + ((type == CardType.NUMBER) ? " " + value + "}" : " " + type + "}");
        }
    }

    public enum CardColor
    {
        RED,
        GREEN,
        BLUE,
        YELLOW,
        WILD
    }

    public enum CardType
    {
        NUMBER,
        SKIP,
        REVERSE,
        DRAW_2,
        DRAW_4,
        WILD
    }
}

