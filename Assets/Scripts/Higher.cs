using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Packages.Rider.Editor.UnitTesting;

public enum GameState
{
    START, DRAWCARD, PASSINGCARDS, MOVECHECKER, PLAYERTURN, COMPUTERTURN, WON, LOST
}

public class Higher : MonoBehaviour
{
    public static GameState state;

    public ComputerInput computerInput;

    public Sprite[] cardFaces;
    public GameObject cardPrefab;

    public GameObject firePrefab;
    public GameObject waterPrefab;

    public UpdateSprite updateSprite;
    public SpriteRenderer prevSpriteRenderer;
    public CardFlipper cardFlipper;

    public UIButtons uiButtons;
    public MatchEndScreen matchEndScreen;

    public AudioManager audioManager;

    public ParticleSystem playerDeckIndicator;
    public ParticleSystem computerDeckIndicator;

    public SFX sFX;

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

    public Transform[] pFieldTransforms;
    public Transform[] cFieldTransforms;

    public Transform playerStartCardPos, playerEndCardPos, computerStartCardPos, computerEndCardPos;
    public float cardPassSpeed;

    // Start is called before the first frame update
    void Start()
    {
        state = GameState.START;
        MatchEndScreen.lossReason = LossReason.NA;
        pFieldTransforms = new Transform[10];
        cFieldTransforms = new Transform[10];
        playerDeckIndicator = GetComponent<ParticleSystem>();
        computerDeckIndicator = GameObject.Find("ComputerDeckIndicator").GetComponent<ParticleSystem>();
        prevSpriteRenderer = GetComponent<SpriteRenderer>();
        computerInput = GetComponent<ComputerInput>();
        cardFlipper = GetComponent<CardFlipper>();
        audioManager = GetComponent<AudioManager>();
        sFX = FindObjectOfType<SFX>();
        uiButtons = GetComponent<UIButtons>();
        matchEndScreen = GetComponent<MatchEndScreen>();
        FindObjectOfType<AudioManager>().Play("Music");
        Play();
    }

    // Update is called once per frame
    void Update()
    {
        //if (state == GameState.DRAWCARD)
        //{
        //    playerDeckIndicator.Emit(1);
        //} 
        //else
        //{
        //    playerDeckIndicator.Stop();
        //}
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
        startDeckIndicators(true);
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
        GameObject pDeckButton = GameObject.Find("PlayerDeckButton"); // used to get the transform of deck location
        GameObject cDeckButton = GameObject.Find("ComputerDeckButton");

        for (int i = 0; i < pFieldTransforms.Length; i++)
        {
            String pTransName = "PlayerHand" + i;
            String cTransname = "ComputerHand" + i;
            pFieldTransforms[pFieldTransforms.Length - i + -1] = GameObject.Find(pTransName).transform;
            cFieldTransforms[pFieldTransforms.Length - i + -1] = GameObject.Find(cTransname).transform;
        }

        // for fields
        for (int i = 0; i < 10; i++) 
        {
            
            GameObject pNewCard;
            GameObject cNewCard;

            // vector position of player deck
            float pDeckButtonX = pDeckButton.transform.position.x;
            float pDeckButtonY = pDeckButton.transform.position.y;
            float pDeckButtonZ = pDeckButton.transform.position.z;

            // vector position of computer deck
            float cDeckButtonX = cDeckButton.transform.position.x;
            float cDeckButtonY = cDeckButton.transform.position.y;
            float cDeckButtonZ = cDeckButton.transform.position.z;

            // vector position of all 10 player field cards
            float pFieldCardX = pFieldTransforms[i].position.x;
            float pFieldCardY = pFieldTransforms[i].position.y;
            float pFieldCardZ = pFieldTransforms[i].position.z - i;

            // vector position of all 10 computer field cards
            float cFieldCardX = cFieldTransforms[i].position.x;
            float cFieldCardY = cFieldTransforms[i].position.y;
            float cFieldCardZ = cFieldTransforms[i].position.z - i;

            // speed of pass
            cardPassSpeed = 2f;

            //FindObjectOfType<AudioManager>().Play("Dealing");

            // player deck start pos is -9, -4 -- computer deck start pos is 9, 4
            // instantiate pNewCards at -9, 4
            // then have the first one travel to pos -6, -4... and so forth til the end of the row
            pNewCard = Instantiate(cardPrefab, new Vector3(pDeckButtonX, pDeckButtonY, pDeckButtonZ - 3), Quaternion.identity);
            cNewCard = Instantiate(cardPrefab, new Vector3(cDeckButtonX, cDeckButtonY, cDeckButtonZ - 3), new Quaternion(0, 0, 180, 0));

            iTween.MoveTo(pNewCard, new Vector3(pFieldCardX, pFieldCardY, pFieldCardZ), 0.75f);
            iTween.MoveTo(cNewCard, new Vector3(cFieldCardX, cFieldCardY, cFieldCardZ), 0.75f);

            pNewCard.name = pField[i];
            cNewCard.name = cField[i];

            pFieldCards.Add(pNewCard);

            cFieldCards.Add(cNewCard);

            pNewCard.GetComponent<Selectable>().faceUp = true;
            cNewCard.GetComponent<Selectable>().faceUp = false;

            yield return new WaitForSeconds(0.1f); // spawns cards one-by-one visually
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

            callDrawMotion(pMiddleCards[0]);

            yield return new WaitForSeconds(1.5f); // a delay between both draws to allow the player to understand what just happened

            cMiddleCards.Add(cPhysicalDeck[0]);
            cPhysicalDeck.RemoveAt(0);
            int cDrawnCardValue = cMiddleCards[0].GetComponent<Selectable>().value;
            ComputerScoreKeeper.scoreValue += cDrawnCardValue;
            print(cMiddleCards[0].name + " was drawn by the computer.");

            
            callDrawMotion(cMiddleCards[0]);

            yield return new WaitForSeconds(2.5f); // a delay between both draws to allow the player to understand what just happened
            
            whoGoesFirst();
        }
    }

