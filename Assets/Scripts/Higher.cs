using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public enum GameState
{
    START, DRAWCARD, MOVECHECKER, PLAYERTURN, COMPUTERTURN, WON, LOST
}
public class Higher : MonoBehaviour
{
    public static GameState state;

    public ComputerInput computerInput;

    public Sprite[] cardFaces;
    public GameObject cardPrefab;
    public UpdateSprite updateSprite;
    public SpriteRenderer prevSpriteRenderer;
    public CardFlipper cardFlipper;

    public static string[] types = new string[] { "P", "S", "W" };
    public static string[] values = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9" };

    public List<string> pDeck;
    public List<string> cDeck;

    public List<string> pField;
    public List<string> cField;

    public List<GameObject> pMiddleCards;
    public List<GameObject> cMiddleCards;

    public List<GameObject> pPhysicalDeck;
    public List<GameObject> cPhysicalDeck;

    public List<GameObject> pFieldCards; // no more than 10
    public List<GameObject> cFieldCards; // no more than 10

    // Start is called before the first frame update
    void Start()
    {
        state = GameState.START;
        prevSpriteRenderer = GetComponent<SpriteRenderer>();
        computerInput = GetComponent<ComputerInput>();
        cardFlipper = GetComponent<CardFlipper>();
        Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play()
    {
        StopCoroutine(PlayCards());
        StartCoroutine(PlayCards());
    }

    IEnumerator PlayCards()
    {

        pDeck.Clear();
        cDeck.Clear();
        pField.Clear();
        cField.Clear();

        print("Shuffling deck.");

        pDeck = GenerateDeck();
        cDeck = GenerateDeck();

        Shuffle(pDeck);
        Shuffle(pDeck); // shuffling twice because of possible seed value duplicate resulting in same random deck
        Shuffle(cDeck);

        populateField();

        Deal();

        yield return new WaitForSeconds(2f);

        state = GameState.DRAWCARD; // dealing phase ends, now both players enter card drawing state
    }

    public void populateField()
    {
        for (int i = 0; i < 10; i++) // want to transfer first 10 cards from each deck to respective fields
        {
            pField.Add(pDeck[i]);
            pDeck.Remove(pDeck[i]);
            cField.Add(cDeck[i]);
            cDeck.Remove(cDeck[i]);
        }
    }

    // make it so that the player cannot select the cards on the computer's side
    public void lockComputerCards()
    {
        print("locking computer cards");
        foreach (GameObject card in cPhysicalDeck)
        {
            card.tag = "cCard";
        }

        foreach (GameObject card in cFieldCards)
        {
            card.tag = "cCard";
        }
    }

    public static List<string> GenerateDeck()
    {
        List<string> newDeck = new List<string>();
       
        for (int i = 0; i < 2; i++)
        {
            newDeck.Add(types[0] + values[0]); // adding 2 (1)'s to the deck
        }

        for (int i = 0; i < 3; i++)
        {
            newDeck.Add(types[0] + values[1]); // adding 3 (2)'s to the deck
        }

        for (int i = 0; i < 4; i++)
        {
            newDeck.Add(types[0] + values[2]); // adding 4 (3)'s to the deck
        }

        for (int i = 0; i < 4; i++)
        {
            newDeck.Add(types[0] + values[3]); // adding 4 (4)'s to the deck
        }

        for (int i = 0; i < 4; i++)
        {
            newDeck.Add(types[0] + values[4]); // adding 4 (5)'s to the deck
        }

        for (int i = 0; i < 3; i++)
        {
            newDeck.Add(types[0] + values[5]); // adding 3 (6)'s to the deck
        }

        for (int i = 0; i < 2; i++)
        {
            newDeck.Add(types[0] + values[6]); // adding 2 (7)'s to the deck
        }

        for (int i = 0; i < 2; i++)
        {
            newDeck.Add(types[0] + values[7]); // adding 2 (8)'s to the deck
        }

        for (int i = 0; i < 2; i++)
        {
            newDeck.Add(types[0] + values[8]); // adding 2 (9)'s to the deck
        }

        for (int i = 0; i < 3; i++)
        {
            newDeck.Add(types[1] + values[0]); // adding 3 suns (1) to the deck
        }

        for (int i = 0; i < 3; i++)
        {
            newDeck.Add(types[2] + values[0]); // adding 3 wormholes (1) to the deck
        }
        return newDeck;
    }

    void Shuffle<T>(List<T> list)
    {
        System.Random random = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            int k = random.Next(n);
            n--;
            T temp = list[k];
            list[k] = list[n];
            list[n] = temp;
        }
    }

