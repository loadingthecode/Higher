using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX : MonoBehaviour
{
    Material dissolve;

    bool isActive = false;
    float fade = 1f;

    // Start is called before the first frame update
    void Start()
    {
        dissolve = GetComponent<SpriteRenderer>().material;
    }

    public void dissolveCard(GameObject card)
    {
        StopCoroutine(callDissolveCard(card));
        StartCoroutine(callDissolveCard(card));
    }

    public IEnumerator callDissolveCard(GameObject card)
    {
        isActive = true;

        if (isActive)
        {
            while (fade != 0)
            {
                yield return new WaitForSeconds(0.05f);
                fade -= Time.deltaTime;

                if (fade <= 0f)
                {
                    fade = 0f;
                    isActive = false;
                }
                card.GetComponent<SpriteRenderer>().material.SetFloat("_Fade", fade);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isActive = true;
        }

        if (isActive)
        {
            fade -= Time.deltaTime;

            if (fade <= 0f)
            {
                fade = 0f;
                isActive = false;
            }

            dissolve.SetFloat("_Fade", fade);
            print(dissolve.GetFloat("_Fade"));
        }
    }
}
