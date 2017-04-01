using UnityEngine;
using System.Collections;

public class ScrollTexture : MonoBehaviour {
	
	//not visible in the inspector
	public static float scrollSpeed;
	public static bool accelerate;
	
	//visible in the inspector
	public Material[] materials;
	
	//scrollspeed is 2 and start accelerating
	void Start(){
	scrollSpeed = 2;
	accelerate = false;
	StartCoroutine(Accelerate());
	}

	void Update(){
		
		//while accelerate is true and highspeed is not current speed, keep accelerating
		if(scrollSpeed < Manager.carHighSpeed && accelerate){
		scrollSpeed += Time.deltaTime;	
		}
		
		//for each material in the materials array, scroll it using offset
		foreach(Material material in materials){
		Vector2 offset = new Vector2(Time.time * scrollSpeed, material.mainTextureOffset.y);
		material.mainTextureOffset = offset;
		}
	}
	
	IEnumerator Accelerate(){
	yield return new WaitForSeconds(3);
	//wait 3 seconds and accelerate
	accelerate = true;
	}
}
