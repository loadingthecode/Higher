using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

// class responsible for controlling the visible sprite face
// of each card
public class UpdateSprite : MonoBehaviour
{
    public Sprite cardFace;
    public Sprite cardBack;
    private Selectable selectable;
    private SpriteRenderer spriteRenderer;
    private Higher higher;
    private PlayerInput playerInput;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        List<string> deck = Higher.GenerateDeck();
        higher = FindObjectOfType<Higher>();

        int i = 0;
        foreach (string card in deck)
        {
            //print(card);
            if (this.name == "P1")
            {
                cardFace = higher.cardFaces[0];
                break;
            }

            if (this.name == "P2")
            {
                cardFace = higher.cardFaces[1];
                break;
            }

            if (this.name == "P3")
            {
                cardFace = higher.cardFaces[2];
                break;
            }

            if (this.name == "P4")
            {
                cardFace = higher.cardFaces[3];
                break;
            }

            if (this.name == "P5")
            {
                cardFace = higher.cardFaces[4];
                break;
            }

            if (this.name == "P6")
            {
                cardFace = higher.cardFaces[5];
                break;
            }

            if (this.name == "P7")
            {
                cardFace = higher.cardFaces[6];
                break;
            }

            if (this.name == "P8")
            {
                cardFace = higher.cardFaces[7];
                break;
            }

            if (this.name == "P9")
            {
                cardFace = higher.cardFaces[8];
                break;
            }

            if (this.name == "S1")
            {
                cardFace = higher.cardFaces[9];
                break;
            }

            if (this.name == "W1")
            {
                cardFace = higher.cardFaces[10];
                break;
            }
            
            i++;
        }
        selectable = GetComponent<Selectable>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void attachFaces()
    {
       

    }


    // Update is called once per frame
    void Update()
    {
        if (selectable.faceUp == true)
        {
            spriteRenderer.sprite = cardFace;
        }
        else if (selectable.faceUp == false)
        {
            spriteRenderer.sprite = cardBack;
        }
    }
}
