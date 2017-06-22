using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuCanvas : MonoBehaviour {

	public float speed = 3f;
	public Text speedIndicator;

	void Start () {
		//DontDestroyOnLoad (transform.gameObject);
	}

	void Update () {
	
	}
	public void OnSlow(){
		speedIndicator.text = "Slow";
		speedIndicator.color = Color.blue;
		speed = 3f;
	}
	public void OnFast(){
		speedIndicator.text = "Fast";
		speedIndicator.color = Color.red;
		speed = 5f;
	}
	public void OnPlay(){
		SceneManager.LoadScene(1);
	}

}
