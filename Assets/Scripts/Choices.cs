using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Choices : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ComputerScoreKeeper.scoreValue = 0;
        PlayerScoreKeeper.scoreValue = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
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
