using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;


namespace PokerGame
{
	public class Hand
    {
        //a PLAYER's or AI's final hand in texas holdem poker is composed of 5 cards
        public CardData[] cards;
        public int strength;

        public Hand()
        {
			//an empty hand is 5 cards, with an unassigned strength value of -1
            cards = new CardData[5];
            strength = -1;

        }
        //inserts a card in the next available slot (if any)
		//note that if the hand is already full, the card to be inserted will just be ignored
        public void insertCard(CardData c)
        {
            for (int i = 0; i < 5; i++)
            {
                if (cards[i] == null)
                {
                    cards[i] = c;
                    return;
                }
            }
        }

		public override string ToString()
        {
			string str = "";
            if (this == null)
            {
				str = "hand is not created";
                return str;
            }
            for (int i = 0; i < 5; i++)
            {
                if (cards[i] == null) //if there is a missing card in the hand, print unknown and carry on
                {
                    str+="unknown ";
                }
                else
                {
					str = str + cards[i].ToString() + " ";
                }
            }
			return str;
        }

        public void sortHand()
        {
			IEnumerable<CardData> query = cards.OrderByDescending(cardData => cardData.value);
            int i = 0;
            foreach (CardData cardData in query)
            {
                //or we can clear cards and insert them again one by one
                cards[i] = cardData;
                i++;
            }
        }

		//to check if a hand is a straight flush, we sort it in decreasing order and compare adjacent cards:
		//if the first is greater than the second by exactly one value, and they have the same suit, we move on to the 2nd and 3rd cards
        public bool isStraightFlush()
        {
            int i = 0;
            this.sortHand();
            //to check for having an ACE 5 4 3 2  (which is a straight but it wont be detected in the below for loop)
			if (cards[0].value == 14 && cards[1].value == 5 && cards[0].suit == cards[1].suit)
            {
                i++;
            }
            //if an ace was followed by a suited 5, we skip the first comparison as we checked it is correct, and continue with
            //our loop normally to check if the other 4 cards are 5 4 3 2 respectively
            for (; i < 4; i++)
            {
				if ((cards[i].value != (cards[i + 1].value + 1)) || (cards[i].suit != cards[i + 1].suit))
                {
                    return false;
                }
            }
            return true;
        }

        public bool isFourOfAKind()
        {
            this.sortHand();
            //we need to check for the 2 cases Big Big Big Big Small and Big Small Small Small Small
            //These are the only 2 cases since hand is sorted in decreasing order
            for (int i = 0; i < 3; i++)
            {
				if (cards[i].value != cards[i + 1].value)
                {
                    //if B B B B S didnt work, we have to try and check if it is a B S S S S case
                    for (int j = 1; j < 4; j++)
                    {
						if (cards[j].value != cards[j + 1].value)
                        {
                            return false;
                        }
                    }
                    return true; //it means B S S S S
                }
            }
            return true; //it means B B B B S
        }

        public bool isFullHouse()
        {
            this.sortHand();
            //basically a full house can only be XXXYY or XXYYY
            //in other words the first 2 cards need to form a pair, and so do the last
            bool firstTwoAreAPair, lastTwoAreAPair, middleCardMakesAPair;
			firstTwoAreAPair = cards[0].value == cards[1].value;
			lastTwoAreAPair = cards[3].value == cards[4].value;
			middleCardMakesAPair = (cards[1].value == cards[2].value || cards[2].value == cards[3].value);

            return (firstTwoAreAPair && lastTwoAreAPair && middleCardMakesAPair);
        }

        public bool isFlush()
        {
            this.sortHand(); //not required, but good for comparing later on
            //we only need all their suits to match
            for (int i = 0; i < 4; i++)
            {
				if (cards[i].suit != cards[i + 1].suit)
                {
                    return false;
                }
            }
            return true;
        }

        public bool isStraight()
        {
            this.sortHand();
            int i = 0;
            //to check for having an ACE 5 4 3 2  (which is a straight but it wont be detected in the below for loop)
			if (cards[0].value == 14 && cards[1].value == 5)
            {
                i++;
            }

            for (; i < 4; i++)
            {
				if (cards[i].value != (cards[i + 1].value + 1))
                {
                    return false;
                }
            }
            return true;
        }

        public bool isThreeOfAKind()
        {
            this.sortHand();
            //there are 3 ways for a three of a kind to happen:  AAABC, ABBBC, and ABCCC
            //such that A<>B and B<> C   (where <> stands for not equal)
            bool aaabc, abbbc, abccc;
            //for extra safety to not mistake a three of a kind for four of a kinds and full houses,
            //we can add the "a is not equal to b" and "b is not equal to c" checks, but no need 
			//since this check wont be called unless we are sure that no stronger hand can be formed from these cards
			aaabc = cards[0].value == cards[1].value && cards[1].value == cards[2].value;
			abbbc = cards[1].value == cards[2].value && cards[2].value == cards[3].value;
			abccc = cards[2].value == cards[3].value && cards[3].value == cards[4].value;

            return (aaabc || abbbc || abccc);
        }

        public bool isTwoPair()
        {
            this.sortHand();
            //the three cases are  AA BB C , AA B CC, and A BB CC
            //we will implement the cheapest comparisons necessary to find and ensure that we have a 2 pair, since if we had a stronger hand
            //it wouldnt matter if it is considered as a two pair
            bool aabbc, aabcc, abbcc;
			aabbc = (cards[0].value == cards[1].value && cards[2].value == cards[3].value);
			aabcc = (cards[0].value == cards[1].value && cards[3].value == cards[4].value);
			abbcc = (cards[1].value == cards[2].value && cards[3].value == cards[4].value);

            return (aabbc || aabcc || abbcc);
        }

