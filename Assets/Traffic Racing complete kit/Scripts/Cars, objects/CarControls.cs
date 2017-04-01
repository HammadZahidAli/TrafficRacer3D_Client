using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.SceneManagement;

public class CarControls : MonoBehaviour {
	
	//variables visible in the inspector
	[Range(0.5f, 2.5f)]
	public float movespeed;
	[Range(1, 10)]
	public float moveRange;
	
	[Range(5, 15)]
	public int highSpeed;
	public int boostSpeed;
	public float boostLength;
	public ParticleSystem sparks;
	public ParticleSystem boostParticles;
	public float turnRotation;
	public bool vibrateOnCollision;
	public bool keyBoardControls;
	public string leftKey;
	public string rightKey;
	
	//variable not visible in the inspector
	bool crash;
	int collisionCount;
	GameObject smoke;
	GameObject carMesh;
	AudioSource carAudioNormal;
	AudioSource carAudioBoost;
	Coroutine boostCoroutine;
	
	Manager manager;
	
	void Awake(){
	//set the overall speed to the speed of this car
	Manager.carHighSpeed = highSpeed;	
	}
	
	void Start(){
	//find the smoke particles and set them not active
	smoke = transform.Find("smoke").gameObject;	
	smoke.SetActive(false);	
	boostParticles.Stop();	
	
	carMesh = transform.Find("car mesh").gameObject;
	manager = FindObjectOfType<Manager>();
	
	//get car audio
	carAudioNormal = GetComponents<AudioSource>()[0];
	carAudioBoost = GetComponents<AudioSource>()[1];
	}

	void Update(){
	//check if car is not in garage
	if(SceneManager.GetActiveScene().name != "Garage"){
	//if touch controls are enabled, check for mouse button 0 (touch on mobile devices) and make sure you're not clicking UI
	if(PlayerPrefs.GetInt("touchControls") == 0 && !manager.keyboardControls){
	if(Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject()){
	//check car position and clicked position on the screen to move the car to the right side
    if(Input.mousePosition.x < Screen.width/2 && transform.position.z > -moveRange){
    transform.Translate(0, 0, movespeed * Time.deltaTime * -5);
	carMesh.transform.rotation = Quaternion.AngleAxis(180 - turnRotation, Vector3.up);
    }
    else if(Input.mousePosition.x > Screen.width/2 && transform.position.z < moveRange){
    transform.Translate(0, 0, movespeed * Time.deltaTime * 5);
	carMesh.transform.rotation = Quaternion.AngleAxis(180 + turnRotation, Vector3.up);
    } 
	}
	else{
	carMesh.transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
	}
	}
	//if you don't want to use touch controls, accelerometer controls are automatically enabled:
	else if(!manager.keyboardControls){
	//check car position and move it using accelerometer
	if(transform.position.z < moveRange && transform.position.z > -moveRange && ScrollTexture.accelerate){
	transform.Translate(0, 0, Input.acceleration.x * movespeed * Time.deltaTime * 15);	
	}
	
	//prevent car from going off the road
	if(transform.position.z >= moveRange){
	transform.position = new Vector3(transform.position.x, transform.position.y, moveRange - 0.001f);	
	}
	//prevent car from going off the road
	if(transform.position.z <= -moveRange){
	transform.position = new Vector3(transform.position.x, transform.position.y, -moveRange + 0.001f);		
	}
	}
	else{
		if(Input.GetKey(manager.leftKey) && transform.position.z > -moveRange){
			transform.Translate(0, 0, -movespeed * Time.deltaTime * 3);
			carMesh.transform.rotation = Quaternion.AngleAxis(180 - turnRotation, Vector3.up);
		}
		if(Input.GetKey(manager.rightKey) && transform.position.z < moveRange){
			transform.Translate(0, 0, movespeed * Time.deltaTime * 3);
			carMesh.transform.rotation = Quaternion.AngleAxis(180 + turnRotation, Vector3.up);
		}
		if((!Input.GetKey(manager.rightKey) && !Input.GetKey(manager.leftKey)) || transform.position.z >= moveRange || transform.position.z <= -moveRange){
			carMesh.transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
		}
	}
	}
	
	//while counting down in the beginning of the game, move car forward (just for a nice effect)
	if(Manager.count && transform.position.x > 2f){
	transform.Translate(-0.1f, 0, 0);	
	}
	
	//if car has crashed, move it backwards
	if(crash){
	transform.Translate(0.3f, 0, 0);	
	}
	}
	
