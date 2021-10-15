using ExitGames.Client.Photon;
using simulation;
using System.Collections.Generic;
using UnityEngine;

public class Utilities 
{
    public static int mod(int x, int m)
    {
        return (x % m + m) % m;
    }

    public static void Shuffle<T>(List<T> toShuffle)
    {
        for(int i = toShuffle.Count - 1; i >= 0; i--)
        {
            int index = Random.Range(0, i + 1);
            if (i == index) continue;

            T temp = toShuffle[i];
            toShuffle[i] = toShuffle[index];
            toShuffle[index] = temp;
        }
    }


    #region CARD SERIALIZATION
    public static readonly byte[] serialCardMemory = new byte[12]; //12 bytes = 3 ints for a card
    public static short SerializeCard(StreamBuffer outStream, object cardToSerialize)
    {
        Card card = (Card)cardToSerialize;

        lock (serialCardMemory)
        {
            byte[] bytes = serialCardMemory;
            int index = 0;
            Protocol.Serialize(card.value, bytes, ref index);
            Protocol.Serialize((short)card.color, bytes, ref index);
            Protocol.Serialize((short)card.type, bytes, ref index);
            outStream.Write(bytes, 0, 12);
        }

        return 12;
    }

    public static object DeserializeCard(StreamBuffer inStream, short length)
    {
        Card card = new Card();

        lock (serialCardMemory)
        {
            inStream.Read(serialCardMemory, 0, 12);
            int index = 0;
            short color, type;

            Protocol.Deserialize(out card.value, serialCardMemory, ref index);
            Protocol.Deserialize(out color, serialCardMemory, ref index);
            Protocol.Deserialize(out type, serialCardMemory, ref index);

            card.color = (CardColor)color;
            card.type = (CardType)type;
        }

        return card;
    }
    #endregion
}
