using System;
using UnityEngine;
using System.Collections;

namespace PokerGame
{
	public class DecisionMaker
    {
        const float callThresholdDefault = 5;
        const float raiseThresholdDefault = 6; //default values from http://www.simplyholdem.com/chen.html are 8 and 9, but here I lowered them to make the game more enjoyable

        public Card card1;
        public Card card2;
        public Hand finalHand;

		//Random randNum;
        CardData[] allPossibilities; //an array that will hold 7 cards

		public DecisionMaker(Card c1, Card c2)
        {
			card1 = c1;
			card2 = c2;
            //card1 = new Card(9, 3);
            //card2 = new Card(12, 0);
            allPossibilities = new CardData[7];
            finalHand = new Hand();
        }

        public float initialHandStrength()
        {
            //this formula depends on 3 things: strength of highest card, closeness of cards, and whether or not they
            //have the same suit
            //It is based on the "Chen Formula" developed by Bill Chen
            float strength = 0.0f;
            int biggerValue;
			if (card1 == null) {
				return 1.0f;
			}
			if (card1.Data.value > card2.Data.value)
            {
				biggerValue = card1.Data.value;
            }
            else
            {
				biggerValue = card2.Data.value;
            }
            switch (biggerValue)
            {
                //if ace assign 10 points
                case 14:
                    strength = 10;
                    break;
                //if king assign 8 points
                case 13:
                    strength = 8;
                    break;
                //if queen assign 7 points
                case 12:
                    strength = 7;
                    break;
                //if jack assign 6 points
                case 11:
                    strength = 6;
                    break;
				//for 10 to 2, give each card half of the face value (example a 7 gets a score of 3.5)
                default:
                    strength = biggerValue / 2.0f;
                    break;
            }
            //now that we've assigned the strength we check for pairs and double it if we find any, with a minimum of 5 points
            //for any pair, and a pair of fives is given an extra point (i.e. 6 points)
			if (card1.Data.value == card2.Data.value)
            {
                strength = strength * 2;
                if (strength < 5)
                {
                    strength = 5;
                }
				if (card1.Data.value == 5)
                {
                    strength = 6;
                }
            }
            //if the 2 cards are suited we add 2 points
			if (card1.Data.suit == card2.Data.suit)
            {
                strength += 2;
            }
            //if the cards are close to each other, they score higher.
            //the farthest apart they are, the more points are lost
            //the hand loses 1 point for 1 gappers (AQ, J9), 2 points for 2 gappers (J8, AJ), 4 points for 3 gappers(J7,84), 5 points for larger gappers including A2, A3, and A4
			if (Math.Abs(card1.Data.value - card2.Data.value) == 2)
            {
                strength -= 1;
            }
			else if (Math.Abs(card1.Data.value - card2.Data.value) == 3)
            {
                strength -= 2;
            }
			else if (Math.Abs(card1.Data.value - card2.Data.value) == 4)
            {
                strength -= 4;
            }
			else if (Math.Abs(card1.Data.value - card2.Data.value) >= 5)
            {
                strength -= 5;
            }

            //finally, the last part of the formula states that if the 2 cards are connected or (1-gap apart and are both
            //lower value than a Queen) we must add a point since you can then make all higher straights
			if (((Math.Abs(card1.Data.value - card2.Data.value) == 2) && biggerValue < 12) || (Math.Abs(card1.Data.value - card2.Data.value) == 1))
            {
                strength++;
            }
            Console.WriteLine("value is " + strength.ToString());
            return strength;
        }

        //the preflop action depends on how strong the AI's hand is, and whether the opponent is a raiser or a folder
        public float blindAction(float strengthOfHoleCards, float courage)
        {
            //the return values will be as follows:
            // 0 	in case we have to check/ fold
            // 1 	in case we need to call
            // 2+   any number from 2.0 and above to indicate a raise of X big blinds 
            float callThreshold = callThresholdDefault - courage;
            float raiseThreshold = raiseThresholdDefault - courage;

            return (decideOnAction(strengthOfHoleCards, callThreshold, raiseThreshold, courage));
        }


