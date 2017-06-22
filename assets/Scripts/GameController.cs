using UnityEngine;
using System;
using System.Collections;

namespace PokerGame
{

	public class GameController : MonoBehaviour
	{
		public Deck theDeck;

		public enum State
		{
			PreFlop,
PreTurn,
PreRiver,
PostRiver}		;

		public enum Decision
		{
			Fold,
			Check,
			Call,
			Raise}
		;
		public static float playerBalance;
		public static float computerBalance;

		public static State currentState;
		public static bool stateComplete;

		public float decisionValue = 0;
		public Decision finalResponse;

		//array of 2
		public static Card[] playerHoleCards;
		//array of 2
		public static Card[] computerHoleCards;
		//array of 3
		public static Card[] flopCards;

		public Card turnCard;
		public Card riverCard;

		public static float speed;
		//courage will be an indicator of how much, on average, the player is raising compared to how much he/she is folding
		//courage = (number of raises - number of folds)/hands played
		public float courage = 0;
		public static int numberOfRaises = 0;
		public static int numberOfFolds = 0;
		public static int handsPlayed = 1; //to not devide by 0

		public float computerRaiseAmount;
		public DecisionMaker dm;
		//the AI/computer that will play against the human player

		public static bool isAllIn;
		public static bool gameEnded = false;

		//private bool isFirstHand = true; //to indicate whether this is the first time we initialize

		public GameObject audioBox;

		//void Awake() {
		//	DontDestroyOnLoad (transform.gameObject);
		//}

		void Start ()
		{
			//if this is the first time the scene is loaded, set the player's balance to 300 and the computer balance to 1000
			if (PlayerPrefs.GetInt("firstTime") == 0){
				PlayerPrefs.SetFloat ("playerBalance", 300);
				PlayerPrefs.SetFloat ("computerBalance", 5000);
				PlayerPrefs.SetInt ("firstTime", 1);
			}
			playerBalance = PlayerPrefs.GetFloat ("playerBalance");
			computerBalance = PlayerPrefs.GetFloat("computerBalance");
			speed = 0.5f; // can vary the speed of the game from here TODO: insert it in settings
			stateComplete = false;
			playerHoleCards = new Card[2];
			computerHoleCards = new Card[2];
			flopCards = new Card[3];
			theDeck = GetComponentInChildren<Deck> ();
		
			//if(theDeck.cards[0] ==null){
			//	for (int i = 0; i < 1000; i++) {
			//		Debug.Log ("killing some time");
			//	}
			//}
			//assigning the player's cards, computer's cards, and the common 5 cards in between
			/*
			for (int i = 0; i < 2; i++) {
				playerHoleCards [i] = theDeck.cards [i];
				computerHoleCards [i] = theDeck.cards [i + 2];
				flopCards [i] = theDeck.cards [i + 4];
			}
			flopCards [2] = theDeck.cards [6];
			turnCard = theDeck.cards [7];
			riverCard = theDeck.cards [8];*/

			assignCards();
			//StartCoroutine(assignCards());

			dm = new DecisionMaker (computerHoleCards [0], computerHoleCards [1]); //giving the decision maker its 2 cards
			Debug.Log ("initial hand strength = " + dm.initialHandStrength ());
			if (computerHoleCards [0] != null && computerHoleCards [1] != null) {
				Debug.Log (computerHoleCards [0].Data.ToString ());
				Debug.Log (computerHoleCards [1].Data.ToString ());
			}
		}

		void Update ()
		{
			//first we wait for the user to make a decision, then when it's the computer's turn to act
			if (CanvasScript.playersTurn) {
				return;
			}
			Debug.Log ("now it's the computer's turn");

			courage = (numberOfRaises - numberOfFolds)*1.0f / (handsPlayed*1.0f);

			//when player'sTurn becomes false we check what decision maker (the AI class) wants to do, perform the action, and give the turn back to the player
			decideNextMove();
			performMove ();

			CanvasScript.playersTurn = true; //finally we return the turn back to the player
		}

