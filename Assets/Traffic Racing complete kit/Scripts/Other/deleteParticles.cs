using UnityEngine;
using System.Collections;

public class deleteParticles : MonoBehaviour {

	void Start(){
	//delete gameobject (particles) after 1 second
	Destroy(gameObject, 1);
	}
}