    public void callDrawMotion(GameObject card)
    {
        StopCoroutine(drawMotion(card));
        StartCoroutine(drawMotion(card));
    }
    IEnumerator drawMotion(GameObject card)
    {
        iTween.MoveTo(card, new Vector3(card.transform.position.x, card.transform.position.y, -2), 0.01f);
        card.GetComponent<Selectable>().faceUp = true;
        if (card.tag == "Card")
        {
            FindObjectOfType<AudioManager>().Play("FlipCard");
            iTween.RotateFrom(card, new Vector3(0, 180, 0), 2.5f);
            yield return new WaitForSeconds(1f);
            FindObjectOfType<AudioManager>().Play("SelectCard");
            iTween.MoveTo(card, new Vector3(2, 0, 0), 0.75f);
        } 
        else
        {
            FindObjectOfType<AudioManager>().Play("FlipCard");
            iTween.RotateFrom(card, new Vector3(180, 0, 0), 2.5f);
            yield return new WaitForSeconds(1f);
            FindObjectOfType<AudioManager>().Play("SelectCard");
            iTween.MoveTo(card, new Vector3(-2, 0, 0), 0.75f);
        }
    }

    public void whoGoesFirst()
    {
        if (state == GameState.MOVECHECKER)
        {
            startDeckIndicators(false);
            if (PlayerScoreKeeper.scoreValue > ComputerScoreKeeper.scoreValue)
            {
                // if the Computer draws the last card in his hand and goes to redrawing phase
                // AND also draws a lower card in the redrawing phase, they lose
                if (cFieldCards.Count == 0)
                {
                    print("Computer scored lower and has no cards left in play. Game over.");
                    state = GameState.WON;
                    callMatchEnd();
                } 
                else
                {
                    print("Player scored higher, therefore computer goes first.");
                    state = GameState.COMPUTERTURN;
                    print("It is now computer turn.");
                    computerInput.chooseCard();
                }
            } 
            else if (PlayerScoreKeeper.scoreValue < ComputerScoreKeeper.scoreValue)
            {
                // if the player draws the last card in his hand and goes to redrawing phase
                // AND also draws a lower card in the redrawing phase, they lose
                if (pFieldCards.Count == 0)
                {
                    print("Player scored lower and has no cards left in play. Game over.");
                    MatchEndScreen.lossReason = LossReason.NOCARDS;
                    state = GameState.LOST;
                    callMatchEnd();
                }
                else
                {
                    print("Computer scored higher, therefore player goes first.");
                    state = GameState.PLAYERTURN;
                }
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

    // pre-select card method that checks to see if there are any cards left
    // e.g. before the player's turn to select a card, if the player has no cards left
    // the player loses
    public void checkSelectableCards()
    {
        print("Checking selectable cards in hands.");
        if (state == GameState.MOVECHECKER)
        {
            print("Checking selectable cards in computer's hand.");
            if (cFieldCards.Count == 0)
            {
                state = GameState.WON;
                callMatchEnd();
            }
            else
            {
                state = GameState.COMPUTERTURN;
                computerInput.chooseCard();
            }

        }
        else if (state == GameState.COMPUTERTURN)
        {
            print("Again there are " + pFieldCards.Count + " cards left in the player's hand");
            if (pFieldCards.Count == 0)
            {
                print("No cards left in the player's hand.");
                state = GameState.LOST;
                MatchEndScreen.lossReason = LossReason.NOCARDS;
                callMatchEnd();
            }
            else {
                state = GameState.PLAYERTURN;
            }
        }
    }

    public void SelectCard(GameObject selected)
    {
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

                UseWormholeCard(selected);
            }
            else if (selected.GetComponent<Selectable>().value == 1 && selected.GetComponent<Selectable>().type == "P" 
                && pMiddleCards[pMiddleCards.Count - 1].GetComponent<Selectable>().faceUp == false)
            {
                // revive the burnt card
                // destroy pluto card
                // regain the lost points
                UseReviveCard(selected);
            }
            else
            {
                UsePlanetCard(selected);
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
                UseSunCard(selected);
            }
            // if the computer selects "Wormhole" card
            // flip the computer and player's score
            else if (selected.GetComponent<Selectable>().type == "W")
            {
                print("Computer has switched their score with the player's using a Wormhole card.");

                UseWormholeCard(selected);
            }
            else if (selected.GetComponent<Selectable>().value == 1 && selected.GetComponent<Selectable>().type == "P"
                && cMiddleCards[cMiddleCards.Count - 1].GetComponent<Selectable>().faceUp == false)
            {
                // revive the burnt card
                // destroy pluto card
                // regain the lost points
                UseReviveCard(selected);
            }
            else
            {
                UsePlanetCard(selected);
            }
        }
    }

    // method to cast a Sun card on the opponent
    public void UseSunCard(GameObject sunCard)
    {
        StopCoroutine(SunCard(sunCard));
        StartCoroutine(SunCard(sunCard));
    }

    // method to cast a Wormhole card on the opponent
    public void UseWormholeCard(GameObject wormHoleCard)
    {
        StopCoroutine(WormholeCard(wormHoleCard));
        StartCoroutine(WormholeCard(wormHoleCard));
    }

    // method to select a Planet card
    public void UsePlanetCard(GameObject planetCard)
    {
        StopCoroutine(PlanetCard(planetCard));
        StartCoroutine(PlanetCard(planetCard));
    }

    // method to select a Pluto Revive card
    public void UseReviveCard(GameObject reviveCard)
    {
        StopCoroutine(ReviveCard(reviveCard));
        StartCoroutine(ReviveCard(reviveCard));
    }

    public IEnumerator SunCard(GameObject sunCard)
    {
       
        if (state == GameState.MOVECHECKER)
        {
            removePFieldCard(sunCard);
            // responsible for movement of Sun card towards the target card
            float burnedCardX = cMiddleCards[cMiddleCards.Count - 1].transform.position.x;
            float burnedCardY = cMiddleCards[cMiddleCards.Count - 1].transform.position.y;
            float burnedCardZ = cMiddleCards[cMiddleCards.Count - 1].transform.position.z;
            iTween.MoveTo(sunCard, new Vector3(burnedCardX, burnedCardY, burnedCardZ), 0.75f);
            yield return new WaitForSeconds(0.50f);
            Destroy(sunCard);
            //yield return new WaitForSeconds(0.50f);
            // if use a sun card on facedown card
            // create a super nova and destroy all cards in the middle
            // redraw cards
            if (cMiddleCards[cMiddleCards.Count - 1].GetComponent<Selectable>().faceUp == false)
            {
                FindObjectOfType<AudioManager>().Play("Shielded");

                print("hit a facedown card with a Suncard, supernova destroys everything. redrawing.");
                Destroy(sunCard);
                PlayerScoreKeeper.scoreValue = 0;
                ComputerScoreKeeper.scoreValue = 0;
                RedrawCard();
            } 
            else
            {
                //computerFire.GetComponent<ParticleSystem>().Play();
                //iTween.MoveTo(computerFire, new Vector3(burnedCardX, burnedCardY, 0), 0.01f);
                //computerFire.transform.parent = cMiddleCards[cMiddleCards.Count - 1].transform;
                FindObjectOfType<AudioManager>().Play("Burn");
                GameObject newComputerSideFire = Instantiate(firePrefab, new Vector3(burnedCardX, burnedCardY - 1, -5), firePrefab.transform.rotation);
                yield return new WaitForSeconds(0.75f);
                newComputerSideFire.GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmitting);
                sFX.dissolveCard(cMiddleCards[cMiddleCards.Count - 1]);
                cardFlipper.StartFlip(cMiddleCards[cMiddleCards.Count - 1]);
                yield return new WaitForSeconds(0.5f);
                Math.Max(ComputerScoreKeeper.scoreValue -= cMiddleCards[cMiddleCards.Count - 1].GetComponent<Selectable>().value, 0);
                
            }
             // special cards are used up, not stored in middle
            // if the computer plays a special card as their last card, they lose
            if (pFieldCards.Count == 0)
            {
                print("Player has committed a foul. Game lost.");
                MatchEndScreen.lossReason = LossReason.FOUL;
                state = GameState.LOST;
                callMatchEnd();
            }
            else
            {
                yield return new WaitForSeconds(1f);
                checkSelectableCards();
            }
        } 
        else if (state == GameState.COMPUTERTURN)
        {
            
            removeCFieldCard(sunCard);
            // responsible for movement of Sun card towards the target card
            float burnedCardX = pMiddleCards[pMiddleCards.Count - 1].transform.position.x;
            float burnedCardY = pMiddleCards[pMiddleCards.Count - 1].transform.position.y;
            float burnedCardZ = pMiddleCards[pMiddleCards.Count - 1].transform.position.z;
            iTween.MoveTo(sunCard, new Vector3(burnedCardX, burnedCardY, burnedCardZ), 0.75f);
            yield return new WaitForSeconds(0.50f);
            Destroy(sunCard); // special cards are used up, not stored in middle
            

            if (pMiddleCards[pMiddleCards.Count - 1].GetComponent<Selectable>().faceUp == false)
            {
                FindObjectOfType<AudioManager>().Play("Shielded");
                print("hit a facedown card with a Suncard, supernova destroys everything. redrawing.");
                Destroy(sunCard);
                PlayerScoreKeeper.scoreValue = 0;
                ComputerScoreKeeper.scoreValue = 0;
                RedrawCard();
            }
            else
            {
                //playerFire.GetComponent<ParticleSystem>().Play();
                //iTween.MoveTo(playerFire, new Vector3(burnedCardX, burnedCardY, 0), 0.01f);
                //playerFire.transform.parent = pMiddleCards[pMiddleCards.Count - 1].transform;
                FindObjectOfType<AudioManager>().Play("Burn");
                GameObject newPlayerSideFire = Instantiate(firePrefab, new Vector3(burnedCardX, burnedCardY - 1, -5), firePrefab.transform.rotation);
                yield return new WaitForSeconds(0.75f);
                newPlayerSideFire.GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmitting);
                sFX.dissolveCard(pMiddleCards[pMiddleCards.Count - 1]);
                cardFlipper.StartFlip(pMiddleCards[pMiddleCards.Count - 1]);
                yield return new WaitForSeconds(0.5f);
                Math.Max(PlayerScoreKeeper.scoreValue -= pMiddleCards[pMiddleCards.Count - 1].GetComponent<Selectable>().value, 0);
            }

            // if the computer plays a special card as their last card, they lose
            if (cFieldCards.Count == 0)
            {
                print("Computer has committed a foul. Game won.");
                state = GameState.WON;
                callMatchEnd();
            }
            else
            {
                yield return new WaitForSeconds(1f);
                checkSelectableCards();
            }
        }
    }

    public IEnumerator WormholeCard(GameObject wormHoleCard)
    {
        // responsible for movement of wormhole card towards the center
        iTween.MoveTo(wormHoleCard, new Vector3(0, 0, -10), 0.75f);

        FindObjectOfType<AudioManager>().Play("Wormhole");
        // switch the card positions
        for (int i = 0; i < pMiddleCards.Count; i++)
        {
            iTween.RotateTo(pMiddleCards[i], new Vector3(0, 0, 180), 1.5f);
            //yield return new WaitForSeconds(1f);
            iTween.MoveTo(pMiddleCards[i], new Vector3(-1 * (i + 2), 0, -1 * i), 0.75f);
            //pMiddleCards[i].transform.position = new Vector3(-1 * (i + 2), 0, -1 * i); // selected card goes to middle card position.
            //pMiddleCards[i].transform.rotation = new Quaternion(0, 0, 180, 0);
        }

        for (int i = 0; i < cMiddleCards.Count; i++)
        {
            iTween.RotateTo(cMiddleCards[i], new Vector3(0, 0, 0), 1.5f);
            //yield return new WaitForSeconds(1f);
            iTween.MoveTo(cMiddleCards[i], new Vector3(i + 2, 0, -1 * i), 0.75f);
        }

        // switch the list pointers
        List<GameObject> tempList = pMiddleCards;
        pMiddleCards = cMiddleCards;
        cMiddleCards = tempList;

        // switches their scores
        int temp = PlayerScoreKeeper.scoreValue;
        PlayerScoreKeeper.scoreValue = ComputerScoreKeeper.scoreValue;
        ComputerScoreKeeper.scoreValue = temp;

        yield return new WaitForSeconds(0.5f);

        if (state == GameState.MOVECHECKER)
        {
            removePFieldCard(wormHoleCard);
            Destroy(wormHoleCard);

            // if the player plays a special card as their last card, they lose
            if (pFieldCards.Count == 0)
            {
                print("Player has committed a foul. Game over.");
                state = GameState.LOST;
                MatchEndScreen.lossReason = LossReason.FOUL;
                callMatchEnd();
            }
            else
            {
                yield return new WaitForSeconds(1f);
                checkSelectableCards();
            }
        } 
        else if (state == GameState.COMPUTERTURN)
        {
            removeCFieldCard(wormHoleCard);
            Destroy(wormHoleCard);

            // if the player plays a special card as their last card, they lose
            if (cFieldCards.Count == 0)
            {
                print("Computer has committed a foul. Game won.");
                state = GameState.WON;
                callMatchEnd();
            }
            else
            {
                checkSelectableCards();
                state = GameState.PLAYERTURN;
                yield return new WaitForSeconds(1f);
            }
        }
    }

    public IEnumerator PlanetCard(GameObject planetCard)
    {
        if (state == GameState.MOVECHECKER)
        {
            // check if the previous card in hand was burned by a Sun card
            // if so, put out the fire indicating that card is now unrecoverable
            //if (pMiddleCards[pMiddleCards.Count - 1].GetComponent<Selectable>().faceUp == false)
            //{
            //    playerFire.GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            //}

            if ((PlayerScoreKeeper.scoreValue + planetCard.GetComponent<Selectable>().value) > ComputerScoreKeeper.scoreValue)
            {
                print("Player has played a card that allows his score to surpass the computer's.");
                PlayerScoreKeeper.scoreValue += planetCard.GetComponent<Selectable>().value;
                AddMiddle(planetCard);
                removePFieldCard(planetCard);
                yield return new WaitForSeconds(1f);
                checkSelectableCards();
              
            }
            else if ((PlayerScoreKeeper.scoreValue + planetCard.GetComponent<Selectable>().value) < ComputerScoreKeeper.scoreValue)
            {
                print("Player has played a card with insufficient value. Game over.");
                PlayerScoreKeeper.scoreValue += planetCard.GetComponent<Selectable>().value;
                AddMiddle(planetCard);
                removePFieldCard(planetCard);
                state = GameState.LOST;
                MatchEndScreen.lossReason = LossReason.INSUFFICIENTVALUE;
                callMatchEnd();
            }
            else // player has picked a card that equalizes their total with the computer's total score
            {
                print("Player has played a card that equalizes their total with the computer's total score. Redrawing.");
                PlayerScoreKeeper.scoreValue += planetCard.GetComponent<Selectable>().value;
                AddMiddle(planetCard);
                removePFieldCard(planetCard);
                yield return new WaitForSeconds(1f); // wait for player to catch up on what's happening
                RedrawCard();
            }
        }
        else if (state == GameState.COMPUTERTURN)
        {
            // check if the previous card in hand was burned by a Sun card
            // if so, put out the fire indicating that card is now unrecoverable
            //if (cMiddleCards[cMiddleCards.Count - 1].GetComponent<Selectable>().faceUp == false)
            //{
            //    print(cMiddleCards[cMiddleCards.Count - 1].GetComponent<Selectable>().faceUp);
            //    computerFire.GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            //}

            if ((ComputerScoreKeeper.scoreValue + planetCard.GetComponent<Selectable>().value) > PlayerScoreKeeper.scoreValue)
            {
                print("Computer has played a card that allows his score to surpass the player's.");
                ComputerScoreKeeper.scoreValue += planetCard.GetComponent<Selectable>().value;
                AddMiddle(planetCard);
                removeCFieldCard(planetCard);
                yield return new WaitForSeconds(1f);
                checkSelectableCards();             
            }
            else if ((ComputerScoreKeeper.scoreValue + planetCard.GetComponent<Selectable>().value) < PlayerScoreKeeper.scoreValue)
            {
                print("Computer has played a card with insufficient value. Game won.");
                ComputerScoreKeeper.scoreValue += planetCard.GetComponent<Selectable>().value;
                AddMiddle(planetCard);
                removeCFieldCard(planetCard);
                state = GameState.WON;
                callMatchEnd();
            }
            else // computer has picked a card that equalizes their total with the player's total score
            {
                print("Computer has played a card that equalizes their total with the player's total score. Redrawing.");
                ComputerScoreKeeper.scoreValue += planetCard.GetComponent<Selectable>().value;
                AddMiddle(planetCard);
                removeCFieldCard(planetCard);
                yield return new WaitForSeconds(1f); // wait for player to catch up on what's happening
                RedrawCard();
            }
        }
            
    }

    public IEnumerator ReviveCard(GameObject reviveCard)
    {
        FindObjectOfType<AudioManager>().Play("Revive");
        if (state == GameState.MOVECHECKER)
        {
            // responsible for movement of Revive card towards the burned card
            float burnedCardX = pMiddleCards[pMiddleCards.Count - 1].transform.position.x;
            float burnedCardY = pMiddleCards[pMiddleCards.Count - 1].transform.position.y;
            float burnedCardZ = pMiddleCards[pMiddleCards.Count - 1].transform.position.z;
            iTween.MoveTo(reviveCard, new Vector3(burnedCardX, burnedCardY, burnedCardZ), 0.75f);

            GameObject newPlayerSideWater = Instantiate(waterPrefab, new Vector3(burnedCardX, burnedCardY - 1, -5), waterPrefab.transform.rotation);
            yield return new WaitForSeconds(0.75f);
            newPlayerSideWater.GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmitting);

            sFX.restoreCard(pMiddleCards[pMiddleCards.Count - 1]);
            cardFlipper.StartFlip(pMiddleCards[pMiddleCards.Count - 1]);
            yield return new WaitForSeconds(0.5f);
            PlayerScoreKeeper.scoreValue += pMiddleCards[pMiddleCards.Count - 1].GetComponent<Selectable>().value;
            removePFieldCard(reviveCard);
            Destroy(reviveCard);
            yield return new WaitForSeconds(1f); // wait for player to catch up on what's happening
            checkSelectableCards();
        }
        else if (state == GameState.COMPUTERTURN)
        {
            // responsible for movement of Revive card towards the burned card
            float burnedCardX = cMiddleCards[cMiddleCards.Count - 1].transform.position.x;
            float burnedCardY = cMiddleCards[cMiddleCards.Count - 1].transform.position.y;
            float burnedCardZ = cMiddleCards[cMiddleCards.Count - 1].transform.position.z;
            iTween.MoveTo(reviveCard, new Vector3(burnedCardX, burnedCardY, burnedCardZ), 0.75f);

            GameObject newComputerSideWater = Instantiate(waterPrefab, new Vector3(burnedCardX, burnedCardY - 1, -5), waterPrefab.transform.rotation);
            yield return new WaitForSeconds(0.75f);
            newComputerSideWater.GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmitting);

            sFX.restoreCard(cMiddleCards[cMiddleCards.Count - 1]);
            cardFlipper.StartFlip(cMiddleCards[cMiddleCards.Count - 1]);
            yield return new WaitForSeconds(0.5f);
            ComputerScoreKeeper.scoreValue += cMiddleCards[cMiddleCards.Count - 1].GetComponent<Selectable>().value;
            removeCFieldCard(reviveCard);
            Destroy(reviveCard);
            checkSelectableCards();
            state = GameState.PLAYERTURN;
            yield return new WaitForSeconds(1f); // wait for player to catch up on what's happening
        }
    }

    // method to remove cards from the player's hand as they are played
    public void removePFieldCard(GameObject card)
    {
        pFieldCards.Remove(card);
        print("Size of player's field cards is now " + pFieldCards.Count);
    }

    // method to remove cards from the computer's hand as they are played
    public void removeCFieldCard(GameObject card)
    {
        cFieldCards.Remove(card);
        print("Size of computer's field cards is now " + cFieldCards.Count);
    }

    // method controlling scene switch based on match end conditions
    public void callMatchEnd()
    {
        StopCoroutine(matchEnd());
        StartCoroutine(matchEnd());
    }

    public IEnumerator matchEnd()
    {
        print(state);
        print("Calculating win/loss state...");
        yield return new WaitForSeconds(1f);

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

        //float xOffset = 0.5f;

        if (state == GameState.MOVECHECKER)
        {
            // selected card goes to middle card position.
            iTween.MoveTo(additionalMiddle, new Vector3((playerMiddleListSize) + 2, 0, -1 * (playerMiddleListSize)), 0.75f);
            pMiddleCards.Add(additionalMiddle);
        }
        else if (state == GameState.COMPUTERTURN)
        {
            // selected card goes to middle card position
            iTween.MoveTo(additionalMiddle, new Vector3((-1 * computerMiddleListSize) - 2, 0, -1 * (computerMiddleListSize)), 0.75f); 
            cMiddleCards.Add(additionalMiddle);
        }
    }


    public void RedrawCard()
    {
        StopCoroutine(DoRedrawCard());
        StartCoroutine(DoRedrawCard());
    }

    public IEnumerator DoRedrawCard()
    {
        // reset scores in a draw
        PlayerScoreKeeper.scoreValue = 0;
        ComputerScoreKeeper.scoreValue = 0;

        
        FindObjectOfType<AudioManager>().Play("Draw");

        // getting rid of all the middle cards
        for (int i = 0; i < pMiddleCards.Count; i++)
        {
            iTween.MoveTo(pMiddleCards[i], new Vector3(12, 0, 0), 0.75f);
            yield return new WaitForSeconds(0.2f);
            Destroy(pMiddleCards[i]);
        }

        for (int i = 0; i < cMiddleCards.Count; i++)
        {
            iTween.MoveTo(cMiddleCards[i], new Vector3(-12, 0, 0), 0.75f);
            yield return new WaitForSeconds(0.2f);
            Destroy(cMiddleCards[i]);
        }

        // clearing up any references in the arraylist
        pMiddleCards.Clear();
        cMiddleCards.Clear();

        print("Player middle cards list now has " + pMiddleCards.Count + " cards.");
        print("Computer middle cards list now has " + cMiddleCards.Count + " cards.");

        startDeckIndicators(true); // indicate draw state on deck with particle system emission
        state = GameState.DRAWCARD; // go back to draw state
    }

    public void startDeckIndicators(Boolean b)
    {
        if (b == true)
        {
            computerDeckIndicator.Play();
            playerDeckIndicator.Play();
            
        }
        else
        {
            playerDeckIndicator.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear); // stop emitting indicator outside of draw card state
            computerDeckIndicator.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}
