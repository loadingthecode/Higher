using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum LossReason
{
    FOUL, NOCARDS, INSUFFICIENTVALUE, NA
}

public class MatchEndScreen : MonoBehaviour
{
    public Higher higher;
    public Text reasonText;
    public static LossReason lossReason;

    // Start is called before the first frame update
    void Start()
    {
        ComputerScoreKeeper.scoreValue = 0;
        PlayerScoreKeeper.scoreValue = 0;
        higher = GetComponent<Higher>();
        reasonText = GameObject.FindGameObjectWithTag("LossReasonText").GetComponent<Text>();
        PrintLossReason(lossReason);
    }

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void PrintLossReason(LossReason lossReason)
    {
        if (lossReason == LossReason.INSUFFICIENTVALUE)
        {
            reasonText.text = "The card you selected did not have enough value.";
        }
        else if (lossReason == LossReason.FOUL)
        {
            reasonText.text = "A special card cannot be selected if it is the last card in your hand.";
        }
        else if (lossReason == LossReason.NOCARDS)
        {
            reasonText.text = "You have no more cards in your hand.";
        }
    }

    public void Restart()
    {
        print("Restarting game.");
        SceneManager.LoadScene("Higher");
    }

    public void Quit()
    {
        print("Quitting game.");
        Application.Quit();
    }
}
