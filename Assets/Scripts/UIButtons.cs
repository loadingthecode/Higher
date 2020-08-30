using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIButtons : MonoBehaviour
{
    public Higher higher;
    public Selectable selectable;

    // Start is called before the first frame update
    void Start()
    {
        higher = GetComponent<Higher>();
        selectable = GetComponent<Selectable>();
    }

    public void ResetScene()
    {
        ComputerScoreKeeper.scoreValue = 0;
        PlayerScoreKeeper.scoreValue = 0;
        SceneManager.LoadScene("Higher");
    }

    public void FlipComputerCards()
    {
        foreach (GameObject card in higher.cFieldCards)
        {
            card.GetComponent<Selectable>().faceUp = !(card.GetComponent<Selectable>().faceUp);
        }
    }
}
