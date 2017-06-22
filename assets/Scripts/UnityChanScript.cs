using UnityEngine;
using System.Collections;

public class UnityChanScript : MonoBehaviour {

	public static Animator anim;

	void Start () {
		anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		
		if(Input.GetKeyDown("1")){
			anim.Play("WAIT01",-1,0f); //-1 is the default layer, and 0f means start from the beginning
		}
		if(Input.GetKeyDown("2")){
			anim.Play("WAIT02",-1,0f); 
		}
		if(Input.GetKeyDown("3")){
			anim.Play("WAIT03",-1,0f); 
		}
		if(Input.GetKeyDown("4")){
			anim.Play("WAIT04",-1,0f); 
		}
		if (Input.GetMouseButtonDown(0)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray,out hit)) {
				if (hit.transform.name == "unitychan") {
					Debug.Log ("UnityChan is clicked by mouse");
					if (Random.Range (0, 10) <= 2) {  //a 1 in 5 chance she gets knocked over when clicked on
						anim.Play ("DAMAGED01", -1, 0f);
					} else {
						anim.Play ("DAMAGED00", -1, 0f);
					}
				}

			}
		//if (Input.GetMouseButtonDown (0)) {
		//	anim.Play ("DAMAGED00", -1, 0f);
		//}
		}
	}
}
