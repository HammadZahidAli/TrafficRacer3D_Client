using UnityEngine;
using System.Collections;
 
public class rotateCar : MonoBehaviour{
    
	//float visible in the inspector
    public float sensitivity;
	
	//variables not visible in the inspector
    Vector3 mouseReference;
    Vector3 mouseOffset;
    Vector3 rotation;
     
    void Start(){
	//set the rotation of the platform to 0
    rotation = Vector3.zero;
    }
     
    void Update(){
	//if start panel is not visible anymore, check for mouse button 0 (touch on mobile devices)
	if(!garage.startPanel.activeSelf){
	if(Input.GetMouseButtonDown(0)){
	//reference to the first touched/clicked position
	mouseReference = Input.mousePosition;
	}
    if(Input.GetMouseButton(0)){
	//if finger or mouse button stays down, rotate based on mouse/finger position
    mouseOffset = (Input.mousePosition - mouseReference);
    rotation.y = -(mouseOffset.x + mouseOffset.y) * sensitivity;
    transform.Rotate(rotation);
    mouseReference = Input.mousePosition;
    }
	else{
	//if button/finger is not down, rotate the car
	transform.Rotate(Vector3.up * sensitivity);	
	}
    }
	}
}