    IEnumerator DealCards()
    {
        float pXOffset = 0;
        float pZOffset = 0.03f;

        float cXOffset = 0;
        float cZOffset = 0.03f;

        GameObject pDeckButton = GameObject.Find("PlayerDeckButton"); // used to get the transform of deck location
        GameObject cDeckButton = GameObject.Find("ComputerDeckButton");

        // for fields
        for (int i = 0; i < 10; i++) 
        {
            yield return new WaitForSeconds(0.1f); // spawns cards one-by-one visually
            GameObject pNewCard;
            GameObject cNewCard;

            pNewCard = Instantiate(cardPrefab, new Vector3(-5.79f + pXOffset, -4f, transform.position.z + pZOffset), Quaternion.identity);
            cNewCard = Instantiate(cardPrefab, new Vector3(5.57f + cXOffset, 4f, transform.position.z + cZOffset), new Quaternion(0, 0, 180, 0));
             
            pNewCard.name = pField[i];
            cNewCard.name = cField[i];

            pFieldCards.Add(pNewCard);

            cFieldCards.Add(cNewCard);

            pNewCard.GetComponent<Selectable>().faceUp = true;
            cNewCard.GetComponent<Selectable>().faceUp = false;

            pXOffset = pXOffset + 1.5f;
            pZOffset = pZOffset + 0.03f;
            cXOffset = cXOffset - 1.5f;
            cZOffset = cZOffset + 0.03f;
        }

        // for decks
        for (int i = 0; i < 22; i++)
        {
            GameObject pNewCard;
            GameObject cNewCard;

            // the rest stay in the player's deck
            // note: +1f for z transform because deck needs to be in front of all the card prefabs
            pNewCard = Instantiate(cardPrefab, new Vector3(pDeckButton.transform.position.x, pDeckButton.transform.position.y, pDeckButton.transform.position.z + 1f), Quaternion.identity);
            cNewCard = Instantiate(cardPrefab, new Vector3(cDeckButton.transform.position.x, cDeckButton.transform.position.y, cDeckButton.transform.position.z + 1f), new Quaternion(0, 0, 180, 0));

            pNewCard.name = pDeck[i];
            cNewCard.name = cDeck[i];

            // ** still testing for draw card **
            pPhysicalDeck.Add(pNewCard);

            cPhysicalDeck.Add(cNewCard);

            pNewCard.GetComponent<Selectable>().faceUp = false;
            cNewCard.GetComponent<Selectable>().faceUp = false;

            pNewCard.GetComponent<Selectable>().inDeckPile = true;
            cNewCard.GetComponent<Selectable>().inDeckPile = true;
        }

        // prevent selection of computer cards by player
        lockComputerCards();
    }

