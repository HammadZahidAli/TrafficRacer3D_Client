using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
//car variables
public class Car{
	public GameObject carPrefab;
	public string carName;
	public int carPrice;
}

public class garage : MonoBehaviour {
	
	//variables visible in the inspector
	public Slider speedSlider;
	public List<Car> cars;
	
	//variables not visible in the inspector
	public static GameObject startPanel;
	public static GameObject startPanelContent;
	
	GameObject loading;
	GameObject currentCar;
	GameObject leftButton;
	GameObject rightButton;
	GameObject carName;
	GameObject carLock;
	GameObject carPrice;
	GameObject coinsLabel;
	GameObject locationPanel;
	
	bool fading;
	
	void Start(){	
	//always unlock the first car
	PlayerPrefs.SetInt("" + cars[0].carName, 1);
	
	//find all the UI objects
	//--------------------------------------------------------------------------------------------------------
	carLock = GameObject.Find("locked car panel");
	carPrice = GameObject.Find("car price");
	
	//set the price of the price label to the price of the visible car
	carPrice.GetComponent<Text>().text = "" + cars[PlayerPrefs.GetInt("selectedCar")].carPrice;
	
	startPanel = GameObject.Find("Start panel");	
	startPanel.SetActive(true);
	
	startPanelContent = GameObject.Find("start panel content");
	startPanelContent.SetActive(true);
	
	locationPanel = GameObject.Find("location panel");
	locationPanel.SetActive(false);
	
	coinsLabel = GameObject.Find("coins");	
	//set the coins text to your total coins
	coinsLabel.GetComponent<Text>().text = "" + PlayerPrefs.GetInt("coins");
	
	loading = GameObject.Find("loading");	
	loading.SetActive(false);
	
	leftButton = GameObject.Find("left");
	rightButton = GameObject.Find("right");
	
	//instantiate the selected car and make it a child of the rotating platform
	currentCar = Instantiate(cars[PlayerPrefs.GetInt("selectedCar")].carPrefab, Vector3.zero, transform.rotation) as GameObject;
	currentCar.transform.parent = GameObject.Find("Platform").transform;
	
	carName = GameObject.Find("car name");
	//set the car name to the name of the visible car
	carName.GetComponent<Text>().text = "" + cars[PlayerPrefs.GetInt("selectedCar")].carName;
	
	//--------------------------------------------------------------------------------------------------------
	
	//set the slider to the speed of the currently selected car by getting the highspeed from its car controls script
	speedSlider.value = cars[PlayerPrefs.GetInt("selectedCar")].carPrefab.GetComponent<CarControls>().highSpeed;
	
	//see if selected car has been unlocked and if not, set the lock active and set its price
	if(PlayerPrefs.GetInt("" + cars[PlayerPrefs.GetInt("selectedCar")].carName) == 0){
	carLock.SetActive(true);
	carPrice.GetComponent<Text>().text = "" + cars[PlayerPrefs.GetInt("selectedCar")].carPrice;
	}
	else{
	carLock.SetActive(false);	
	}
	}
	
	void Update(){
	//set the buttons that switch cars false if the last car of the array is visible
	if(PlayerPrefs.GetInt("selectedCar") < 1){
	leftButton.SetActive(false);	
	}
	else{
	leftButton.SetActive(true);	
	}
	
	if(PlayerPrefs.GetInt("selectedCar") == cars.Count - 1){
	rightButton.SetActive(false);	
	}
	else{
	rightButton.SetActive(true);	
	}
	//if start panel is fading out and not done yet, decrease its alpha by time.deltatime
	if(fading && startPanel.GetComponent<Image>().color.a > 0){
	var color = new Color(
	startPanel.GetComponent<Image>().color.r,
	startPanel.GetComponent<Image>().color.g,
	startPanel.GetComponent<Image>().color.b,
	startPanel.GetComponent<Image>().color.a);
	color.a -= Time.deltaTime;
	startPanel.GetComponent<Image>().color = color;	
	}
	else if(fading){
	//if fading is true but the fading is done already, set fading false and remove the start panel completely
	fading = false;	
	startPanel.SetActive(false);
	}
	}
	