        //this function takes the 3 flop cards and returns a decision according to the AI's 2 cards and input
        public float preTurnAction(Card[] flopCards, float courage)
        {
            //allPossibilities array will hold all the simulations (including the last 2 unknown cards)
			allPossibilities[0] = card1.Data;
			allPossibilities[1] = card2.Data;
			allPossibilities[2] = flopCards[0].Data;
            allPossibilities[3] = flopCards[1].Data;
            allPossibilities[4] = flopCards[2].Data;
            //now for the last 2 cards, we will go through the (52 -2 -3 = 47) cards of the deck and evaluate the hand on every occasion
            //each hand will then be added to the total score
			CardData[] remainingCards = new CardData[47];
            bool isUsedUp = false;
            int k = 0; //this will traverse the remaining cards array
            int totalStrength = 0;

			//this cardData array will have a list of 52 new cards. Then we'll remove the 5 that we know can not come up as the last 2 cards
			CardData[] tempDeck = new CardData[52];
			for (int i = 0; i < 52; i++) {
				tempDeck [i] = new CardData (i);
			}
            for (int i = 0; i < 52; i++, k++)
            {
                for (int j = 0; j < 5; j++)
                {
					if (allPossibilities[j].index == tempDeck[i].index)
                    {
                        isUsedUp = true;
                    }
                }
				//if the card is being used (whether by the flop or by us(the AI), do not insert it and do not increment k)
                if (isUsedUp)
                {
                    isUsedUp = false;
                    k--;
                }
                else
                {
					remainingCards[k] = tempDeck[i];
                    // Console.Write(" " + tempDeck.cards[i].printCard());
                }
            }

            //now we insert all the combinations for the last 2 cards, get the best 5 out of 7 hand, and input its score
            for (int i = 0; i < 46; i++)
            {
                for (int j = i + 1; j < 47; j++)
                {
					//insert the last 2 cards into the 7 element array and see what will the best hand be
                    allPossibilities[5] = remainingCards[i];
                    allPossibilities[6] = remainingCards[j];
                    Hand tempHand = new PokerGame.Hand();
                    tempHand = Hand.getStrongestHand(allPossibilities);
                    tempHand.evaluate();
                    totalStrength += tempHand.strength;
                    //Console.WriteLine(i + " " + j + " " + totalStrength);
                    //tempHand.printHand();
                }
            }
            //the above 2 for loops, based on trial and error and some calculations roughly provide the following guidlines towards what we have till now
            //totalStrenght = 3000  nothing; 34,000  pair; 150,000 two pair; 442K 3 of a kind; 275K  straight; 550K flush (with no straight flush potential);
            //912K full house, 4.5 Million 4 of a kind; and totalStrength = 70,236,894 if a straight flush already exists (before turn and river are revealed). This is the maximum possible score.

            float callThreshold = (callThresholdDefault * 10000) - (courage * 5000); //this way default callthreshold is 8000, and fluctuates based on the courage of the enemy
            float raiseThreshold = (raiseThresholdDefault * 20000) - (courage * 5000);

            Console.WriteLine(decideOnAction(totalStrength * 1.0f, callThreshold, raiseThreshold, courage));
            return (decideOnAction(totalStrength * 1.0f, callThreshold, raiseThreshold, courage));
        }

        //this will be called after the turn is shown, but before the river card is exposed
        //so, it only has 46 cards to simulate and traverse (since 52 - 2 - 3 - 1 = 46). 2 cards for the computer, 3 for the flop, and 1 for the turn
        public float preRiverAction(Card[] flopCards, Card turn, float courage)
        {
            //this function usually is preceeded by the preTurnAction function, but just in case for some reason allPossibilities wasnt filled already we will refill it here
			allPossibilities[0] = card1.Data;
            allPossibilities[1] = card2.Data;
			allPossibilities[2] = flopCards[0].Data;
			allPossibilities[3] = flopCards[1].Data;
            allPossibilities[4] = flopCards[2].Data;
            allPossibilities[5] = turn.Data;

            CardData[] remainingCards = new CardData[46];
			CardData[] tempDeck = new CardData[52];
			for (int i = 0; i < 52; i++) {
				tempDeck [i] = new CardData (i);
			}
            bool isUsedUp = false;
            int k = 0; //this will traverse the remaining cards array
            int totalStrength = 0;
            for (int i = 0; i < 52; i++, k++)
            {
                for (int j = 0; j < 6; j++)
                {
					if (allPossibilities[j].index == tempDeck[i].index)
                    {
                        isUsedUp = true;
                    }
                }
                //if the card is being used (whether by the flop, turn or by the computer, do not insert it and do not increment k)
                if (isUsedUp)
                {
                    isUsedUp = false;
                    k--; //to cancel out the k++ in the for loop
                }
                else
                {
                    //include this unused card in our possible river cards
                    remainingCards[k] = tempDeck[i];
                }
            }

            //now we insert all the combinations for the last card, get the best 5 out of 7 hand, and input its score
            for (int i = 0; i < 46; i++)
            {
				//insert all the possibilities of the last card in the 7 card array and retreive the strongest hand
                allPossibilities[6] = remainingCards[i];
                Hand tempHand = new PokerGame.Hand();
                tempHand = Hand.getStrongestHand(allPossibilities);
                tempHand.evaluate();
                totalStrength += tempHand.strength;
                Console.WriteLine(i + " " + totalStrength);
                //tempHand.printHand();
            }

            //in here the totalScore varies from a minimum of 18 (when the 6 card hand is only high cards (since 3 possibilities of pairs))
            //to a maximum of 2,988,804 if a straight flush already exists (that is 46 times the score of a straight flush)
            float callThreshold = (callThresholdDefault * 50 - 50) - courage; //when courage is 0 the default is 350, which is reasonable since having a pair scores 364 (in other words, you must have at least a pair to call) 
            float raiseThreshold = (raiseThresholdDefault*1111) - courage; //more than 10000 to raise, if courage is 0
            return decideOnAction(totalStrength, callThreshold, raiseThreshold, courage);
        }