    // will draw the first card it sees from pDeck
    // turn it face up, move it to the middle
    // call on PlayerScoreKeeper to increment player's score by the card's value
    public IEnumerator DrawCard()
    {
        if (state == GameState.DRAWCARD)
        {
            print("Cards are being drawn.");

            // draws the first card from the deck (top)
            // then removes it from the deck
            // gets value of card and adds it to total player score
            // and of course flips the card up to its face
            // and moves the card to the middle arraylist

            // after  the player draw a card
            // move on to the MOVECHECKER state to lock out any more card draws
            state = GameState.MOVECHECKER;

            pMiddleCards.Add(pPhysicalDeck[0]); // in case we need to destroy it in the case of a draw
            pMiddleCards[0].GetComponent<Selectable>().inMiddle = true; // to make sure the player can't click on the middle card to gain value
            pPhysicalDeck.RemoveAt(0);
            int pDrawnCardValue = pMiddleCards[0].GetComponent<Selectable>().value;
            PlayerScoreKeeper.scoreValue += pDrawnCardValue;
            print(pMiddleCards[0].name + " was drawn by the player.");
            pMiddleCards[0].GetComponent<Selectable>().faceUp = true;
            pMiddleCards[0].transform.position = new Vector3(2, 0, 0); // right next to the player's score counter

            yield return new WaitForSeconds(1f); // a delay between both draws to allow the player to understand what just happened

            cMiddleCards.Add(cPhysicalDeck[0]);
            cPhysicalDeck.RemoveAt(0);
            int cDrawnCardValue = cMiddleCards[0].GetComponent<Selectable>().value;
            ComputerScoreKeeper.scoreValue += cDrawnCardValue;
            print(cMiddleCards[0].name + " was drawn by the computer.");
            cMiddleCards[0].GetComponent<Selectable>().faceUp = true;
            cMiddleCards[0].transform.position = new Vector3(-2, 0, 0);

            yield return new WaitForSeconds(1f); // a delay between both draws to allow the player to understand what just happened
            
            whoGoesFirst();
        }
    }

    public void whoGoesFirst()
    {
        if (state == GameState.MOVECHECKER)
        {
            if (PlayerScoreKeeper.scoreValue > ComputerScoreKeeper.scoreValue)
            {
                // if the Computer draws the last card in his hand and goes to redrawing phase
                // AND also draws a lower card in the redrawing phase, they lose
                if (cFieldCards.Count != 0)
                {
                    print("Computer scored lower and has no cards left in play. Game over.");
                    state = GameState.WON;
                }
                print("Player scored higher, therefore computer goes first.");
                state = GameState.COMPUTERTURN;
                print("It is now computer turn.");
                computerInput.chooseCard();
            } 
            else if (PlayerScoreKeeper.scoreValue < ComputerScoreKeeper.scoreValue)
            {
                // if the player draws the last card in his hand and goes to redrawing phase
                // AND also draws a lower card in the redrawing phase, they lose
                if (pFieldCards.Count != 0)
                {
                    print("Player scored lower and has no cards left in play. Game over.");
                    state = GameState.LOST;
                }
                print("Computer scored higher, therefore player goes first.");
                state = GameState.PLAYERTURN;
            } 
            else
            {
                print("Player and computer score are the same. Entering DRAWCARD state.");
                RedrawCard();
            }
        }
    }

    public void Deal()
    {
        StopCoroutine(DealCards());
        StartCoroutine(DealCards());
    }

    public void Draw()
    {
        StopCoroutine(DrawCard());
        StartCoroutine(DrawCard());
    }

    public void Select(GameObject selected)
    {
        StopCoroutine(SelectCard(selected));
        StartCoroutine(SelectCard(selected));
    }



