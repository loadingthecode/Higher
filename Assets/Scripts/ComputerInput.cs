using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;

public class ComputerInput : MonoBehaviour
{
    public Higher higher;
    public GameObject selectedCard;

    private bool cameFromOtherDefaultBehavior;
    private int indexOfSelectedCard;
    // Start is called before the first frame update
    void Start()
    {
        higher = GetComponent<Higher>();
        selectedCard = this.gameObject;
        cameFromOtherDefaultBehavior = false;
        indexOfSelectedCard = 0;
    }

    public void chooseCard()
    {
        // defaultBehaviorA will choose the card in hand that will exceed point value by the least amount over the player's point value
        // defaultBehaviorB will choose a special card
        // if the player runs out of the respective defaultBehaviorA or B cards, then it will swap to the other one.
        // if both default behaviors are out, well the computer lost.
        // for now let the chance of either default behavior per turn be 50% chance to activate (for a total of 100%)
        // int to control chance of either doing defaultBehaviorA or defaultBehaviorB
        // also need to check if hit with sun card in the previous turn WITHIN BOTH BEHAVIORS
        selectedCard = this.gameObject;
        cameFromOtherDefaultBehavior = false;


        // if hit by a Sun card in the previous turn, prioritize looking for a pluto card
        // if findPlutoCard fails to change selectedCard, it will remain at this.gameObject
        // therefore we just go pick as normal
        if (higher.cMiddleCards[higher.cMiddleCards.Count - 1].GetComponent<Selectable>().faceUp == false)
        {
            print("Computer was hit by a Sun card last turn, so checking for available Pluto card.");
            findPlutoCard();
        }

        if (selectedCard == this.gameObject)
        {
            int behaviorPick = Random.Range(0, 100);
            print("Computer was not hit b ya Sun card, so just going through normal algorithm.");
            // defaultBehaviorA will include behaviorPick nums ranging from 0 - 49
            if (behaviorPick >= 0 && behaviorPick <= 49)
            {
                findLeastBiggerCard();
            }
            // defaultBehaviorB will include behaviorPick nums ranging from 50 - 99
            else
            {
                pickSpecialCard();
            }
        }

        higher.cFieldCards.RemoveAt(indexOfSelectedCard);// to ensure we don't accidentally point to null
        print("Computer chooses " + selectedCard);
        higher.SelectCard(selectedCard);
    }

    void findPlutoCard()
    {
        // before this method is called, the fact that computer was hit by a sun card on the last turn will be checked
        // if hit by a sun card in the previous turn, priority goes to selecting a pluto card
        // if there is no pluto card in the hand
        // then pick as usual

        for (int i = 0; i < higher.cFieldCards.Count; i++)
        {
            GameObject currentCard = higher.cFieldCards[i];
            if (currentCard.GetComponent<Selectable>().type == "P" 
                && currentCard.GetComponent<Selectable>().value == 1)
            {
                print("Computer found Pluto card.");
                selectedCard = currentCard;
                indexOfSelectedCard = i;
                break;
            }
        }
        print("Computer DID NOT find Pluto card.");
    }

    // defaultBehaviorA
    void findLeastBiggerCard()
    {
        // keep track of leastBigger value using a leastBigger int
        // set this equal to 100 or some really big number (it's acting like a MIN value)
        // loop through remaining cards in computer's deck
        // compare the value at each card vs. the player's total score by
        // by adding the value of the current card to the computer's score 
        // compare the difference between the resulting computer score - player's total score
        // if the difference is smaller than the current leastBigger value, replace the leastBigger value with that difference
        // else move on
        // if you get to the end and all the resulting computer scores were negative, resort to defaultBehaviorB (special cards)

        int leastBiggerVal = 1000;
        GameObject leastBiggerCard =  new GameObject();
        // in case there are no bigger val cards, you want to at least keep 1 equalizer card as a back up
        // e.g. you need a card of value 8 or greater to surpass the player's but your biggest card is a 7
        // that means at least the 7 will let you draw, so keep track of that card's index in the worst case scenario
        bool equalizerFound = false;
        int indexOfEqualizer = 0;
        GameObject anEqualizerCard = new GameObject(); 

        for (int i = 0; i < higher.cFieldCards.Count; i++)
        {
            int scoreDifference = ComputerScoreKeeper.scoreValue + higher.cFieldCards[i].GetComponent<Selectable>().value - PlayerScoreKeeper.scoreValue;
            if (higher.cFieldCards[i].GetComponent<Selectable>().type == "P")
            {
                if (scoreDifference < leastBiggerVal && scoreDifference > 0)
                {
                    leastBiggerVal = scoreDifference;
                    leastBiggerCard = higher.cFieldCards[i];
                    indexOfSelectedCard = i;
                }
                else if (scoreDifference == 0)
                {
                    equalizerFound = true;
                    anEqualizerCard = higher.cFieldCards[i];
                    indexOfEqualizer = i;
                }
            }
        }

        // if leastBiggerVal didn't change (meaning no valid leastBigger card was found)
        // first check if there is anEqualizerCard
        // if there is, select that
        // if not, go to default behaviorB (we need to check if they already came from default behaviorB)
        // if they already came from default behaviorB, then just return a random planet card (computer gives up)
        if (leastBiggerVal == 1000)
        {
            if (equalizerFound == true)
            {
                selectedCard = anEqualizerCard;
                indexOfSelectedCard = indexOfEqualizer;
            }
            else if (cameFromOtherDefaultBehavior == false)
            {
                // set this to true
                // call defaultBehaviorB
                cameFromOtherDefaultBehavior = true;
                pickSpecialCard();
            }
            else
            {
                // select a random planet card
                int randomIndex = Random.Range(0, higher.cFieldCards.Count);
                indexOfSelectedCard = randomIndex;
                selectedCard = higher.cFieldCards[randomIndex];
            }
        } 
        else
        {
            selectedCard = leastBiggerCard; 
        }
    }

    void pickSpecialCard()
    {
        // look for any special card (for now)
        // if you find one, stop immediately and just pick it
        // else if you don't find any,
        // check if you've come from defaultBehaviorA
        // if you have, pick a random card
        // if you haven't, go to defaultBehaviorB

        GameObject specialCard = this.gameObject;

        for (int i = 0; i < higher.cFieldCards.Count; i++)
        {
            string cardType = higher.cFieldCards[i].GetComponent<Selectable>().type;
            if ( cardType == "S" || cardType == "W")
            {
                specialCard = higher.cFieldCards[i];
                indexOfSelectedCard = i;
                break;
            }
        }

        if (specialCard != this.gameObject && (specialCard.GetComponent<Selectable>().type == "S" || specialCard.GetComponent<Selectable>().type == "W"))
        {
            selectedCard = specialCard;
        }
        else if (cameFromOtherDefaultBehavior == false)
        {
            // set this to true
            // call other defaultBehavior
            cameFromOtherDefaultBehavior = true;
            findLeastBiggerCard();
        }
        else
        {
            // select a random planet card
            int randomIndex = Random.Range(0, higher.cFieldCards.Count);
            selectedCard = higher.cFieldCards[randomIndex];
            indexOfSelectedCard = randomIndex;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
