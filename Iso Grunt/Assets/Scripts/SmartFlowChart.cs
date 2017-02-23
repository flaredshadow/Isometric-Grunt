using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class SmartFlowChart : MonoBehaviour {

	public Flowchart flowchart;

	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void setName()
	{
		MainEngine.self.mainSheet.setName(flowchart.GetStringVariable("PlayerName"));
	}

	public void triggerBattle(CreativeSpore.RpgMapEditor.TeleporterBehaviour givenTeleporter)
	{
		givenTeleporter.transform.position = MainEngine.self.transform.position;

		MainEngine.self._setState(MainEngine.playSME.Transitioning);
		GameObject transitionGO = Instantiate(MainEngine.self.transitionPrefab);
		transitionGO.GetComponent<Transition>().initFadeToBlack(givenTeleporter, MainEngine.self.gameObject);
		OverEnemyBehaviour enemyScript = givenTeleporter.GetComponentInParent<OverEnemyBehaviour>();
		if(enemyScript != null)
		{
			MainEngine.self.primaryEncounter = enemyScript.firstSPECIES;
		}
	}
}