	//on collision with car
	void OnCollisionEnter(Collision col){
	if(col.gameObject.name != "road" && col.gameObject.name != "boost(Clone)"){
		
	if(vibrateOnCollision && PlayerPrefs.GetInt("pcOnly") == 0 && !manager.keyboardControls){
		Handheld.Vibrate();
	}
		
	//get audio, give it random duration and volume, and play it
	AudioSource audio = GetComponents<AudioSource>()[2];
	audio.pitch = Random.Range(1f, 3f);
	audio.volume = Random.Range(0.5f, 1f);
    audio.Play();	
	//get first contact point of the collision, instantiate sparks effect and move it to the contact point
	ContactPoint contact = col.contacts[0];
    ParticleSystem sparksEffect = Instantiate(sparks);
	sparksEffect.transform.position = contact.point;
	
	//add 1 to the collisions
	collisionCount++;
	//after 5 collisions, turn on smoke effect and give a text warning
	if(collisionCount == 5){
	smoke.SetActive(true);	
	StartCoroutine(warning());
	}
	//after more than 5 collisions, car crashes
	if(collisionCount > 5){
	StartCoroutine(Crash());	
	}
	}
	else if(col.gameObject.name == "boost(Clone)"){
	Destroy(col.gameObject);
	if(boostCoroutine != null)
    StopCoroutine(boostCoroutine);
    boostCoroutine = StartCoroutine(boost());	
	}
	}
	
	void OnTriggerEnter(Collider other){
	if(other.gameObject.name != "boost(Clone)"){
	//play audio and car crashes
	AudioSource audio = GetComponents<AudioSource>()[2];
    audio.Play();
	StartCoroutine(Crash());	
	}
	}
	
	IEnumerator boost(){
	carAudioNormal.Stop();
	carAudioBoost.Play();
	Manager.boostFlash.SetActive(true);
	yield return new WaitForSeconds(0.05f);
	Manager.boostFlash.SetActive(false);
	Manager.boostVignette.SetActive(true);
	boostParticles.Play();
	ScrollTexture.scrollSpeed = Manager.carHighSpeed + boostSpeed;	
	yield return new WaitForSeconds(boostLength);
	ScrollTexture.scrollSpeed = Manager.carHighSpeed;
	Manager.boostVignette.SetActive(false);
	boostParticles.Stop();
	carAudioBoost.Stop();
	carAudioNormal.Play();
	}
	
	IEnumerator Crash(){
	//crash is true
	crash = true;
	//wait a moment
	yield return new WaitForSeconds(1);
	//set new coins to distance * 200
	var newCoins = Manager.distance * 200f;
	//save extra coins and than set the player to be game over
	PlayerPrefs.SetInt("coins", PlayerPrefs.GetInt("coins") + (int)newCoins);
	Manager.gameOver = true;
	
	//save distance if it's better than current best and set label active
	if(Manager.distance > PlayerPrefs.GetFloat("bestDistance")){
	PlayerPrefs.SetFloat("bestDistance", Manager.distance);
	Manager.bestDistanceLabel.SetActive(true);
	}
	//destroy car
	Destroy(gameObject);
	}
	
	IEnumerator warning(){
	//set warning text active, wait some seconds and set it not active
	Manager.damageWarning.SetActive(true);
	yield return new WaitForSeconds(2);
	Manager.damageWarning.SetActive(false);
	}
}
