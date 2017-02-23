using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour {

	public int originalIndex;
	public string originalName;

	// Use this for initialization
	void Start () {
		originalIndex = gameObject.scene.buildIndex;
		originalName = gameObject.scene.name;
		DontDestroyOnLoad(gameObject);
		bool appendSelf = true;

		foreach (GameObject sControl in MainEngine.self.sceneControllerGOs)
		{
			if (sControl.GetComponent<SceneController>().originalIndex == originalIndex) // check if these represent the same level, aka check for redundancy
			{
				Destroy(gameObject);
				appendSelf = false;
			}
		}

		if(appendSelf == true)
		{
			MainEngine.self.sceneControllerGOs.Add(gameObject);
		}
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