    // should encapsuale some of this in a "comparePoints" method later on dude
    public IEnumerator SelectCard(GameObject selected)
    {
        // selected card has its value added to the players pool if it would make the total at least equal or greater to the enemy pool
        // and replaces whatever card is currently in the middle
        
        int selectedCardScore = selected.GetComponent<Selectable>().value;
        if (state == GameState.PLAYERTURN)
        {
            state = GameState.MOVECHECKER;
            selected.GetComponent<Selectable>().inMiddle = true; // to make sure the player can't click on the middle card to gain value
            // if the player selects "The Sun" card
            // turn facedown the last card the computer played
            // which removes the points that the card gave to the computer
            if (selected.GetComponent<Selectable>().type == "S")
            {
                print("Player has burned the computer's "+ cMiddleCards[cMiddleCards.Count - 1] + " card");

                UseSunCard(selected);
            }
            //// if the player selects "Wormhole" card
            //// flip the player and computer's score
            else if (selected.GetComponent<Selectable>().type == "W")
            {
                print("player has switched their score with the computer's using a wormhole card.");       
                // a temp arraylist that holds one of the player's middle cards list
                // transfer one player's arraylist contents into the temp arraylist
                // switch the transforms of both the computer and the player's cards
                // transfer the other player's arraylist contents into the first player's arraylist
                // transfer the temp arraylist content int othe other player's arraylist

                // switch the card positions
                for (int i = 0; i < pMiddleCards.Count; i++)
                {
                    pMiddleCards[i].transform.position = new Vector3(-1 * (i + 2), 0, -1 * i); // selected card goes to middle card position.
                    pMiddleCards[i].transform.rotation = new Quaternion(0, 0, 180, 0);
                }

                for (int i = 0; i < cMiddleCards.Count; i++)
                {
                    cMiddleCards[i].transform.position = new Vector3(i + 2, 0, -1 * i); // selected card goes to middle card position.
                    cMiddleCards[i].transform.rotation = new Quaternion(0, 0, 0, 0);
                }

                // switch the list pointers
                List<GameObject> tempList = pMiddleCards;
                pMiddleCards = cMiddleCards;
                cMiddleCards = tempList;

                // switches their scores
                int temp = PlayerScoreKeeper.scoreValue;
                PlayerScoreKeeper.scoreValue = ComputerScoreKeeper.scoreValue;
                ComputerScoreKeeper.scoreValue = temp;

                yield return new WaitForSeconds(1f);
                removePFieldCard(selected);
                Destroy(selected);

                // if the player plays a special card as their last card, they lose
                if (pFieldCards.Count == 0)
                {
                    print("Player has committed a foul. Game over.");
                    state = GameState.LOST;
                    matchEnd();
                }
                else
                {
                    state = GameState.COMPUTERTURN;
                    yield return new WaitForSeconds(1f);
                    computerInput.chooseCard();
                }
            }
            else if (selected.GetComponent<Selectable>().value == 1 && selected.GetComponent<Selectable>().type == "P" 
                && pMiddleCards[pMiddleCards.Count - 1].GetComponent<Selectable>().faceUp == false)
            {
                // revive the burnt card
                // destroy pluto card
                // regain the lost points
                cardFlipper.StartFlip(pMiddleCards[pMiddleCards.Count - 1]);
                yield return new WaitForSeconds(0.5f);
                PlayerScoreKeeper.scoreValue += pMiddleCards[pMiddleCards.Count - 1].GetComponent<Selectable>().value;
                removePFieldCard(selected);
                Destroy(selected);
                state = GameState.COMPUTERTURN;
                yield return new WaitForSeconds(1f); // wait for player to catch up on what's happening
                computerInput.chooseCard();
            }
            else
            {
                if ((PlayerScoreKeeper.scoreValue + selectedCardScore) > ComputerScoreKeeper.scoreValue)
                {
                    print("Player has played a card that allows his score to surpass the computer's.");
                    PlayerScoreKeeper.scoreValue += selectedCardScore;
                    AddMiddle(selected);
                    removePFieldCard(selected);
                    state = GameState.COMPUTERTURN;
                    yield return new WaitForSeconds(1f); // wait for player to catch up on what's happening
                    computerInput.chooseCard();
                }
                else if ((PlayerScoreKeeper.scoreValue + selectedCardScore) < ComputerScoreKeeper.scoreValue)
                {
                    print("Player has played a card with insufficient value. Game over.");
                    PlayerScoreKeeper.scoreValue += selectedCardScore;
                    AddMiddle(selected);
                    removePFieldCard(selected);
                    state = GameState.LOST;
                    matchEnd();
                }
                else // player has picked a card that equalizes their total with the computer's total score
                {
                    print("Player has played a card that equalizes their total with the computer's total score. Redrawing.");
                    PlayerScoreKeeper.scoreValue += selectedCardScore;
                    AddMiddle(selected);
                    removePFieldCard(selected);
                    yield return new WaitForSeconds(1f); // wait for player to catch up on what's happening
                    RedrawCard();
                }
            }
        }
        else if (state == GameState.COMPUTERTURN)
        {
            selected.GetComponent<Selectable>().faceUp = true; // make sure to flip the card up
            // if the Computer selects "The Sun" card
            // destroy the last card the player played
            // which removes the points that the card gave to the player
            if (selected.GetComponent<Selectable>().type == "S")
            {
                print("Computer has played a Sun card.");
                cardFlipper.StartFlip(pMiddleCards[pMiddleCards.Count - 1]);
                yield return new WaitForSeconds(0.5f);
                Math.Max(PlayerScoreKeeper.scoreValue -= pMiddleCards[pMiddleCards.Count - 1].GetComponent<Selectable>().value, 0);
                Destroy(selected); // special cards are used up, not stored in middle

                // if the computer plays a special card as their last card, they lose
                if (cFieldCards.Count == 0)
                {
                    print("Computer has committed a foul. Game won.");
                    state = GameState.WON;
                    matchEnd();
                }
                else
                {
                    state = GameState.PLAYERTURN;
                    yield return new WaitForSeconds(1f);
                }
            }
            // if the computer selects "Wormhole" card
            // flip the computer and player's score
            else if (selected.GetComponent<Selectable>().type == "W")
            {
                print("Computer has switched their score with the player's using a Wormhole card.");

                for (int i = 0; i < pMiddleCards.Count; i++)
                {
                    pMiddleCards[i].transform.position = new Vector3(-1 * (i + 2), 0, -1 * i); // selected card goes to middle card position.
                    pMiddleCards[i].transform.rotation = new Quaternion(0, 0, 180, 0);
                }

                for (int i = 0; i < cMiddleCards.Count; i++)
                {
                    cMiddleCards[i].transform.position = new Vector3(i + 2, 0, -1 * i); // selected card goes to middle card position.
                    cMiddleCards[i].transform.rotation = new Quaternion(0, 0, 0, 0);
                }

                // switch the list pointers
                List<GameObject> tempList = pMiddleCards;
                pMiddleCards = cMiddleCards;
                cMiddleCards = tempList;

                // switches their scores
                int temp = ComputerScoreKeeper.scoreValue;
                ComputerScoreKeeper.scoreValue = PlayerScoreKeeper.scoreValue;
                PlayerScoreKeeper.scoreValue = temp;
                yield return new WaitForSeconds(1f);
                Destroy(selected);

                // if the computer plays a special card as their last card, they lose
                if (cFieldCards.Count == 0)
                {
                    print("Computer has committed a foul. Game won.");
                    state = GameState.WON;
                    matchEnd();
                }
                else
                {
                    state = GameState.PLAYERTURN;
                    yield return new WaitForSeconds(1f);
                }
            }
            else if (selected.GetComponent<Selectable>().value == 1 && selected.GetComponent<Selectable>().type == "P"
                && cMiddleCards[cMiddleCards.Count - 1].GetComponent<Selectable>().faceUp == false)
            {
                // revive the burnt card
                // destroy pluto card
                // regain the lost points
                cardFlipper.StartFlip(cMiddleCards[cMiddleCards.Count - 1]);
                yield return new WaitForSeconds(0.5f);
                PlayerScoreKeeper.scoreValue += cMiddleCards[cMiddleCards.Count - 1].GetComponent<Selectable>().value;
                Destroy(selected);
                state = GameState.PLAYERTURN;
                yield return new WaitForSeconds(1f); // wait for player to catch up on what's happening
            }
            else
            {    
                if ((ComputerScoreKeeper.scoreValue + selectedCardScore) > PlayerScoreKeeper.scoreValue)
                {
                    print("Computer has played a card that allows his score to surpass the player's.");
                    ComputerScoreKeeper.scoreValue += selectedCardScore;
                    AddMiddle(selected);
                    yield return new WaitForSeconds(1f);
                    state = GameState.PLAYERTURN;
                }
                else if ((ComputerScoreKeeper.scoreValue + selectedCardScore) < PlayerScoreKeeper.scoreValue)
                {
                    print("Computer has played a card with insufficient value. Game won.");
                    ComputerScoreKeeper.scoreValue += selectedCardScore;
                    AddMiddle(selected);
                    state = GameState.WON;
                    matchEnd();
                }
                else // computer has picked a card that equalizes their total with the player's total score
                {
                    print("Computer has played a card that equalizes their total with the player's total score. Redrawing.");
                    ComputerScoreKeeper.scoreValue += selectedCardScore;
                    AddMiddle(selected);
                    yield return new WaitForSeconds(1f); // wait for player to catch up on what's happening
                    RedrawCard();
                }
            }
            
            
        }
    }

