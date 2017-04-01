using UnityEngine;
using System.Collections;

public class MoveObject : MonoBehaviour {
	
	//visible in the inspector
	public bool car;
	public bool switchLine;
	public int slowDownDistance;
	public float vehicleLength = 6;
	
	[HideInInspector]
	public bool sameDirection = true;
	
	//not visible in the inspector
	float randomSpeed;
	float nextPos;
	bool isMovingLeft;
	bool isMovingRight;
	
	GameObject lightLeft;
	GameObject lightRight;
	
	CarSpawner carSpawner;
	
	void Start(){
	//define random speed for car objects
	randomSpeed = Random.Range(2.5f, 3f);
	
	//if this car can change track, make the change of actually change track smaller
	var random = Random.Range(0, 3);
	if(random == 0 && car && switchLine){
	//start changing track (switching line)
	StartCoroutine(otherLine());	
	}
	
	//find car lights and turn them off
	if(car && switchLine){
	lightLeft = transform.Find("light left").gameObject;
	lightRight = transform.Find("light right").gameObject;
	
	lightLeft.SetActive(false);
	lightRight.SetActive(false);
	
	carSpawner = GameObject.Find("Game Manager").GetComponent<CarSpawner>();
	}
	
	if(!sameDirection && car)
		transform.Rotate(Vector3.up * 180);
	}

	void Update(){	
	//if object is a car, move it with car speed (randomspeed) and if another car is to close, slow down
	if(car){
	if(Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), -transform.right, slowDownDistance)){
	randomSpeed += Time.deltaTime;	
	}
		if(sameDirection){
			transform.Translate(Vector3.right * Time.deltaTime * ScrollTexture.scrollSpeed * randomSpeed);	
		}
		else{
			transform.Translate(Vector3.left * Time.deltaTime * ScrollTexture.scrollSpeed * randomSpeed * 3.5f);	
		}
	}
	//else, move it as fast as all other objects and textures
	else{
	transform.Translate(Vector3.right * Time.deltaTime * ScrollTexture.scrollSpeed * 6f, Space.World);
	}
	
	//destroy all objects after they aren't visible to increase performance
	if(transform.position.x > 15){
	Destroy(gameObject);	
	}
	
	//move car left till it reaches next position
	if(isMovingLeft && transform.position.z > nextPos){
	transform.Translate(Vector3.forward * Time.deltaTime * -3);		
	if(transform.position.z <= nextPos){
	isMovingLeft = false;	
	lightLeft.SetActive(false);
	}
	}
	//move car right till it reaches next position
	if(isMovingRight && transform.position.z < nextPos){
	transform.Translate(Vector3.forward * Time.deltaTime * 3);	
	if(transform.position.z >= nextPos){
	isMovingRight = false;	
	lightRight.SetActive(false);
	}
	}
	}
	
	IEnumerator otherLine(){
	//after instantiating this car, wait for seconds based on car speed (scrollspeed of the textures)
	yield return new WaitForSeconds(Random.Range(0.2f, 0.4f) * (17 - ScrollTexture.scrollSpeed));
	//will this car move left or right?
	var leftRight = Random.Range(0, 2);
	//get and store current z position
	var posZ = transform.position.z;
	
	//by default, moving to anoter line is possible
	bool possible = true;
	
	//set the 5 startpositions of rays
	Vector3 startPos1 = new Vector3(transform.position.x + vehicleLength, transform.position.y + 0.5f, transform.position.z);
	Vector3 startPos2 = new Vector3(transform.position.x + vehicleLength/2, transform.position.y + 0.5f, transform.position.z);
	Vector3 startPos3 = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
	Vector3 startPos4 = new Vector3(transform.position.x - vehicleLength/2, transform.position.y + 0.5f, transform.position.z);
	Vector3 startPos5 = new Vector3(transform.position.x - vehicleLength, transform.position.y + 0.5f, transform.position.z);
	
	if(leftRight == 0 //if car wants to go left and one of the rays hits a car on the left side, possible is false
	&& (Physics.Raycast(startPos1, -transform.forward, 6) 
	|| Physics.Raycast(startPos2, -transform.forward, 6) 
	|| Physics.Raycast(startPos3, -transform.forward, 6) 
	|| Physics.Raycast(startPos4, -transform.forward, 6) 
	|| Physics.Raycast(startPos5, -transform.forward, 6))){ 
	possible = false;	
	}
	if(leftRight == 1 //if car wants to go right and one of the rays hits a car on the right side, possible is false
	&& (Physics.Raycast(startPos1, transform.forward, 6) 
	|| Physics.Raycast(startPos2, transform.forward, 6) 
	|| Physics.Raycast(startPos3, transform.forward, 6) 
	|| Physics.Raycast(startPos4, transform.forward, 6) 
	|| Physics.Raycast(startPos5, transform.forward, 6))){
	possible = false;
	}
	
	if(carSpawner.leftSameDirection != carSpawner.rightSameDirection && 
	((leftRight == 0 && transform.position.z > 0 && transform.position.z < 4) || 
	(leftRight == 1 && transform.position.z < 0 && transform.position.z > -4)))
		possible = false;
	
	if(possible){
	//if possible, check if car wants to go left and move it using isMovingLeft. Also turn on left light
	if(leftRight == 0 && transform.position.z > -5.8f){
	nextPos = posZ - 4;
	isMovingLeft = true;
	lightLeft.SetActive(true);
	}
	if(leftRight == 1 && transform.position.z < 5.8f){
	//if possible, check if car wants to go right and move it using isMovingRight. Also turn on right light
	nextPos = posZ + 4;
	isMovingRight = true;	
	lightRight.SetActive(true);
	}
	}
	}
	
	void OnCollisionEnter(Collision other){
	if(other.gameObject.name != "road" && switchLine){
	//for extra safety, immediately stop car movement when it hits another car
	isMovingLeft = false;
	isMovingRight = false;
	//also turn its lights of again
	lightLeft.SetActive(false);
	lightRight.SetActive(false);
	}	
	}
}
