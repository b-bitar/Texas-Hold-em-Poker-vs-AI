using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PokerGame
{
	//this class will represent the visual aspect of a card
	public class Card : MonoBehaviour
    {
		private CardFront front;
		public bool isFaceUp;
	
		private CardData data;

		//this makes the data read only and unchangeable from other classes
		public CardData Data {
			get {
				return this.data;
			}
		}


		public void Initialize(int cardIndex){
			isFaceUp = false;
			data = new CardData (cardIndex);

			front = GetComponentInChildren<CardFront>();

			front.Initialize (); //load the materials into the static array if necessary
			front.SetCard (data.index); //give the card the correct material, based on its index
		}

		public void Initialize(int cardValue, int cardSuit)
        {  
			isFaceUp = false;
			data = new CardData (cardValue,cardSuit);
			front = GetComponentInChildren<CardFront>();

			front.Initialize ();
			front.SetCard (data.index);
        }
			
		public void flipCard(){
			if (isFaceUp == true) {
				return; //because it's already faceup
			}

			//if it wasnt showing, rotate it and set isFaceUp to true (in case this function is called again)

			//Quaternion rotation = Quaternion.LookRotation (new Vector3(0,180,270));

			//Quaternion current = transform.localRotation;
			//for (int i = 0; i < 1000; i++) {
			//	Quaternion newRotation = Quaternion.AngleAxis (90, Vector3.up);
			//	transform.rotation = Quaternion.Slerp (transform.rotation, newRotation, 0.05f);
			//}

			//TODO: rotate the cards
			//for (int i = 0; i < 100; i++) {
			//	float x = 20;
			//	float y = 180;
				this.transform.Rotate(new Vector3(20f, 180f, 0f));
			//}

			isFaceUp = true;

		}
    }
}