    public void UseSunCard(GameObject sunCard)
    {
        StopCoroutine(SunCard(sunCard));
        StartCoroutine(SunCard(sunCard));
    }

    // method to cast a Sun card on the opponent
    public IEnumerator SunCard(GameObject sunCard)
    {
       
        if (state == GameState.MOVECHECKER)
        {
            cardFlipper.StartFlip(cMiddleCards[cMiddleCards.Count - 1]);
            yield return new WaitForSeconds(0.5f);
            Math.Max(ComputerScoreKeeper.scoreValue -= cMiddleCards[cMiddleCards.Count - 1].GetComponent<Selectable>().value, 0);
            Destroy(sunCard); // special cards are used up, not stored in middle

            // if the computer plays a special card as their last card, they lose
            if (pFieldCards.Count == 0)
            {
                print("Player has committed a foul. Game lost.");
                state = GameState.LOST;
                matchEnd();
            }
            else
            {
                state = GameState.COMPUTERTURN;
                yield return new WaitForSeconds(1f);
                computerInput.chooseCard();
            }
        } 
        else if (state == GameState.COMPUTERTURN)
        {
            cardFlipper.StartFlip(pMiddleCards[pMiddleCards.Count - 1]);
            yield return new WaitForSeconds(0.5f);
            Math.Max(PlayerScoreKeeper.scoreValue -= pMiddleCards[pMiddleCards.Count - 1].GetComponent<Selectable>().value, 0);
            Destroy(sunCard); // special cards are used up, not stored in middle

            // if the computer plays a special card as their last card, they lose
            if (cFieldCards.Count == 0)
            {
                print("Computer has committed a foul. Game won.");
                state = GameState.WON;
                matchEnd();
            }
            else
            {
                state = GameState.PLAYERTURN;
                yield return new WaitForSeconds(1f);
            }
        }
    }

