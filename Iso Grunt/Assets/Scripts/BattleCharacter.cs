using UnityEngine;
using System.Collections;

public class BattleCharacter : MonoBehaviour {

	public static float elevationSpacing = 3.0f;

	public StatSheet sheet;
	public float elevation;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void _setSheet(StatSheet givenSheet)
	{
		sheet = givenSheet;
		elevation = sheet.flightHeight * elevationSpacing;

		//need to set sprite to match species
	}
}
