using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleEngine : MonoBehaviour {

	public static BattleEngine self;

	public GameObject bCharPrefab;
	public Vector3 arenaCenter;
	public float charSpacing;

	List<BattleCharacter> playerCharacters = new List<BattleCharacter> ();
	List<BattleCharacter> enemyCharacters = new List<BattleCharacter> ();

	// Use this for initialization
	void Start ()
	{
		self = this;

		foreach(StatSheet liveSheet in MainEngine.self._getLiveParty())
		{
			GameObject bcGO = Instantiate(bCharPrefab);
			BattleCharacter bcScript = bcGO.GetComponent<BattleCharacter>();
			bcScript._setSheet(liveSheet);
			playerCharacters.Add(bcScript);
			bcGO.transform.position = _getLinePos(bcScript);
		}
		int totalEnemies = Random.Range(MainEngine.self.minBattleEnemies, MainEngine.self.maxBattleEnemies+1);
		for(int i = 0; i < totalEnemies; i++)
		{
			GameObject bcGO = Instantiate(bCharPrefab);
			BattleCharacter bcScript = bcGO.GetComponent<BattleCharacter>();
			if(i == 0) // first enemy is always the overworld enemy
			{
				bcScript._setSheet(new StatSheet("Enemy "+i, MainEngine.self.primaryEncounter));
			}
			else
			{
				SPECIES randomEnemy = MainEngine.self.encounterEnemies[Random.Range(0, MainEngine.self.encounterEnemies.Count)];
				bcScript._setSheet(new StatSheet("Enemy "+i, randomEnemy));
			}
			enemyCharacters.Add(bcScript);
			bcGO.transform.position = _getLinePos(bcScript);
			bcGO.transform.Rotate(0, 180, 0);
		}


	}
	
	// Update is called once per frame
	void Update () {
	
	}

	Vector3 _getLinePos(BattleCharacter givenChar)
	{
		Vector3 spaceFromCenter;

		if(playerCharacters.Contains(givenChar))
		{
			spaceFromCenter = new Vector3((playerCharacters.IndexOf(givenChar) + 1) * charSpacing, givenChar.elevation, 0);
			return arenaCenter - spaceFromCenter;
		}
		else
		{
			if(!enemyCharacters.Contains(givenChar))
			{
				Debug.Log("Character not found in either list");
			}

			spaceFromCenter = new Vector3((enemyCharacters.IndexOf(givenChar) + 1) * charSpacing, givenChar.elevation, 0);
			return arenaCenter + spaceFromCenter;
		}
	}

	public void _squirmingClaws()
	{
		
	}

	public void _plagueBite()
	{
		
	}

	public void _sewerStench()
	{

	}

	public void _piedPiper()
	{

	}

	public void _swoop()
	{

	}

	public void _scentOfBlood()
	{

	}

	public void _echoScreech()
	{

	}

	public void _nightFlight()
	{

	}

	public void _tuskFling()
	{
		
	}

	public void _bodySlam()
	{

	}

	public void _mudCannonBall()
	{

	}

	public void _threeLittlePigs()
	{

	}

	public void _talonDrop()
	{

	}

	public void _flee()
	{
		
	}

	public void _poisonTest()
	{

	}

	public void _healTest()
	{

	}
}