    // method to remove cards from the player's hand as they are played
    public void removePFieldCard(GameObject card)
    {
        pFieldCards.Remove(card);
        print("Size of player's field cards is now " + pFieldCards.Count);
    }

    // method controlling scene switch based on match end conditions
    public void matchEnd()
    {
        if (state == GameState.LOST)
        {
            print("Player lost. Game over.");
            SceneManager.LoadScene("Lost");
        } 
        else if (state == GameState.WON)
        {
            print("Player won! Game over.");
            SceneManager.LoadScene("Win");
        }
    }
    
    public void AddMiddle(GameObject additionalMiddle)
    {
        int playerMiddleListSize = pMiddleCards.Count;
        int computerMiddleListSize = cMiddleCards.Count;

        if (state == GameState.MOVECHECKER)
        {
            additionalMiddle.transform.position = new Vector3(playerMiddleListSize + 2, 0, -1 * (playerMiddleListSize)); // selected card goes to middle card position.
            pMiddleCards.Add(additionalMiddle);
        }
        else if (state == GameState.COMPUTERTURN)
        {
            additionalMiddle.transform.position = new Vector3((-1 * computerMiddleListSize) - 2, 0, -1 * (computerMiddleListSize)); // selected card goes to middle card position
            cMiddleCards.Add(additionalMiddle);
        }
    }


    public void RedrawCard()
    {
        // reset scores in a draw
        PlayerScoreKeeper.scoreValue = 0; 
        ComputerScoreKeeper.scoreValue = 0;

        // getting rid of all the middle cards
        for (int i = 0; i < pMiddleCards.Count; i++)
        {
            Destroy(pMiddleCards[i]);
        }

        for (int i = 0; i < cMiddleCards.Count; i++)
        {
            Destroy(cMiddleCards[i]);
        }

        // clearing up any references in the arraylist
        pMiddleCards.Clear();
        cMiddleCards.Clear();

        print("Player middle cards list now has " + pMiddleCards.Count + " cards.");
        print("Computer middle cards list now has " + cMiddleCards.Count + " cards.");

        state = GameState.DRAWCARD; // go back to draw state
    }
}
