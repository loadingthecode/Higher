using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreKeeper : MonoBehaviour
{
    public static int scoreValue = 0;

    Text score;
    Color originalColor;

    public void callChangeColor()
    {
        StopCoroutine(changeColor());
        StartCoroutine(changeColor());
    }

    private IEnumerator changeColor()
    {
        score.color = Color.green;
        yield return new WaitForSeconds(0.5f);
        score.color = originalColor;
    }

    // Start is called before the first frame update
    void Start()
    {
        score = GetComponent<Text>();
        originalColor = score.color;
    }

    // Update is called once per frame
    void Update()
    {
        score.text = "" + scoreValue;
    }
}
