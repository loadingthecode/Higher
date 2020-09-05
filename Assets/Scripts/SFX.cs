using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX : MonoBehaviour
{
    Material dissolve;
    Material scorch;

    bool isDissolving = false;
    bool isRecovering = false;
    float fade = 1f;

    bool scorched = false;

    // Start is called before the first frame update
    void Start()
    {
        dissolve = GetComponent<SpriteRenderer>().material;
    }

    public void scorchCard(GameObject card)
    {
        if (scorched == false)
        {
            card.GetComponent<SpriteRenderer>().material.SetFloat("_Scorched", 1);
        }
    }

    public void dissolveCard(GameObject card)
    {  
        StopCoroutine(callDissolveCard(card));
        StartCoroutine(callDissolveCard(card));
    }

    public IEnumerator callDissolveCard(GameObject card)
    {
        isDissolving = true;

        while (fade >= 0.60f && isDissolving == true)
        {
            yield return new WaitForSeconds(0.002f);
            fade -= Time.deltaTime;

            if (fade <= 0.60f)
            {
                fade = 0.60f;
                isDissolving = false;
            }
            card.GetComponent<SpriteRenderer>().material.SetFloat("_Fade", fade);
        }
    }

    public void restoreCard(GameObject card)
    {
        StopCoroutine(callRestoreCard(card));
        StartCoroutine(callRestoreCard(card));
    }

    public IEnumerator callRestoreCard(GameObject card)
    {
        isRecovering = true;
        fade = 0.60f;

        while (fade < 1f && isRecovering)
        {
            yield return new WaitForSeconds(0.001f);
            fade = fade + Time.deltaTime * 2f; // why can't we use delta time

            if (fade >= 1f)
            {
                fade = 1f;
                isRecovering = false;
            }
            card.GetComponent<SpriteRenderer>().material.SetFloat("_Fade", fade);
            print(fade);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
