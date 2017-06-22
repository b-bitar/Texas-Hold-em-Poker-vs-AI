using UnityEngine;
using System.Collections;

public class AudioBoxScript : MonoBehaviour {

	public static AudioClip[] checkList;
	public static AudioClip[] callList;
	public static AudioClip[] allInList;
	public static AudioClip[] foldList;
	public static AudioClip[] raiseList;
	public static AudioClip[] tauntList;
	public static AudioClip buttonSound;
	public static AudioClip badCards;
	public static AudioClip pair;
	public static AudioClip loss;
	public static AudioClip cheer;
	public static AudioClip highHole;
	public enum AudioType { Check, Call, AllIn, Fold,Raise,Taunt };

	public GameObject go;

	//void Awake() {
	//	DontDestroyOnLoad (transform.gameObject);
	//}

	void Start () {
		//audio.PlayOneShot (GetComponent<AudioSource> ());

		checkList = new AudioClip[3];
		callList = new AudioClip[3];
		allInList = new AudioClip[3];
		foldList = new AudioClip[3];
		raiseList = new AudioClip[3];
		tauntList = new AudioClip[3];

		//load all the checks
		checkList[0] = Resources.Load("Sounds/Check1") as AudioClip;
		checkList[1] = Resources.Load("Sounds/Check2") as AudioClip;
		checkList[2] = Resources.Load("Sounds/Check3") as AudioClip;

		//load all the calls
		callList[0] = Resources.Load("Sounds/Call1") as AudioClip;
		callList[1] = Resources.Load("Sounds/Call2") as AudioClip;
		callList[2] = Resources.Load("Sounds/Call3") as AudioClip;

		//load all the allIns
		allInList[0] = Resources.Load("Sounds/AllIn1") as AudioClip;
		allInList[1] = Resources.Load("Sounds/AllIn2") as AudioClip;
		allInList[2] = Resources.Load("Sounds/AllIn3") as AudioClip;

		//load all the folds
		foldList[0] = Resources.Load("Sounds/Fold1") as AudioClip;
		foldList[1] = Resources.Load("Sounds/Fold2") as AudioClip;
		foldList[2] = Resources.Load("Sounds/Fold3") as AudioClip;

		//load all the raises
		raiseList[0] = Resources.Load("Sounds/Raise1") as AudioClip;
		raiseList[1] = Resources.Load("Sounds/Raise2") as AudioClip;
		raiseList[2] = Resources.Load("Sounds/Raise3") as AudioClip;

		//load all the taunts
		tauntList[0] = Resources.Load("Sounds/Taunt1") as AudioClip;
		tauntList[1] = Resources.Load("Sounds/Taunt2") as AudioClip;
		tauntList[2] = Resources.Load("Sounds/Taunt3") as AudioClip;

		//load other sounds
		buttonSound = Resources.Load("Sounds/Button1") as AudioClip;
		pair = Resources.Load ("Sounds/Pair") as AudioClip;
		loss = Resources.Load ("Sounds/Loss") as AudioClip;
		cheer = Resources.Load ("Sounds/Woohoo") as AudioClip;
		highHole = Resources.Load ("Sounds/Interesting") as AudioClip;
		badCards = Resources.Load ("Sounds/BadCards") as AudioClip;

		GetComponent<AudioSource>().clip = tauntList[2]; //default audio clip; will be modified later
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.P)) {
			GetComponent<AudioSource>().Play ();
		}
	}
		
	public void playRandomAudio(AudioType at){
		//note that I can only do this here because all arrays are of the same size (3). If we have more audios of a specific kind, more functions would be needed
		int randNumber = Random.Range (0, 3);
		switch (at) {
		case AudioType.Check:
			GetComponent<AudioSource> ().clip = checkList [randNumber];
			break;
		case AudioType.Call:
			GetComponent<AudioSource> ().clip = callList [randNumber];
			break;
		case AudioType.AllIn:
			GetComponent<AudioSource> ().clip = allInList [randNumber];
			break;
		case AudioType.Fold:
			GetComponent<AudioSource> ().clip = foldList [randNumber];
			break;
		case AudioType.Raise:
			GetComponent<AudioSource> ().clip = raiseList [randNumber];
			break;
		case AudioType.Taunt:
			GetComponent<AudioSource> ().clip = tauntList [randNumber];
			break;
		}
		GetComponent<AudioSource> ().Play ();
	}

	public void playPairAudio(){
		GetComponent<AudioSource> ().clip = pair;
		GetComponent<AudioSource> ().Play ();
	}

	public void playLossAudio(){
		GetComponent<AudioSource> ().clip = loss;
		GetComponent<AudioSource> ().Play ();
	}
	public void playCheerAudio(){
		GetComponent<AudioSource> ().clip = cheer;
		GetComponent<AudioSource> ().Play ();
	}
	public void playHighHoleAudio(){
		GetComponent<AudioSource> ().clip = highHole;
		GetComponent<AudioSource> ().Play ();
	}
	public void playButtonAudio(){
		GetComponent<AudioSource> ().clip = buttonSound;
		GetComponent<AudioSource> ().Play ();
	}
	public void playBadCards(){
		GetComponent<AudioSource> ().clip = badCards;
		GetComponent<AudioSource> ().Play ();
	}
}
