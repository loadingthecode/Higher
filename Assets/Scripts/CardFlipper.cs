using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CardFlipper : MonoBehaviour
{
    public int timer;

    public void StartFlip(GameObject card)
    {
        StartCoroutine(CalculateFlip(card));
    }

    public void Flip(GameObject card)
    {
        if (card.GetComponent<Selectable>().faceUp == true)
        {
            card.GetComponent<Selectable>().faceUp = false;
        }
        else
        {
            card.GetComponent<Selectable>().faceUp = true;
        }
    }

    IEnumerator CalculateFlip(GameObject card)
    {
        for (int i = 0; i < 180; i ++)
        {
            yield return new WaitForSeconds(0.002f);
            card.transform.Rotate(new Vector3(0f, 1f, 0f));
            timer++;

            if (timer == 90)
            {
                Flip(card);
            }
        }
        timer = 0;
    }

}
