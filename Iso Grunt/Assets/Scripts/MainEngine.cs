using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public enum GAMESTATE{BeginGame, Dialogue, Paused, CutScene, OverWorldPlay, BattlePlay, EnterScene, ExitScene, Ending};
public enum KINGDOM{Animal, Monster, Machine};
public enum SPECIES{Rat, Bat, Boar, Falcon, Wolf, Pterodactyl, Bear, Zombie, Toaster};
public enum TARGETING{FirstEnemy, ChooseEnemy, Self, FirstAlly, ChooseAlly, AllEnemies, AllAllies, AllCharacters};

public class MainEngine : MonoBehaviour {

	public static MainEngine self;

	public GameObject transitionPrefab, pauseMenuPrefab, playerHudPrefab;

	public Material redSwapMat, blueSwapMat, yellowSwapMat, greenSwapMat, purpleSwapMat, darkGraySwapMat, whiteSwapMat, tanSwapMat;

	public CreativeSpore.RpgMapEditor.PlayerController pControl; // reference to the rpg Player

	public CreativeSpore.RpgMapEditor.PhysicCharBehaviour physCharBehav;

	public enum playSME {Transitioning, Roaming, Battling}

	public playSME playState = playSME.Roaming;

	public GAMESTATE currentGameState;

	public AudioSource audioSource;

	public AudioClip buzzClip;

	public string battleSceneName;

	public bool fleeing;
	public StatSheet mainSheet;
	public List<StatSheet> partySheets = new List<StatSheet> ();
	public SPECIES primaryEncounter;
	public List<SPECIES> encounterEnemies = new List<SPECIES> ();
	public List<Item> playerUsableItems = new List<Item>(), enemyUsableItems = new List<Item>();
	public List<GameObject> sceneControllerGOs = new List<GameObject>();
	public int minBattleEnemies, maxBattleEnemies, playerCoins;

	// Use this for initialization
	void Awake ()
	{
		self = this;
		if(partySheets.Count == 0)
		{
			mainSheet = new StatSheet("Main Guy", SPECIES.Rat);
			partySheets.Add(mainSheet);
			//partySheets.Add(new StatSheet("Second Guy", SPECIES.Rat));
			//partySheets.Add(new StatSheet("Third Guy", SPECIES.Rat));
			//partySheets.Add(new StatSheet("Fourth Guy", SPECIES.Rat));
		}
	}
	
	// Update is called once per frame
	void Update () {
	}

	void _lockPlayer()
	{
		pControl.enabled = false;
		physCharBehav.enabled = false;
	}

	void _unLockPlayer()
	{
		pControl.enabled = true;
		physCharBehav.enabled = true;
	}

	public void _setState(playSME givenState)
	{
		playState = givenState;
		switch(givenState)
		{
			case playSME.Transitioning:
				_lockPlayer();
				break;
			case playSME.Roaming:
				_unLockPlayer();
				break;
		}

	}

	public List<string> _battleItemsToOptions(bool showCosts)
	{
		List<string> odList = new List<string>();
		foreach(Item pBItem in playerUsableItems)
		{
			string itemText = pBItem.itemName + ", " + pBItem.amount;
			if(showCosts == true)
			{
				itemText += " : " + pBItem.purchaseValue + " Coins";
			}
			odList.Add(itemText);
		}
		return odList;
	}

	public void _addItem(Item givenItem)
	{
		foreach(Item itm in playerUsableItems)
		{
			if(itm.GetType() == givenItem.GetType())
			{
				itm.amount += givenItem.amount;
				return;
			}
		}

		playerUsableItems.Add(givenItem);
	}

	public bool _removeItem(Item givenItem, int removalAmount)//returns false when more trying to remove more of an item than you have
	{
		if(removalAmount > givenItem.amount)
		{
			return false;
		}

		givenItem.amount -= removalAmount;
		if(givenItem.amount == 0)
		{
			playerUsableItems.Remove(givenItem);
		}

		return true;
	}

	void OnEnable()
	{
		//Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
		SceneManager.sceneLoaded += _OnLevelFinishedLoading;
	}

	void OnDisable()
	{
		//Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
		SceneManager.sceneLoaded -= _OnLevelFinishedLoading;
	}

	void _OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
	{
		foreach (GameObject sControl in sceneControllerGOs)
		{
			sControl.SetActive(sControl.GetComponent<SceneController>().originalIndex == SceneManager.GetActiveScene().buildIndex);
		}
		_setState(MainEngine.playSME.Transitioning);
		GameObject transitionGO = Instantiate(transitionPrefab);
		transitionGO.GetComponent<Transition>().initUnFade();
	}

	public List<StatSheet> _getLiveParty()
	{
		List<StatSheet> liveMems = new List<StatSheet>();
		foreach(StatSheet pMem in partySheets)
		{
			if(pMem.hp > 0)
			{
				liveMems.Add(pMem);
			}
		}

		return liveMems;
	}
}
