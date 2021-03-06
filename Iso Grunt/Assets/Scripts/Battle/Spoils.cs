﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Spoils : MonoBehaviour {

	public static Spoils self;

	public enum SPOILSSTATE {AddExp, AddCoins, RankUp, Wait, ReturnOWorld}

	public GameObject teleporterPrefab;
	public Text expEarnedLabel;
	public Text coinsEarnedLabel;

	public SPOILSSTATE? currentSpoilsState, postWaitSpoilsState;

	float basicWaitTime = 1.1f;

	// Use this for initialization
	void Start ()
	{
		self = this;
		currentSpoilsState = SPOILSSTATE.AddExp;
		Instantiate(MainEngine.self.pauseMenuPrefab).transform.SetParent(BattleEngine.self.battleCanvas.transform, false);
		transform.SetSiblingIndex(transform.parent.childCount-1);
		_setWait(SPOILSSTATE.AddExp, basicWaitTime);
	}
	
	// Update is called once per frame
	void Update ()
	{
		//Debug.Log(MainEngine.self.currentGameState);
		if(MainEngine.self.currentGameState == GAMESTATE.BattlePlay)
		{
			expEarnedLabel.text = "EXP Earned : " + BattleEngine.self.expEarned;
			coinsEarnedLabel.text = "Coins Earned : " + BattleEngine.self.coinsEarned;

			switch(currentSpoilsState)
			{
				case SPOILSSTATE.AddExp:
					_addExp();
					break;
				case SPOILSSTATE.AddCoins:
					_addCoins();
					break;
				case SPOILSSTATE.RankUp:
					break;
				case SPOILSSTATE.Wait:
					break;
				case SPOILSSTATE.ReturnOWorld:
					break;
			}
		}
		else if(MainEngine.self.currentGameState == GAMESTATE.EnterScene)
		{
			Destroy(gameObject);
		}
	}

	void _setWait(SPOILSSTATE givenNextState, float waitTime)
	{
		currentSpoilsState = SPOILSSTATE.Wait;
		postWaitSpoilsState = givenNextState;
		Invoke("_finishWait", waitTime);
	}

	void _finishWait()
	{
		currentSpoilsState = postWaitSpoilsState;
		postWaitSpoilsState = null;
	}

	void _addExp()
	{
		float waitTime = 1f / (BattleEngine.self.expEarned + 1);
		if(BattleEngine.self.expEarned > 0)
		{
			int iterator = 0;
			foreach(StatSheet liveSheet in MainEngine.self._getLiveParty())
			{
				
				if(liveSheet.hp > 0)
				{
					liveSheet.exp += 1;
					if(liveSheet.exp == liveSheet.maxExp)
					{
						liveSheet.level += 1;
						liveSheet.exp = 0;
						liveSheet.maxExp *= liveSheet.maxExpGrowth;
						currentSpoilsState = SPOILSSTATE.RankUp;
						liveSheet._rankUp();
						Pause.self.huds[iterator]._makeStatAdders();
					}
				}
				iterator++;
			}

			BattleEngine.self.expEarned -= 1;
			if(currentSpoilsState == SPOILSSTATE.AddExp)//only tally more EXP if not set to RankUp state
			{
				_setWait(SPOILSSTATE.AddExp, waitTime);
			}
		}
		else
		{
			_setWait(SPOILSSTATE.AddCoins, basicWaitTime);
		}
	}

	void _addCoins()
	{
		float waitTime = 1f / (BattleEngine.self.coinsEarned + 1);
		if(BattleEngine.self.coinsEarned > 0)
		{
			MainEngine.self.playerCoins += BattleEngine.self.coinsEarned;
			BattleEngine.self.coinsEarned -= 1;
			_setWait(SPOILSSTATE.AddCoins, waitTime);
		}
		else
		{
			MainEngine.self.fleeing = false;
			Debug.Log("return to overworld");
			MainEngine.self.playState = MainEngine.playSME.Roaming;
			CreativeSpore.RpgMapEditor.TeleporterBehaviour tPorter = Instantiate(teleporterPrefab).GetComponent<CreativeSpore.RpgMapEditor.TeleporterBehaviour>();
			tPorter.transform.position = MainEngine.self.transform.position;
			tPorter.TeleportOnEnter = true;
			tPorter.TargetSceneName = "OverWorld";
			tPorter.TargetTeleporterName = "";
			currentSpoilsState = SPOILSSTATE.ReturnOWorld;
			//MainEngine.self._initiateSceneChange(Engine.self.CurrentWorldSceneName, doorEnum.ReturnFromBattle);
		}
	}
}