		//this method will adjust finalResponse, without performing any actions
		public void decideNextMove() {
			//check to see if we need to move to the next state
			if (stateComplete) {
				currentState++;

				stateComplete = false;
			}

			switch (currentState) {
			case (State)0:
				//Debug.Log ("preflop state");
				decisionValue = dm.blindAction (dm.initialHandStrength (), courage);

				break;
			case (State)1:
				//Debug.Log ("preturn state");
				if (flopCards [0] == null || flopCards [1] == null || flopCards [2] == null) {
					decisionValue = 1;
					return;
				}
				//at the beginning of this state, we have to show the 3 flop cards
				flopCards [0].flipCard ();
				flopCards [1].flipCard ();
				flopCards [2].flipCard ();
				decisionValue = dm.preTurnAction (flopCards, courage);
				break;
			case (State)2:
				//Debug.Log ("preriver state");
				turnCard.flipCard ();
				decisionValue = dm.preRiverAction (flopCards, turnCard, courage);
				Debug.Log ("the preriver decision value is " + decisionValue);

				break;
			case (State)3:
				//Debug.Log ("postriver state");
				riverCard.flipCard ();
				decisionValue = dm.postRiverAction (flopCards, turnCard, riverCard, courage);
				Debug.Log ("the postriver decision value is " + decisionValue);
					//for (int i = 0; i < 52; i++) {
					//Instantiate(Deck); Destroy everything and build with another deck( if the player wants/can afford)
					//}
				//}
				break;
			case (State)4:
				//finally when the postriver betting stage ends we have to compare the hands, decide a winner, distribute the money and reset currentstate to 0;
				Hand playersHand = new Hand ();
				Hand computersHand = new Hand ();
				CardData[] playersPossibilities = new CardData [7];
				CardData[] computersPossibilities = new CardData [7];
				for (int i = 0; i < 5; i++) {
					if (i == 0 || i == 1) { 
						playersPossibilities [i] = playerHoleCards [i].Data;
						computersPossibilities [i] = computerHoleCards [i].Data;
					} else {
						playersPossibilities [i] = flopCards [i - 2].Data;
						computersPossibilities [i] = flopCards [i - 2].Data;
					}
				}
				playersPossibilities [5] = turnCard.Data;
				computersPossibilities [5] = turnCard.Data;	
				playersPossibilities [6] = riverCard.Data;
				computersPossibilities [6] = riverCard.Data;
				playersHand = Hand.getStrongestHand (playersPossibilities);
				computersHand = Hand.getStrongestHand (computersPossibilities);
				if (Hand.compare (playersHand, computersHand) == playersHand) {
					//player wins
					playerBalance += (CanvasScript.potBalance);
					UnityChanScript.anim.Play("WAIT03",-1,0f); 
					CanvasScript.potBalance = 0;
					//it is optional to show the computer's hands if he lost, but since this is a game for fun:
					computerHoleCards [0].flipCard ();
					computerHoleCards [1].flipCard ();
					UnityChanScript.anim.Play("WAIT03",-1,0f); 
					CanvasScript.decisionTextString = "Winning Hand: " + computersHand.ToString();

				} else if (Hand.compare (playersHand, computersHand) == computersHand) {
					//computer wins
					computerHoleCards [0].flipCard ();
					computerHoleCards [1].flipCard ();
					computerBalance += CanvasScript.potBalance;
					CanvasScript.potBalance = 0;
					audioBox.GetComponentInChildren<AudioBoxScript> ().playRandomAudio (AudioBoxScript.AudioType.Taunt);
				} else {
					//it's a tie, split the pot
					playerBalance += (CanvasScript.potBalance / 2);
					computerBalance += (CanvasScript.potBalance / 2);
					CanvasScript.potBalance = 0;
				}
				CanvasScript.decisionTextString = "Winning Hand: " + computersHand.ToString();
				Debug.Log ("player's hand is: " + playersHand.ToString ());
				Debug.Log ("computer's hand is: " + computersHand.ToString ());

				gameEnded = true;
				currentState = 0;
				stateComplete = false;
				break;
			}

			//a 5 percent chance to bluff
			if (decisionValue == 0) {
				if (UnityEngine.Random.Range ((int)0, (int)100) < 5) {
					decisionValue = UnityEngine.Random.Range (2.0f, 10.0f); //this lets the computer raise instead of folding to try and deceive the player
				}
			}

			//a 5 percent chance to not raise in order to hide a good card, or to raise when we should have folded/called
			if (UnityEngine.Random.Range ((int)0, (int)100) < 5) {
				decisionValue = UnityEngine.Random.Range (2.0f, 10.0f) - decisionValue;
				if (decisionValue < 2.0f) {
					decisionValue = 1.0f;
				}
			}

			//now that the computer has made a decision, we check, call, raise, or fold
			if (CanvasScript.raiseAmount != 0) {//if player HAS raised, computer can't check
				if (decisionValue == 0) {
					finalResponse = Decision.Fold;
				} else if (decisionValue == 1) {
					finalResponse = Decision.Call;
				} else {
					finalResponse = Decision.Raise;
				}
			} else { //if player didnt raise
				if (decisionValue == 0) {
					finalResponse = Decision.Check;
				} else if (decisionValue == 1) {
					finalResponse = Decision.Check;
				} else {
					finalResponse = Decision.Raise;
				}
			}
		}
		public void performMove() {
			if ((int)currentState >= 4 || gameEnded) {
				return; //the game would have ended already, no need for any decisions/actions
			}
			switch (finalResponse) {
			case(Decision.Check):
				//if we both checked then it is time to show the next card or compare hands if it is done
				stateComplete = true;
				decideNextMove ();
				//tell the player that the computer will check both in text and audio
				CanvasScript.decisionTextString = "I Check";
				audioBox.GetComponentInChildren<AudioBoxScript>().playRandomAudio (AudioBoxScript.AudioType.Check);
				break;
			case Decision.Call:
				//when calling the computer pays the amount the player asked for and then moves to the next state
				computerBalance -= CanvasScript.raiseAmount;
				CanvasScript.potBalance += CanvasScript.raiseAmount;
				CanvasScript.raiseAmount = 0;
				stateComplete = true; //to go to the next state
				CanvasScript.decisionTextString = "I Call";
				audioBox.GetComponentInChildren<AudioBoxScript>().playRandomAudio (AudioBoxScript.AudioType.Call);
				break;
			case Decision.Raise:
				//when the computer raises, the state isnt complete as it becomes the player's turns
				computerRaiseAmount = CanvasScript.raiseAmount + (CanvasScript.bigBlind * decisionValue); //i.e. calling what the player wants + raising with X times the big blind
				if (computerRaiseAmount > computerBalance || computerRaiseAmount > playerBalance) {
					//setting the maximum raise amount to the minimum of the two players' balances
					computerRaiseAmount = computerBalance > (playerBalance+CanvasScript.raiseAmount) ? (playerBalance+CanvasScript.raiseAmount) : computerBalance; //go all in
					isAllIn = true;
					audioBox.GetComponentInChildren<AudioBoxScript> ().playRandomAudio (AudioBoxScript.AudioType.AllIn);
				} else {
					audioBox.GetComponentInChildren<AudioBoxScript> ().playRandomAudio (AudioBoxScript.AudioType.Raise);
				}
				CanvasScript.amountDue = computerRaiseAmount - CanvasScript.raiseAmount; //now the player has to put up as much or he loses. Note that the player has to place the difference only, since he already put some of this money before
				computerBalance -= computerRaiseAmount;
				CanvasScript.potBalance += computerRaiseAmount;
				CanvasScript.raiseAmount = 0;
				CanvasScript.decisionTextString = "I raise to " + computerRaiseAmount.ToString("0.00"); 

				break;
			case Decision.Fold:
				//the round has ended, so we must reset everything
				playerBalance += CanvasScript.potBalance;
				CanvasScript.potBalance = 0;
				//the player wins
				UnityChanScript.anim.Play("WAIT03",-1,0f); 
				//because it's not competitive, show the computer's hands anyway:
				computerHoleCards [0].flipCard ();
				computerHoleCards [1].flipCard ();

				//TODO destroy all cards and distribute again
				CanvasScript.decisionTextString = "I Fold";
				audioBox.GetComponentInChildren<AudioBoxScript> ().playRandomAudio (AudioBoxScript.AudioType.Fold);

				if (theDeck.cards.Length > 0) {
					foreach (Card go in (theDeck.cards)) {
						//	Destroy (go);
					}
				}
				//Destroy (theDeck);
				GameObject deckPrefab = Resources.Load ("Prefab/Deck") as GameObject;

				gameEnded = true;
				//Deck d2 = Instantiate (deckPrefab).GetComponent<Deck> ();
				//theDeck = d2;
				//Start ();
				break;
			}
		}
		public void assignCards(){
			//if (!isFirstHand) {
				//yield return new WaitForSeconds (0.8f); //we need to wait for the deck to have its array of cards initialized first
			//}
			//isFirstHand = false;
			for (int i = 0; i < 2; i++) {
				playerHoleCards [i] = theDeck.cards [i];
				computerHoleCards [i] = theDeck.cards [i + 2];
				flopCards [i] = theDeck.cards [i + 4];
			}
			flopCards [2] = theDeck.cards [6];
			turnCard = theDeck.cards [7];
			riverCard = theDeck.cards [8];
			return;
		}

	}
}