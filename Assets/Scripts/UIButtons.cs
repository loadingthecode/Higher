using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIButtons : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetScene()
    {
        ComputerScoreKeeper.scoreValue = 0;
        PlayerScoreKeeper.scoreValue = 0;
        SceneManager.LoadScene("SampleScene");
    }
}
