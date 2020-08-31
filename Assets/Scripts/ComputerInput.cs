using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerInput : MonoBehaviour
{
    public Higher higher;

    public GameObject selectedCard;
    // Start is called before the first frame update
    void Start()
    {
        higher = GetComponent<Higher>();
        selectedCard = this.gameObject;
    }

    public void chooseCard()
    {
        int choiceMaxRange = higher.cFieldCards.Count;
        int indexOfSelectedCard = Random.Range(0, choiceMaxRange); // pick a random card within how many cards are left in field

        // access cFieldCards at the random index
        // and that specific card will have its Selectable value added to the middle of his point total
        // and also move to the correct middle position
        selectedCard = higher.cFieldCards[indexOfSelectedCard];
        higher.cFieldCards.RemoveAt(indexOfSelectedCard);// to ensure we don't accidentally point to null
        print("Computer chooses " + selectedCard);
        higher.SelectCard(selectedCard);
    }



    // Update is called once per frame
    void Update()
    {

    }
}
