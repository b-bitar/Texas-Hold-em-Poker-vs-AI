using System;

//this class will hold all the data that a card holds, but does not deal with any visuals
public class CardData
{
	public enum Suit { Clubs, Diamonds, Hearts, Spades
	};
	public int index; //from 0 to 51
	public int value;
	public Suit suit;

	public CardData (int cardIndex)
	{
		index = cardIndex;
		value = cardIndex % 13 + 2; //so that 0 is 2, 1 is 3, 2 is 4, and so on until 11 is a king, and 12 is an ACE with a value of 14

		//Now, we make the first 13 numbers to be clubs (2 to ace), the second 13 numbers will be diamonds
		//and so on alphabetically
		suit = (Suit)(cardIndex/13);
	}

	public CardData( int cardValue, int cardSuit){

		value = cardValue;
		suit = (Suit)cardSuit;
		index = (cardSuit * 13) + cardValue -2;  

	}

	//for debugging purposes only
	public override string ToString()
	{
		string valueString;
		switch (this.value) {
		case 11: valueString = "Jack";
			break;
		case 12: valueString = "Queen";
			break;
		case 13: valueString = "King";
			break;
		case 14: valueString = "Ace";
			break;
		default: valueString = value.ToString();
			break;
		}          
		return (valueString+ " of " + suit.ToString());
	}
}
