using UnityEngine;
using System.Collections;

public class CardFront : MonoBehaviour {

	public static Material[] materials;

	//public int index = 0;
	// Use this for initialization
	public void Initialize () {
		if (materials == null) {
			//in case the materials aren't loaded yet, we need to load them and then use one of them
			materials = new Material[52];
			//load all the clubs
			materials [0] = Resources.Load ("Materials/Clubs/Clubs_2", typeof(Material)) as Material;
			materials [1] = Resources.Load ("Materials/Clubs/Clubs_3", typeof(Material)) as Material;
			materials [2] = Resources.Load ("Materials/Clubs/Clubs_4", typeof(Material)) as Material;
			materials [3] = Resources.Load ("Materials/Clubs/Clubs_5", typeof(Material)) as Material;
			materials [4] = Resources.Load ("Materials/Clubs/Clubs_6", typeof(Material)) as Material;
			materials [5] = Resources.Load ("Materials/Clubs/Clubs_7", typeof(Material)) as Material;
			materials [6] = Resources.Load ("Materials/Clubs/Clubs_8", typeof(Material)) as Material;
			materials [7] = Resources.Load ("Materials/Clubs/Clubs_9", typeof(Material)) as Material;
			materials [8] = Resources.Load ("Materials/Clubs/Clubs_10", typeof(Material)) as Material;
			materials [9] = Resources.Load ("Materials/Clubs/Clubs_Jack", typeof(Material)) as Material;
			materials [10] = Resources.Load ("Materials/Clubs/Clubs_Queen", typeof(Material)) as Material;
			materials [11] = Resources.Load ("Materials/Clubs/Clubs_King", typeof(Material)) as Material;
			materials [12] = Resources.Load ("Materials/Clubs/Clubs_Ace", typeof(Material)) as Material;
			//all the diamonds
			materials [13] = Resources.Load ("Materials/Diamonds/Diamonds_2", typeof(Material)) as Material;
			materials [14] = Resources.Load ("Materials/Diamonds/Diamonds_3", typeof(Material)) as Material;
			materials [15] = Resources.Load ("Materials/Diamonds/Diamonds_4", typeof(Material)) as Material;
			materials [16] = Resources.Load ("Materials/Diamonds/Diamonds_5", typeof(Material)) as Material;
			materials [17] = Resources.Load ("Materials/Diamonds/Diamonds_6", typeof(Material)) as Material;
			materials [18] = Resources.Load ("Materials/Diamonds/Diamonds_7", typeof(Material)) as Material;
			materials [19] = Resources.Load ("Materials/Diamonds/Diamonds_8", typeof(Material)) as Material;
			materials [20] = Resources.Load ("Materials/Diamonds/Diamonds_9", typeof(Material)) as Material;
			materials [21] = Resources.Load ("Materials/Diamonds/Diamonds_10", typeof(Material)) as Material;
			materials [22] = Resources.Load ("Materials/Diamonds/Diamonds_Jack", typeof(Material)) as Material;
			materials [23] = Resources.Load ("Materials/Diamonds/Diamonds_Queen", typeof(Material)) as Material;
			materials [24] = Resources.Load ("Materials/Diamonds/Diamonds_King", typeof(Material)) as Material;
			materials [25] = Resources.Load ("Materials/Diamonds/Diamonds_Ace", typeof(Material)) as Material;
			//all the hearts
			materials [26] = Resources.Load ("Materials/Hearts/Hearts_2", typeof(Material)) as Material;
			materials [27] = Resources.Load ("Materials/Hearts/Hearts_3", typeof(Material)) as Material;
			materials [28] = Resources.Load ("Materials/Hearts/Hearts_4", typeof(Material)) as Material;
			materials [29] = Resources.Load ("Materials/Hearts/Hearts_5", typeof(Material)) as Material;
			materials [30] = Resources.Load ("Materials/Hearts/Hearts_6", typeof(Material)) as Material;
			materials [31] = Resources.Load ("Materials/Hearts/Hearts_7", typeof(Material)) as Material;
			materials [32] = Resources.Load ("Materials/Hearts/Hearts_8", typeof(Material)) as Material;
			materials [33] = Resources.Load ("Materials/Hearts/Hearts_9", typeof(Material)) as Material;
			materials [34] = Resources.Load ("Materials/Hearts/Hearts_10", typeof(Material)) as Material;
			materials [35] = Resources.Load ("Materials/Hearts/Hearts_Jack", typeof(Material)) as Material;
			materials [36] = Resources.Load ("Materials/Hearts/Hearts_Queen", typeof(Material)) as Material;
			materials [37] = Resources.Load ("Materials/Hearts/Hearts_King", typeof(Material)) as Material;
			materials [38] = Resources.Load ("Materials/Hearts/Hearts_Ace", typeof(Material)) as Material;
			//all the spades
			materials [39] = Resources.Load ("Materials/Spades/Spades_2", typeof(Material)) as Material;
			materials [40] = Resources.Load ("Materials/Spades/Spades_3", typeof(Material)) as Material;
			materials [41] = Resources.Load ("Materials/Spades/Spades_4", typeof(Material)) as Material;
			materials [42] = Resources.Load ("Materials/Spades/Spades_5", typeof(Material)) as Material;
			materials [43] = Resources.Load ("Materials/Spades/Spades_6", typeof(Material)) as Material;
			materials [44] = Resources.Load ("Materials/Spades/Spades_7", typeof(Material)) as Material;
			materials [45] = Resources.Load ("Materials/Spades/Spades_8", typeof(Material)) as Material;
			materials [46] = Resources.Load ("Materials/Spades/Spades_9", typeof(Material)) as Material;
			materials [47] = Resources.Load ("Materials/Spades/Spades_10", typeof(Material)) as Material;
			materials [48] = Resources.Load ("Materials/Spades/Spades_Jack", typeof(Material)) as Material;
			materials [49] = Resources.Load ("Materials/Spades/Spades_Queen", typeof(Material)) as Material;
			materials [50] = Resources.Load ("Materials/Spades/Spades_King", typeof(Material)) as Material;
			materials [51] = Resources.Load ("Materials/Spades/Spades_Ace", typeof(Material)) as Material;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
//		if (Input.GetKeyDown (KeyCode.U)) {
//			GetComponent<Renderer> ().material = materials[index];
//			index++;
//			if (index >= 52) {
//				index = 0;
//			}
//		}
	}

	public void SetCard( int index ){
		GetComponent<Renderer> ().material = materials[index];
	}
}
