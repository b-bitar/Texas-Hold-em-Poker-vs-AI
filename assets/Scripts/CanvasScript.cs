using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace PokerGame
{
	public class CanvasScript : MonoBehaviour
	{
		
		public static float potBalance;
		public static float raiseAmount = 0;  //the player's raise amount
		public static float bigBlind = 10 + (5* ((int)GameController.handsPlayed/10)); //every 10 games increase the blind by 5

		public Slider raiseSlider;
		public Text displayRaiseAmount;
		public Text playerBalanceText;
		public Text computerBalanceText;
		public Text potBalanceText;
		public Text callCostText;
		public Text bigBlindText;

		public Text decisionText;
		public static string decisionTextString;

		public Button checkButton;
		public Button callButton;
		public Button raiseButton;
		public Button foldButton;
		public Button dealNextHandButton;

		public static bool playersTurn;

		public static float amountDue; //this will be increased when the computer raises
		public GameObject audioBox;
		public float sensitivity = 30f;

		void Start(){
			
			playersTurn = true;
			potBalance = 0;
			amountDue = 0;

			//at the beginning both players must pay blinds, so:
			GameController.playerBalance-= bigBlind;
			GameController.computerBalance -= bigBlind;
			potBalance += (2 * bigBlind);

		}

		void Update(){
			//the minimum and maximum values on the raise slider cannot exceed the player's balance
			raiseSlider.minValue = (bigBlind)<GameController.playerBalance?bigBlind:GameController.playerBalance;
			raiseSlider.maxValue = GameController.playerBalance<GameController.computerBalance?GameController.playerBalance:GameController.computerBalance;
			bigBlindText.text = "Big Blind: " + bigBlind;
			if (GameController.playerBalance == 0) { //if the player has no money he cant raise
				raiseButton.interactable = false;
			} else {
				
				if (GameController.isAllIn) {
					raiseButton.interactable = false;
				} else {
					raiseButton.interactable = true;
				}
			}
			//if the computer didnt raise him/her, disable the call and the fold buttons
			if (amountDue == 0) {
				checkButton.interactable = true;
				callButton.interactable = false;
				if (GameController.isAllIn) {
					callButton.interactable = true;
				}
				foldButton.interactable = false;

			} else {
				checkButton.interactable = false;
				callButton.interactable = true;
				foldButton.interactable = true;
			}
			callCostText.text = "(" + amountDue.ToString ("0.00") + ")";

			//update all of the labels
			potBalanceText.text = "Pot: " + potBalance.ToString("0.00");
			playerBalanceText.text = "Balance: " + GameController.playerBalance.ToString ("0.00");
			computerBalanceText.text = "Balance: " + GameController.computerBalance.ToString ("0.00");
			decisionText.text = decisionTextString;

			bigBlind = 10 + (5* (int)(GameController.handsPlayed/10)); //every 10 games increase the blind by 5
		
			if (GameController.gameEnded == true) {
				dealNextHandButton.interactable = true;
			}

			if(Input.GetKeyDown(KeyCode.JoystickButton3)){ //triangle
				if (raiseButton.IsInteractable ()) {
					OnRaise ();
				}
			}
			if(Input.GetKeyDown(KeyCode.JoystickButton0)){ //Square
				if (callButton.IsInteractable ()) {
					OnCall ();
				}
			}
			if(Input.GetKeyDown(KeyCode.JoystickButton2)){ //Circle
				if(foldButton.IsInteractable()){
					OnFold();
				}
			}
			if(Input.GetKeyDown(KeyCode.JoystickButton1)){ //X
				if (checkButton.IsInteractable()) {
					OnCheck();
				}
			}
			if(Input.GetKeyDown(KeyCode.JoystickButton4)){ //L1
				OnMin();
			}
			if(Input.GetKeyDown(KeyCode.JoystickButton6)){ //L2
				On3BB();
			}
			if(Input.GetKeyDown(KeyCode.JoystickButton5)){ //R1
				OnPot();
			}
			if(Input.GetKeyDown(KeyCode.JoystickButton7)){ //R2
				OnMax();
			}
//			if (Input.GetAxis ("PS4_LeftAnalogHorizontal") > 0 ) {
//				raiseSlider.value += Input.GetAxis ("PS4_LeftAnalogHorizontal") *sensitivity;
//				displayRaiseAmount.text = raiseSlider.value.ToString ("0.00");
//			}

			if (Input.GetKeyDown (KeyCode.JoystickButton13)) {
			if (dealNextHandButton.IsInteractable ()) {
				OnDealNextHand ();
			}
		}
			if(Input.GetKeyDown(KeyCode.JoystickButton10) ||Input.GetKeyDown(KeyCode.JoystickButton10) || Input.GetKeyDown(KeyCode.JoystickButton11) ||Input.GetKeyDown(KeyCode.JoystickButton12)){
				if (dealNextHandButton.IsInteractable ()) {
					OnDealNextHand ();
				}
			}

		}

		public void OnDealNextHand() {
			GameController.gameEnded = false;

			PlayerPrefs.SetFloat ("playerBalance", GameController.playerBalance);
			PlayerPrefs.SetFloat ("computerBalance", GameController.computerBalance);
			SceneManager.LoadScene(0);
			decisionTextString = "";
		}
		public void OnCheck ()
		{
			raiseAmount = 0;
			audioBox.GetComponentInChildren<AudioBoxScript> ().playButtonAudio ();
			//if the player simply checks, it becomes the computer's turn to decide what to do 
			playersTurn = false;
		}
		public void OnCall ()
		{
			raiseAmount = 0;
			audioBox.GetComponentInChildren<AudioBoxScript> ().playButtonAudio ();
			//when simply calling (aka paying the amount due, send this amount from the player's balance to the pot)
			GameController.playerBalance = GameController.playerBalance - amountDue;
			potBalance = potBalance + amountDue;
			amountDue = 0;
			GameController.currentState++; //telling the game controller to show the next card(s) or compare hands if the round is over
			playersTurn = false;
		}
		public void OnRaise ()
		{
			audioBox.GetComponentInChildren<AudioBoxScript> ().playButtonAudio ();
			//if the player raises, some of his money will go to the pot
			raiseAmount = raiseSlider.value;
			GameController.playerBalance = GameController.playerBalance - raiseAmount;
			potBalance = potBalance + raiseAmount;

			amountDue = 0;

			GameController.numberOfRaises++;
			playersTurn = false;
		}
		public void OnFold ()
		{
			raiseAmount = 0;
			audioBox.GetComponentInChildren<AudioBoxScript> ().playButtonAudio ();
			//when the player folds, the money goes to the computer
			GameController.computerBalance = GameController.computerBalance + potBalance;
			potBalance = 0;
			potBalanceText.text = "Pot: " + potBalance.ToString ("0.00");
			computerBalanceText.text = "Balance: " + GameController.computerBalance.ToString ("0.00");
			GameController.numberOfFolds++;
			GameController.gameEnded = true;
		}

		public void OnMin(){
			raiseSlider.value = raiseSlider.minValue;
		}
		public void On3BB(){
			raiseSlider.value = 3*bigBlind;
		}
		public void OnPot(){
			raiseSlider.value = potBalance;
		}
		public void OnMax(){
			raiseSlider.value = raiseSlider.maxValue;
		}
		public void OnSliderMoved(){
			displayRaiseAmount.text = raiseSlider.value.ToString ("0.00");
		}

		public void OnSaveAndExit(){
			//since the variables will be saved already, we just exit
			Application.Quit ();

		}
		public void OnResetProgress(){
			//reset the variables to their default value
			PlayerPrefs.SetFloat ("playerBalance", 300);
			PlayerPrefs.SetFloat ("computerBalance", 5000);
			SceneManager.LoadScene(0);
		}
	}
}