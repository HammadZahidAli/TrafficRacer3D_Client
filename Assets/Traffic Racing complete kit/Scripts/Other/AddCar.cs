using UnityEngine;
using System.Collections;

public class AddCar : MonoBehaviour {
	
	//array of cars visible in the inspector
	public GameObject[] drivableCars;
	
	void Start(){
	//instantiate the selected car on the right position
	var pos = new Vector3(10, 0, 0);
	Instantiate(drivableCars[PlayerPrefs.GetInt("selectedCar")], pos, transform.rotation);	
	}
}
