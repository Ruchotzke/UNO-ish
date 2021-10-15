using simulation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGraphics : MonoBehaviour
{
    #region SINGLETON
    public static CardGraphics Instance
    {
        get
        {
            if (_singleton == null) _singleton = FindObjectOfType<CardGraphics>();
            return _singleton;
        }
    }
    private static CardGraphics _singleton;
    #endregion

    [SerializeField] Texture2D FullTexture;

    public Dictionary<Card, Texture2D> Graphics = new Dictionary<Card, Texture2D>();


    private void Awake()
    {
        GenerateCardTextures();
    }

    private void GenerateCardTextures()
    {
        Graphics.Clear();

        /* Split the texture into card sized pieces */
        /* Each card is 500x750 */
        Vector2Int CardSize = new Vector2Int(500, 750);

        /* First get the main sequence */
        for(int y = 7; y > 3; y--)
        {
            for(int x = 0; x < 14; x++)
            {
                /* We only need 1 copy of the wild card */
                if (x == 13 && y != 7) continue;

                /* Generate a texture */
                Texture2D newTex = new Texture2D(CardSize.x, CardSize.y);
                newTex.SetPixels(0, 0, CardSize.x, CardSize.y, FullTexture.GetPixels(x * CardSize.x, y * CardSize.y, CardSize.x, CardSize.y));
                newTex.Apply();

                /* Generate the corresponding card */
                int value = x;
                CardColor color = (x == 13) ? CardColor.WILD : (CardColor)(7-y);
                CardType type = (x < 10) ? CardType.NUMBER : (CardType)(x - 9);

                /* Update the dictionary */
                Graphics.Add(new Card(color, type, value), newTex);
            }
        }

        /* Generate the final draw 4 wild */
        Texture2D wildTex = new Texture2D(CardSize.x, CardSize.y);
        wildTex.SetPixels(0, 0, CardSize.x, CardSize.y, FullTexture.GetPixels(13 * CardSize.x, 0, CardSize.x, CardSize.y));
        wildTex.Apply();
        Graphics.Add(new Card(CardColor.WILD, CardType.DRAW_4, 0), wildTex);
    }
}
