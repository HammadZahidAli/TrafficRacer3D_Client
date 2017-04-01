using UnityEngine;
using System.Collections;

public class EnvironmentSpawner : MonoBehaviour {
	
	//variables visible in the inspector
	[Header("Spawn settings")]
	public int minObjectsPerWave;
	public int maxObjectsPerWave;
	[Space(5)]
	public float minSpawnWait;
	public float maxSpawnWait;
	[Space(5)]
	public float minWaveWait;
	public float maxWaveWait;
	
	[Header("Spawn positions")]
	public Vector3 spawnPos1;
    public Vector3 spawnPos2;
	[Space(5)]
	public int randomSpawnRange;
	
	[Header("Add roadside objects here:")]
	public GameObject[] objects;
	public GameObject bridge;
	
	//variables not visible in the inspector
    Vector3 randomPos;
	Vector3 spawnPos;
	
	int objectsPerWave;
	float spawnWait;
	float waveWait;
	
    IEnumerator Start(){
		//loop
        while(true){
            for(int i = 0; i < objectsPerWave; i++){
				//while wave has not ended, keep spawning new objects
				int random = Random.Range(0, 50);
				//if random number is 0, spawn a bridge, else spawn object
				if(random == 0){
				Instantiate(bridge);	
				bridge.transform.position = new Vector3(-60, 0, 0);
				}
				else{
				Spawn();	
				}
				//time between spawning new objects is based on scrollspeed (car speed) to have the same amount of objects with differend speeds
				spawnWait = Random.Range(minSpawnWait, maxSpawnWait) * (1/ScrollTexture.scrollSpeed) * 10f;
                yield return new WaitForSeconds(spawnWait);
            }
			//after the wave, change amount of objects and wave wait for next wave to randomize them
			objectsPerWave = Random.Range(minObjectsPerWave, maxObjectsPerWave);
			waveWait = Random.Range(minWaveWait, maxWaveWait);
			
			//wait some time before starting next wave
            yield return new WaitForSeconds(waveWait);
        }
    }
	
	void Spawn(){
	//choose random position
	int randomPosNumber = Random.Range(0, 2);  
	if(randomPosNumber == 0){
	randomPos = spawnPos1;	
	}
	else{
	randomPos = spawnPos2;	
	}
	
	//randomize the position
	int spawnRandomness = Random.Range(-randomSpawnRange, randomSpawnRange);
	spawnPos = new Vector3(randomPos.x, randomPos.y, randomPos.z + spawnRandomness);	
	
	//choose an object from the array
	int randomObject = Random.Range(0, objects.Length);
	
	//instantiate the object and set its position
	var newObject = Instantiate(objects[randomObject]);
	newObject.transform.position = spawnPos;
	
	//turn houses on the left 180° to rotate them towards the road
	if(randomPosNumber == 1 && newObject.name != "tree(Clone)" && newObject.name != "cactus(Clone)" && newObject.name != "cactus 1(Clone)"){
	newObject.transform.Rotate(Vector3.up * 180);	
	}
	}
}