        //the last time to make a decision is after the river
        public float postRiverAction(Card[] flopCards, Card turn, Card river, float courage)
        {
            //now that we know all the cards, we can easily and definitively determine our hand
            allPossibilities[0] = card1.Data;
			allPossibilities[1] = card2.Data;
			allPossibilities[2] = flopCards[0].Data;
			allPossibilities[3] = flopCards[1].Data;
			allPossibilities[4] = flopCards[2].Data;
			allPossibilities[5] = turn.Data;
			allPossibilities[6] = river.Data;

            finalHand = Hand.getStrongestHand(allPossibilities);
            finalHand.evaluate(); //now our hand's strength is based purely on what kind of hand we have with
            //64974 for a straight flush; 4164 for a fourOfAKind; 693 for a full house; 508 for a flush; 254 for a straight; 46 for 3OfAKind
            //20 for two pairs; 1 for a pair; and 0 for high cards
            float callThreshold = callThresholdDefault;
            float raiseThreshold = raiseThresholdDefault;
            //we will solely base our decision of thresholds on courage
            if(courage > 2)
            {
                callThreshold = 1; //if he raises always, a pair might be enough
                raiseThreshold = 45; //raise if you have 3 of a kind or stronger
            }else if(courage > 1)
            {
                callThreshold = 1; 
                raiseThreshold = 254;
            }else if (courage >= 0)
            {
                callThreshold = 15;//call if you have a 2 pair or stronger
                raiseThreshold = 250;
            }else if (courage < 0)
            {
                callThreshold = 45; //3 of a kind or better
                raiseThreshold = 500; //raise when you have a flush or better
            }

            return (decideOnAction(finalHand.strength,callThreshold,raiseThreshold,courage));
        }

        //takes the strength, threshold 1 (call threshold) , threshold 2 (raise threshold), and courage
        //to return 0 (check/fold), 1 (call), or 2+ (raise by 2 big blinds or more)
        public float decideOnAction(float currentStrength, float t1, float t2, float courage)
        {
            float callThreshold = t1;
            float raiseThreshold = t2;

            if (currentStrength < callThreshold)
            {
                return 0;
            }
            else if (currentStrength >= callThreshold && currentStrength < raiseThreshold)
            {
                return 1;
            }
            else //if(currentStrength >= raiseThreshold)
            {
                //when hand is stronger than raise threshold,we must raise but at least 2 big blinds are allowed
                if (currentStrength - raiseThreshold < 2.0f)
                {
                    return 2.0f; //which is the minimum raise allowed  (2 big blinds)
                }
                //if the human opponent is a coward, the AI must raise by a random value between 2 and 3 BBs, if he/she raises moderately
                //the AI's raise must fall between 2 and 5 BBs
                //If the human raises on average more than once per hand, the computer can reach up to 10 BBs
                if (courage < 0)
                {
					return (UnityEngine.Random.value + 2.0f); //between 2 and 3BBs
                }
                else if (courage >= 0 && courage < 1)
                {
					return ((UnityEngine.Random.value * 3f) + 2.0f); //between 2 and 5 BBs
                }
                else
                {
                    //when hand is strong and enemy is brave
					return (UnityEngine.Random.value * 8f + 2.0f); //between 2 and 10 big blinds
                }
            }
        }

    }
}