	//start fading out and set start panel not active when player presses play
	public void play(){
	startPanelContent.SetActive(false);
	fading = true;	
	}
	
	//quit application
	public void quit(){
	Application.Quit();	
	}
	
	//instantiate next car of the array
	//save it as selected car
	//make it child of platform
	public void right(){
	PlayerPrefs.SetInt("selectedCar", PlayerPrefs.GetInt("selectedCar") + 1);	
	Destroy(currentCar);
	currentCar = Instantiate(cars[PlayerPrefs.GetInt("selectedCar")].carPrefab, Vector3.zero, transform.rotation) as GameObject;
	currentCar.transform.parent = GameObject.Find("Platform").transform;
	
	//if current car is not unlocked, set lock true. Else, set lock false
	if(PlayerPrefs.GetInt("" + cars[PlayerPrefs.GetInt("selectedCar")].carName) == 0){
	carLock.SetActive(true);
	carPrice.GetComponent<Text>().text = "" + cars[PlayerPrefs.GetInt("selectedCar")].carPrice;
	}
	else{
	carLock.SetActive(false);	
	}
	
	//set new car name of the instantiated car
	carName.GetComponent<Text>().text = "" + cars[PlayerPrefs.GetInt("selectedCar")].carName;
	//set speed of the instantiated car for the slider
	speedSlider.value = cars[PlayerPrefs.GetInt("selectedCar")].carPrefab.GetComponent<CarControls>().highSpeed;
	}
	
	//instantiate previous car of the array
	//save it as selected car
	//make it child of platform
	public void left(){
	PlayerPrefs.SetInt("selectedCar", PlayerPrefs.GetInt("selectedCar") - 1);	
	Destroy(currentCar);
	currentCar = Instantiate(cars[PlayerPrefs.GetInt("selectedCar")].carPrefab, Vector3.zero, transform.rotation) as GameObject;
	currentCar.transform.parent = GameObject.Find("Platform").transform;	
	
	//if current car is not unlocked, set lock true. Else, set lock false
	if(PlayerPrefs.GetInt("" + cars[PlayerPrefs.GetInt("selectedCar")].carName) == 0){
	carLock.SetActive(true);
	carPrice.GetComponent<Text>().text = "" + cars[PlayerPrefs.GetInt("selectedCar")].carPrice;
	}
	else{
	carLock.SetActive(false);	
	}
	//set new car name of the instantiated car
	carName.GetComponent<Text>().text = "" + cars[PlayerPrefs.GetInt("selectedCar")].carName;
	//set speed of the instantiated car for the slider
	speedSlider.value = cars[PlayerPrefs.GetInt("selectedCar")].carPrefab.GetComponent<CarControls>().highSpeed;
	}
	
	public void buyCar(){
	//check if player has enough coins to buy current car, remove coins and set coins text to new amount of coins
	if(cars[PlayerPrefs.GetInt("selectedCar")].carPrice <= PlayerPrefs.GetInt("coins")){
	PlayerPrefs.SetInt("coins", PlayerPrefs.GetInt("coins") - cars[PlayerPrefs.GetInt("selectedCar")].carPrice);
	coinsLabel.GetComponent<Text>().text = "" + PlayerPrefs.GetInt("coins");
	//save the car as unlocked
	PlayerPrefs.SetInt("" + cars[PlayerPrefs.GetInt("selectedCar")].carName, 1);
	//remove the lock to make the car drivable
	carLock.SetActive(false);
	}
	}
	
	public void race(){
	//show panel to choose your location
	locationPanel.SetActive(true);	
	}
	
	public void back(){
	//go back to the garage
	locationPanel.SetActive(false);	
	}
	
	public void desert(){
	//open desert scene
	loading.SetActive(true);
	SceneManager.LoadScene("Desert");	
	}
	
	public void village(){
	//open village scene
	loading.SetActive(true);
	SceneManager.LoadScene("Village");	
	}
}