        public bool isPair()
        {
            this.sortHand(); //so that the pair would stick to each other since they'll have the same value
            for (int i = 0; i < 4; i++)
            {
				if (cards[i].value == cards[i + 1].value)
                {
                    return true;
                }
            }
            return false;
        }

		//this assigns a value for the strength variable (if it hasnt been assigned one already)
        public void evaluate()
        {
            for (int i = 0; i < 5; i++)
            {
                if (cards[i] == null)
                {
                    return;
                }
            }
            if (strength < 0)
            {
				//the strength values are higher for more rare/stronger hands
				//These indicate the odds of a hand being acheived. See: https://en.wikipedia.org/wiki/Poker_probability
                if (isStraightFlush())
                {
                    strength = 64974; //2598960 / 40
                    return;
                }
                else if (isFourOfAKind())
                {
                    strength = 4164;//Since 624 four of a kinds, we do 2598960 / 624 = 
                    return;
                }
                else if (isFullHouse())
                {
                    strength = 693;//there are 3,744 ways to get a full house
                    return;
                }
                else if (isFlush())
                {
                    strength = 508;//only 5,108 flushes
                    return;
                }
                else if (isStraight())
                {
                    strength = 254;//there are 10,200 straights
                    return;
                }
                else if (isThreeOfAKind())
                {
                    strength = 46;//54,912 three of a kinds
                    return;
                }
                else if (isTwoPair())
                {
                    strength = 20;// there are 123,552 two pairs
                    return;
                }
                else if (isPair())
                {
                    strength = 1;// since there are 1,098,240 cases
                    return;
                }
                else
                {
                    strength = 0;
                    return;
                }
            }
        }

        //returning the max hand out of all 21 hands
        public static Hand getStrongestHand(CardData[] cards)
        {
			//the input here is a 7 card card array. This function returns the "best 5" that make the best hand possible (or equivalent)
            Hand response = new Hand();
            Hand[] allCombinations = getHandCombinations(cards); //array of 21 hands to consider
            response = allCombinations[0];
            for (int i = 1; i < 21; i++)
            {
                //if the hand that we're considering now is stronger than our maximum hand, set maxHand = currentHand
                if (Hand.compare(response, allCombinations[i]) == allCombinations[i])
                {
                    response = allCombinations[i];
                }
            }
            return response;
        }

        //takes an array of 7 elements and returns all the possible combinations of hands (21)
        //this function is generalized for all arrays, but will only be used for 1 case
        private static Hand[] getHandCombinations(CardData[] cards)
        {
            Hand[] combinations = new Hand[21];
            int row = 0; //to trace which hand we are inserting in
            for (int i = 0; i < cards.Length - 4; i++)
            {
                for (int j = i + 1; j < cards.Length - 3; j++)
                {
                    for (int k = j + 1; k < cards.Length - 2; k++)
                    {
                        for (int l = k + 1; l < cards.Length - 1; l++)
                        {
                            for (int m = l + 1; m < cards.Length; m++)
                            {
                                combinations[row] = new Hand();
                                combinations[row].cards[0] = cards[i];
                                combinations[row].cards[1] = cards[j];
                                combinations[row].cards[2] = cards[k];
                                combinations[row].cards[3] = cards[l];
                                combinations[row].cards[4] = cards[m];
                                row++;
                            }
                        }
                    }
                }
            }
            return combinations;
        }

        //a function that returns the bigger hand if one of the hands is bigger than the other.
        //Otherwise, if the two hands are equal it returns null
        public static Hand compare(Hand h1, Hand h2)
        {
			//evaluate each of the hands, so that 1. they get sorted in decescending order and 2. their strength values are correct
			h1.evaluate();
			h2.evaluate();

			CardData[] h1Cards = new CardData[5]; //since we dont need to draw these cards physically, we only represent them as 'CardData's
			CardData[] h2Cards = new CardData[5];

            for(int i = 0; i < 5; i++)
            {
				h1Cards [i] = new CardData (h1.cards[i].index);
				h2Cards [i] = new CardData (h2.cards [i].index);

            }

			if (h1.strength > h2.strength)
            {
                return h1;
            }
            else if (h2.strength > h1.strength)
            {
                return h2;
            }
            //if both hands have the same strength value we need to compare each card individually
			//we can also check by h1.isStraight() and isStraightFlush
            if (h1.strength == 254 || h1.strength == 64974) //if the hands are straights or straight flushes
            {
                //we need to check for the special case of Ace 5 4 3 2
                //in this case the straight is the weakest kind it can possibly be(aka straight to 5), so we set the ace's value to 0 to not trump any other different hand
				//note that here we are setting the value in our temp array of cardDatas so that we dont ruin the original hands h1 and h2
				if (h1Cards[0].value == 14 && h1Cards[1].value == 5)
                {
					h1Cards[0].value = 0; //to make it the weakest straight
                }
				if (h2Cards[0].value == 14 && h2Cards[1].value == 5)
                {
					h2Cards[0].value = 0;
                }
            }
            //now we compare each card from largest to smallest
            for (int i = 0; i < 5; i++)
            {
				if (h1Cards[i].value > h2Cards[i].value)
                {
                    return h1;
                }
				else if (h2Cards[i].value > h1Cards[i].value)
                {
                    return h2;
                }
            }
            //if the loop ends, it means both hands have exactly the same values
            return null;
			//another idea is to return either hand since they are equal, but this is more descriptive
        }
    }
}
