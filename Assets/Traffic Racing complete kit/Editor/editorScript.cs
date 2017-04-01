using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

//change inspector look of multiple objects
[CanEditMultipleObjects]
[CustomEditor(typeof(Manager))]
public class editorScript : Editor{
	
	//variables not visible in the inspector
	Manager managerScript;
	bool pcOnly;
	
	void OnEnable(){
	//get target script and volume slider
	managerScript = (Manager)target;
	var sliderObject = GameObject.Find("Volume slider");
	managerScript.volumeSlider = sliderObject.GetComponent<Slider>();
	
	//check playerprefs to know if you want pc controls only
	if(PlayerPrefs.GetInt("pcOnly") == 0){
	pcOnly = false;	
	}
	else{
	pcOnly = true;	
	}
	}
	
	public override void OnInspectorGUI(){	
	//some space in the inspector
	GUILayout.Space(10);
	
	managerScript.keyboardControls = EditorGUILayout.ToggleLeft("Keyboard", managerScript.keyboardControls);	
	
	GUILayout.Space(3);
	
	GUI.color = new Color(0.7f, 0.7f, 0.7f, 0.3f);
	if(!managerScript.keyboardControls){
	//begin horizontal and add the toggle and save button
	GUILayout.BeginHorizontal("Box");
	GUI.color = Color.white;
	pcOnly = EditorGUILayout.ToggleLeft("Disable accelerometer controls (pc)", pcOnly);	
	GUI.color = new Color(1, 1, 1, 0.8f);
	if(GUILayout.Button("Save")){
	if(pcOnly == false){
	Debug.Log("Accelerometer controls enabled");
	PlayerPrefs.SetInt("pcOnly", 0);
	}
	else{
	Debug.Log("Accelerometer controls disabled");
	PlayerPrefs.SetInt("pcOnly", 1);	
	}
	}
	//end horizontal
	GUILayout.EndHorizontal();
	}
	else{
		GUI.color = new Color(0.7f, 0.7f, 0.7f, 0.3f);
		GUILayout.BeginVertical("Box");
		GUI.color = Color.white;
		managerScript.leftKey = EditorGUILayout.TextField("left key", managerScript.leftKey);
		managerScript.rightKey = EditorGUILayout.TextField("right key", managerScript.rightKey);
		GUILayout.EndVertical();
	}
	
	//give the button a new color
	GUI.color = new Color(0.7f, 0.7f, 0.7f, 1);
	//draw delete playerprefs button
	if(GUILayout.Button("Delete PlayerPrefs data", GUILayout.Height(30))){
		if(EditorUtility.DisplayDialog("Delete PlayerPrefs", "Sure to delete PlayerPrefs data?", "YES", "NO")) {
			managerScript.DeletePlayerPrefs();
			pcOnly = false;
			Debug.LogWarning("PlayerPrefs deleted -> please check your accelerometer settings");
		}
	}
	
	//apply modifications
	serializedObject.ApplyModifiedProperties(); 
	//undo funtionality
	Undo.RecordObject(managerScript, "change in manager");
	
	}
}
