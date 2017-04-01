using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour {
	
	//variables not visible in the inspector
	Vector3 camPos1;
	Vector3 camPos2 = new Vector3(8, 5, 0);
	
	bool pcControlsOnly;
	public bool keyboardControls;
	public string leftKey;
	public string rightKey;
	
	public static float carHighSpeed;

	public static float distance;
	public static float coins;
	
	public static bool count;
	public static bool gameOver;
	
	public static GameObject bestDistanceLabel;
	public static GameObject damageWarning;
	public static GameObject boostVignette;
	public static GameObject boostFlash;
	
	GameObject gameUI;
	GameObject distanceText;
	
	GameObject gameOverPanel;
	GameObject distanceObject;
	GameObject gameOverButtons;
	GameObject coinText;
	
	GameObject countDownText;
	GameObject bestDistanceText;
	
	GameObject optionsPanel;
	GameObject tiltIcon;
	GameObject touchIcon;
	
	public Slider volumeSlider;
	
	float camStartTime;
    float camJourneyLength;
	bool goToCamPos1;
	bool goToCamPos2;
	float countDown;
	
	void Start(){
	distance = 0;
	countDown = 3.5f;
	coins = 0;
	count = true;
	gameOver = false;
	
	//see if we should use mobile controls or pc controls only
	if(PlayerPrefs.GetInt("pcOnly") == 0){
	pcControlsOnly = false;	
	}
	else{
	pcControlsOnly = true;		
	}
	
	//set the first camera position of the two to the current camera position and find the distance between the 2 positions
	camPos1 = Camera.main.transform.position;
    camJourneyLength = Vector3.Distance(camPos1, camPos2);
	
	//find some objects and UI
	//------------------------------------------------------------------------------------------
	damageWarning = GameObject.Find("Damage warning");
	damageWarning.SetActive(false);
	
	AudioListener.volume = PlayerPrefs.GetFloat("volume");
	volumeSlider.value = PlayerPrefs.GetFloat("volume");
	
	gameUI = GameObject.Find("Game UI");
	boostVignette = GameObject.Find("boost vignette");
	boostFlash = GameObject.Find("boost flash");
	distanceText = GameObject.Find("Distance text");
	
	bestDistanceLabel = GameObject.Find("best distance label");
	bestDistanceLabel.SetActive(false);
	
	gameOverPanel = GameObject.Find("game over panel");
	distanceObject = GameObject.Find("Distance");
	gameOverButtons = GameObject.Find("buttons");
	coinText = GameObject.Find("coin text");
	
	gameOverPanel.SetActive(false);
	gameUI.SetActive(false);
	boostVignette.SetActive(false);
	boostFlash.SetActive(false);
	
	optionsPanel = GameObject.Find("options panel");
	tiltIcon = GameObject.Find("tilt icon");
	touchIcon = GameObject.Find("touch icon");
	
	optionsPanel.SetActive(false);
	
	countDownText = GameObject.Find("count down text");
	bestDistanceText = GameObject.Find("best"); 
	
	//------------------------------------------------------------------------------------------
	
	//set a new target distance based on your best distance
    float targetDistance = PlayerPrefs.GetFloat("bestDistance") + 0.01f;
	//show target distance
	bestDistanceText.GetComponent<UnityEngine.UI.Text>().text = "Target distance: " + targetDistance.ToString("f2") + " KM";
	//start count down
	StartCoroutine(CountDown());
	}
	
	void Update(){
	if(!gameOver){
	//if not game over, add distance based on car speed (scrollspeed)
	distance += ScrollTexture.scrollSpeed * 0.005f * Time.deltaTime;
	//show distance
	distanceText.GetComponent<UnityEngine.UI.Text>().text = distance.ToString("f2") + " KM";
	}
	else if(!optionsPanel.activeSelf){
	//if options panel is not active, show UI like distance and pause button
	gameUI.SetActive(false);
	gameOverPanel.SetActive(true);
	//show distance
	distanceObject.GetComponent<UnityEngine.UI.Text>().text = distance.ToString("f2") + " KM";
	
	//add coins with time.deltatime for the nice effect
	if(coins < distance * 200f){
	gameOverButtons.SetActive(false);
	coins += 200 * Time.deltaTime;
	}
	else{
	gameOverButtons.SetActive(true);	
	}
	//show coins
	coinText.GetComponent<UnityEngine.UI.Text>().text = "" + (int)coins;
	}
	
	if(count){
	//while counting, decrease number by time.deltatime and show it
	countDown -= Time.deltaTime;	
	if(countDown > 0.5f){
	countDownText.GetComponent<UnityEngine.UI.Text>().text = "" + countDown.ToString("f0");
	}
	else{
	//if 0.5 seconds is left, show 'race!' text
 	countDownText.GetComponent<UnityEngine.UI.Text>().text = "Race!";	
	}
	}
	
	//move camera to pos2 using lerp
	if(!gameOver && goToCamPos2){
	float distCovered = (Time.time - camStartTime) * 10;
    float fracJourney = distCovered / camJourneyLength;
    Camera.main.transform.position = Vector3.Lerp(camPos1, camPos2, fracJourney);	
	}
	//move camera to pos1 using lerp
	if(!gameOver && goToCamPos1){
	float distCovered = (Time.time - camStartTime) * 10;
    float fracJourney = distCovered / camJourneyLength;
    Camera.main.transform.position = Vector3.Lerp(camPos2, camPos1, fracJourney);		
	}
	}
	
	//delete all playerprefs
	public void DeletePlayerPrefs(){
	PlayerPrefs.DeleteAll();
	PlayerPrefs.SetFloat("volume", volumeSlider.value);	
	}
	
	public void openOptionsPanel(){
	if(gameOver){
	//if player is game over and opens options panel, open it and set other menu's false
	gameUI.SetActive(false);
	gameOverPanel.SetActive(false);
	optionsPanel.SetActive(true);	
	}
	if(!gameOver){
	//if player is not game over yet, set the timescale to 0 to freeze the game
	Time.timeScale = 0;
	gameUI.SetActive(false);
	gameOverPanel.SetActive(false);
	optionsPanel.SetActive(true);	
	}
	
	//show right button according to control settings
	if(!keyboardControls){
		if(PlayerPrefs.GetInt("touchControls") == 1){
		touchIcon.SetActive(false);
		tiltIcon.SetActive(true);	
		}
		if(PlayerPrefs.GetInt("touchControls") == 0){
		tiltIcon.SetActive(false);
		touchIcon.SetActive(true);	
		}
	}
	else{
		tiltIcon.SetActive(false);
		touchIcon.SetActive(false);
	}
	}
	
	//set the touch icon visible and use touch controls
	public void useTouchControls(){
	tiltIcon.SetActive(false);
	touchIcon.SetActive(true);	
	PlayerPrefs.SetInt("touchControls", 0);
	}
	
	//set the tilt icon visible and use tilt controls
	public void useTiltControls(){
	if(!pcControlsOnly){
	touchIcon.SetActive(false);
	tiltIcon.SetActive(true);
	PlayerPrefs.SetInt("touchControls", 1);
	}
	}
	
	//load current scene
	public void restart(){
	SceneManager.LoadScene(SceneManager.GetActiveScene().name);	
	}
	
	//open garage scene
	public void garage(){
	SceneManager.LoadScene("Garage");	
	}
	
	//close options panel and if not game over, set timescale to 1 again
	public void closeOptionsPanel(){
	if(gameOver){
	optionsPanel.SetActive(false);	
	}
	if(!gameOver){
	optionsPanel.SetActive(false);	
	gameUI.SetActive(true);
	Time.timeScale = 1;
	}
	}
	
	public void changeCameraPosition(){
	//if camera position is pos1, go to pos2
	if(Camera.main.transform.position == camPos1){
	camStartTime = Time.time;
	goToCamPos1 = false;
	goToCamPos2 = true;
	}
	else{
	//go to pos 1
	camStartTime = Time.time;
	goToCamPos2 = false;
	goToCamPos1 = true;	
	}
	}
	
	public void SetVolume(){
	//set the volume settings to slider volume and save volume
	AudioListener.volume = volumeSlider.value;
	PlayerPrefs.SetFloat("volume", volumeSlider.value);
	}
	
	IEnumerator CountDown(){
	//Wait while counting down and than set game UI active
	yield return new WaitForSeconds(4f);	
	count = false;
	countDownText.SetActive(false);
	gameUI.SetActive(true);
	}
}
