using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardFlipper : MonoBehaviour
{
    UpdateSprite updateSprite;
    // Start is called before the first frame update
    void Start()
    {
        updateSprite = GetComponent<UpdateSprite>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FlipCard()
    {
        StopCoroutine(Flip());
        StartCoroutine(Flip());
    }

    IEnumerator Flip()
    {
        yield return new WaitForSeconds(2f);
    }
}
