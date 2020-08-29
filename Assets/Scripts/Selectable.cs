using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour
{
    public bool faceUp = false;
    public string type;
    public int value;
    public bool inDeckPile;
    public bool inMiddle = false;

    private string valueString;
    // Start is called before the first frame update
    void Start()
    {
        if (CompareTag("Card") || CompareTag("cCard"))
        {
            type = transform.name[0].ToString();

            for (int i = 1; i < transform.name.Length; i++)
            {
                char c = transform.name[i];
                valueString = valueString + c.ToString();
            }

            if (valueString == "1")
            {
                value = 1;
            }

            if (valueString == "2")
            {
                value = 2;
            }

            if (valueString == "3")
            {
                value = 3;
            }

            if (valueString == "4")
            {
                value = 4;
            }

            if (valueString == "5")
            {
                value = 5;
            }

            if (valueString == "6")
            {
                value = 6;
            }

            if (valueString == "7")
            {
                value = 7;
            }

            if (valueString == "8")
            {
                value = 8;
            }

            if (valueString == "9")
            {
                value = 9;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
