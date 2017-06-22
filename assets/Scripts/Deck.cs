using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerGame
{
    public class Deck : MonoBehaviour
    {
        //the array that will hold the cards of the deck
        public Card[] cards;

		//these will be used in dealing the cards (at the beginning of each round/game)
		public Transform card1PositionGO;
		public Transform card2PositionGO;
		public Transform card3PositionGO;
		public Transform card4PositionGO;
		public Transform communityCardPositionGO;
		private float startTime;
		private float[] journeyLength;
		bool sendSecondCard = false;

		public GameObject audioBox;
		private bool animationDone = false;

		void Awake(){
			GameObject cardPrefab = Resources.Load ("Prefab/Card") as GameObject;
			//create a deck
			cards = new Card[52];
			for (int i = 0; i < 52; i++)
			{
				cards [i] = Instantiate(cardPrefab).GetComponent<Card> ();
				cards [i].Initialize (i);
				cards[i].gameObject.transform.Rotate(new Vector3(1,0,0), 270);
				cards[i].gameObject.transform.Translate(new Vector3(7,-5,-(float)(i*0.005))); //stacking the cards on top of each other

			}
			//randomize the locations of the cards by swapping them together pseudorandomly
			Shuffle();

			//to distribute cards using Lerp, we need these values
			startTime = Time.time;
			journeyLength = new float[9];
			//2 player cards
			journeyLength [0] = Vector3.Distance (cards [0].transform.position, card1PositionGO.position);
			journeyLength [1] = Vector3.Distance (cards [1].transform.position, card2PositionGO.position);
			cards [0].transform.rotation = card1PositionGO.rotation;
			cards [1].transform.rotation = card2PositionGO.rotation;
			//minor tweeking so the card doesnt appear to pass through the deck
			cards[1].transform.position -= new Vector3(0f,0f,1f);
			//hiding the second card till it is its turn
			MeshRenderer renderer = cards [1].GetComponentInChildren<MeshRenderer> ();
			renderer.enabled = false;

			//2 computer cards
			journeyLength [2] = Vector3.Distance (cards [2].transform.position, card3PositionGO.position);
			journeyLength [3] = Vector3.Distance (cards [3].transform.position, card4PositionGO.position);
			cards [2].transform.rotation = card3PositionGO.rotation;
			cards [3].transform.rotation = card4PositionGO.rotation;
			//hiding the second card till it is its turn
			MeshRenderer renderer2 = cards [3].GetComponentsInChildren<MeshRenderer> ()[1]; //the [1] was inserted because we want to hide the back of the card (so we got componentS)
			renderer2.enabled = false;

			//5 community cards
			for (int i = 0; i < 5; i++) {
				journeyLength [4 + i] = Vector3.Distance (cards [4 + i].transform.position, communityCardPositionGO.position + new Vector3 (2 * i, 0, 0));
				cards[4 + i].transform.rotation = communityCardPositionGO.rotation; 
				MeshRenderer rendererCommunity = cards [4+i].GetComponentsInChildren<MeshRenderer> ()[1]; //here also we hide the backs
				rendererCommunity.enabled = false;
			}


		}
        void Start()
		{
			
        }

		void Update () {
			if (cards [0] == null) {
				return;
			}
			dealPlayerCards();
			dealComputerCards ();


			//now, when the computer's cards reach their destination, deal the flop, turn, and river cards face down
			if (cards [3].transform.position == card4PositionGO.position) {
				dealCommunityCards ();
				}
		}


        public void Shuffle()
        {
            //We start from the last card. Then we randomly choose a card to swap it with
            //Next, we move to the 51st card and swap it with another randomly chosen card smaller than it.
            //And so on until we reach the first card in the deck
            int n = 52;//our cursor (starting from last number and going back to first)
            while (n > 1)
            {
                n--;
				int randIndex = UnityEngine.Random.Range (0, n); //is the index of the card that we want to swap with
                //swapping 2 cards
                Card temp = cards[n];
                cards[n] = cards[randIndex];
                cards[randIndex] = temp;
            }
        }
		public void dealPlayerCards(){
			float distCovered = (Time.time - startTime) * GameController.speed;
			float fracJourney1 = distCovered / journeyLength[0];
			float fracJourney2 = distCovered / journeyLength[1];
			if (cards [0] == null) {
				return;
			}
				cards [0].transform.position = Vector3.Lerp (cards [0].transform.position, card1PositionGO.position, fracJourney1);
				if (cards [0].transform.position == card1PositionGO.transform.position && sendSecondCard == false) {
					startTime = Time.time;
					sendSecondCard = true;
				}

			if (sendSecondCard) {
				if (cards [1] == null) {
					return;
				}
				MeshRenderer renderer = cards [1].GetComponentInChildren<MeshRenderer> ();
				renderer.enabled = true;
				cards [1].transform.position = Vector3.Lerp (cards [1].transform.position, card2PositionGO.position, fracJourney2);
			}
			if (cards [1].transform.position == card2PositionGO.position) {
				if (!animationDone) {
					if (cards [0].Data.value == cards [1].Data.value) {
						//a pair!     
						audioBox.GetComponentInChildren<AudioBoxScript>().playPairAudio();
						UnityChanScript.anim.Play("WAIT04",-1,0f); 

						animationDone = true;
					} else if (cards [0].Data.value >= 10 && cards [1].Data.value >= 10) {
						//interesting,  both cards are high
						audioBox.GetComponentInChildren<AudioBoxScript>().playHighHoleAudio();
						UnityChanScript.anim.Play("WAIT02",-1,0f); 
						animationDone = true;
					} else if (cards [0].Data.value < 8 && cards [1].Data.value < 8) 
					{
						//again with these louzy cards!  damage01 
						audioBox.GetComponentInChildren<AudioBoxScript>().playBadCards();
						//audioBox.GetComponentInChildren<AudioBoxScript>().playBadCardsAudio();
						UnityChanScript.anim.Play("DAMAGED01",-1,0f); 
						animationDone = true;
					}

				}
			
			}
		}

		public void dealComputerCards(){
			//when the player's cards arrive, send over the computer's cards
			float distCovered = (Time.time - startTime) * GameController.speed;
			float fracJourney1 = distCovered / journeyLength[2];
			float fracJourney2 = distCovered / journeyLength[3];

			if (cards [2] == null) {
				return;
			}

			cards [2].transform.position = Vector3.Lerp (cards [2].transform.position, card3PositionGO.position, fracJourney1);
			if (cards [2].transform.position == card3PositionGO.transform.position && sendSecondCard ==false) {
				startTime = Time.time;
				sendSecondCard = true;
			}
			if (sendSecondCard) {
				MeshRenderer renderer = cards [3].GetComponentsInChildren<MeshRenderer> ()[1];
				renderer.enabled = true;
				cards [3].transform.position = Vector3.Lerp (cards [3].transform.position, card4PositionGO.position, fracJourney2);
			}
		}

		public void dealCommunityCards(){
			float distCovered = (Time.time - startTime) * GameController.speed;
			float fracJourney = distCovered / journeyLength[4];
			for (int i = 0; i < 5; i++) {
				MeshRenderer renderer = cards [i+4].GetComponentsInChildren<MeshRenderer> ()[1];
				renderer.enabled = true;
				cards [4+i].transform.position = Vector3.Lerp (cards [4+i].transform.position, communityCardPositionGO.position + new Vector3 (2 * i, 0, 0), fracJourney);
			}
	}
		public static void MyDelay( int seconds )
		{
			DateTime dt = DateTime.Now;
			TimeSpan ts =  dt.TimeOfDay + TimeSpan.FromSeconds( seconds );
		}
}

}




