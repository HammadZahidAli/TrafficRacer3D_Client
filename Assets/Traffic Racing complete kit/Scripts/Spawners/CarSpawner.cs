using UnityEngine;
using System.Collections;

public class CarSpawner : MonoBehaviour {
	
	//variables visible in the inspector
	[Header("Spawn settings")]
	public bool leftSameDirection;
	public bool rightSameDirection;
	
	[Space(5)]
	public float startSpawnWait;
	[Space(5)]
	public int maxCarsPerWave;
	public int minCarsPerWave;
	[Space(5)]
	public float maxCarSpawnWait;
	public float minCarSpawnWait;
	[Space(5)]
	public float maxCarWaveWait;
	public float minCarWaveWait;
	
	[Header("Spawn positions")]
	public Vector3 carSpawnPos1;
	public Vector3 carSpawnPos2;
	public Vector3 carSpawnPos3;
	public Vector3 carSpawnPos4;
	
	[Header("Add cars here:")]
	public GameObject[] cars;
	
	//variables not visible in the inspector
	int carsPerWave;
	float carSpawnWait;
	float carWaveWait;
	Vector3 randomPos;
	Vector3 lastSpawnPos;
	
	void Start(){
	//start spawning cars
	StartCoroutine(CarSpawning());
	}

	//IEnumerator to randomize car waves
	IEnumerator CarSpawning(){
		//wait a moment at the beginning of the game
		yield return new WaitForSeconds(startSpawnWait);
        while(true){
			//while wave has not ended, keep spawning cars and wait a moment between spawning new ones
            for(int i = 0; i < carsPerWave; i++){
				SpawnNewCar();
				//time between spawning new cars is based on scrollspeed (car speed) to have the same amount of cars with differend speeds
				carSpawnWait = Random.Range(minCarSpawnWait, maxCarSpawnWait) * (1/ScrollTexture.scrollSpeed) * 10f;
                yield return new WaitForSeconds(carSpawnWait);
            }
			//after the wave, change the amount of cars and wave wait for next wave to randomize them
			carsPerWave = Random.Range(minCarsPerWave, maxCarsPerWave);
			carWaveWait = Random.Range(minCarWaveWait, maxCarWaveWait);
			
			//wait some time before starting next wave
            yield return new WaitForSeconds(carWaveWait);
        }
    }
	
	void SpawnNewCar(){
	//check if there are cars, choose a random one, choose a position and instantiate it
	if(cars.Length > 0){
	var randomCar = Random.Range(0, cars.Length);
	RandomPosition();
	//make sure car is not spawned at the same position as last car
	if(lastSpawnPos != randomPos){
	var newCar = Instantiate(cars[randomCar]);
	newCar.transform.position = randomPos;
	lastSpawnPos = randomPos;
	
	if((randomPos.z < 0 && !leftSameDirection) || (randomPos.z > 0 && !rightSameDirection))
		newCar.GetComponent<MoveObject>().sameDirection = false;
	}
	else{
	SpawnNewCar();	
	}
	}	
	}
	
	void RandomPosition(){
	//using random int to choose spawnposition
	var randomPosNumber = Random.Range(0, 4);
	if(randomPosNumber == 0){
	randomPos = carSpawnPos1;	
	}
	if(randomPosNumber == 1){
	randomPos = carSpawnPos2;	
	}
	if(randomPosNumber == 2){
	randomPos = carSpawnPos3;	
	}
	if(randomPosNumber == 3){
	randomPos = carSpawnPos4;	
	}	
	}
}
