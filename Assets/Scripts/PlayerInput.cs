using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public GameObject selectedCard;
    Higher higher;
    // Start is called before the first frame update
    void Start()
    {
        higher = GetComponent<Higher>();
        selectedCard = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        GetMouseClick();
    }

    void GetMouseClick()
    {
            if (Input.GetMouseButtonDown(0))
            {
                //Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10));
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

                // wait for shuffle to finish, then enter DRAWCARD state
                if (hit)
                {
                    //print("X coordinate of click is " + )
                    if (hit.collider.CompareTag("Deck") && Higher.state == GameState.DRAWCARD)
                    {
                        Deck();
                    }

                    else if (hit.collider.CompareTag("Card") && Higher.state == GameState.PLAYERTURN)
                    {
                        Card(hit.collider.gameObject); // BRO THIS IS HOW YOU GET THE OBJECT U CLICKED
                    }
                }
            }
    }

    void Deck()
    {
        // ONLY CLICKABLE DURING DRAWCARD STATE
        // if deck is clicked, flip the first card in player deck array over
        // bring it to the middle, then add its point value to player's score total
        print("Hit Deck");
        StartCoroutine(higher.DrawCard());
    }

    void Card(GameObject selected)
    {
        // ONLY CLICKABLE DURING PLAYER STATE
        print("Hit a Card");

        // first part is to ensure that selected card isn't null
        // second part is to ensure that the selected card isn't already in the middle
        if (selected.GetComponent<Selectable>().inMiddle == false)
        {
            print("Select card method entered.");
            selectedCard = selected;
            StartCoroutine(higher.SelectCard(selectedCard));
        }
    }
}
