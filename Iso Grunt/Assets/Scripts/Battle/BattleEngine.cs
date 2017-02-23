using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class BattleEngine : MonoBehaviour {

	public static BattleEngine self;

	public GameObject bCharPrefab, buttonPrefab, dropDownPrefab, explosionPrefab, tombStonePrefab, spoilsPrefab, damagePrefab,
	rapidCommandPrefab, pressCommandPrefab, precisionCommandPrefab, chargeCommandPrefab, pipeCommandPrefab,
	pipeRatPrefab,
	statusEffectPrefab, dizzyStarsPrefab;

	public Canvas battleCanvas;
	public AudioSource bEngineAudio;
	public AudioClip buzzClip;

	public Sprite poisonIcon, paralysisIcon, swordIcon, shieldIcon;

	public enum BATTLESTATE
	{
		InitPlayerDecide, PlayerDecide, InitPlayerAttack, PlayerAttack, EnemyDecide, EnemyAttack, PlayerWin, PlayerLose, Flee, InitKill, TombStoneWait, AdjustLineUp, Wait,
		ResolveStatusEffects
	}

	public enum ATTACKSTATE
	{
		InitAttack, MovePreAction, ActionState,  ApplyAttack, HandleFail, MovePostAction
	}

	public BATTLESTATE? currentBState, postWaitBState, delayedBState;
	public ATTACKSTATE? currentAState, postWaitAState, delayedAState;
	public Vector3 arenaCenter;
	public float charSpacing, walkSpeed = 1f, standardWaitTime = .5f;
	public int statusEffectsResolved, commandsDestroyed, attackSubState = 0, bonus = 0, expEarned = 0, coinsEarned = 0;
	public bool preGotNextCharInLine;
	public BattleCharacter actingBC;
	public List<BattleCharacter> playerCharacters = new List<BattleCharacter> (), enemyCharacters = new List<BattleCharacter> (), targetFriendlies = new List<BattleCharacter> (), targOpposed = new List<BattleCharacter> ();

	Attack activeAttack;

	Item itemToBeUsed;

	System.Action invokeAction;

	Vector3 mainDDPosition = new Vector3 (0, 120, 0), buttonOffsetPosition = new Vector3 (0, 40, 0), commandPosition = new Vector3 (0, 120, 0),
	ddOffsetPosition = new Vector3 (200, 0, 0), choiceButtonOffset = new Vector3 (0, .5f, 0), fallVelocity = Vector3.down * 20f;
	// ^ to be used as sideways adjustment for Spells, Items, and Fleeing dropdowns

	List<Item> itemsEarned = new List<Item> ();

	// Use this for initialization
	void Start ()
	{
		self = this;
		MainEngine.self.currentGameState = GAMESTATE.BattlePlay;
		foreach(StatSheet liveSheet in MainEngine.self._getLiveParty())
		{
			GameObject bcGO = Instantiate(bCharPrefab);
			BattleCharacter bcScript = bcGO.GetComponent<BattleCharacter>();
			bcScript._setSheet(liveSheet);
			playerCharacters.Add(bcScript);
			bcScript.team = playerCharacters;
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
			bcScript.team = enemyCharacters;
			bcGO.transform.position = _getLinePos(bcScript);
			bcGO.transform.Rotate(0, 180, 0);
		}

		_decideFirstTurn();
	}

	void resetVarsForTurnEnd()
	{
		targOpposed.Clear ();
		targetFriendlies.Clear ();
		activeAttack = null;
		invokeAction = null;
		attackSubState = 0;
		bonus = 0;
		commandsDestroyed = 0;
	}

	public void _resetVariables ()//does not reset the states, maybe it should?
	{
		playerCharacters.Clear();
		enemyCharacters.Clear();
		invokeAction = null;
		MainEngine.self.enemyUsableItems.Clear();
		activeAttack = null;
		attackSubState = 0;
		statusEffectsResolved = 0;
		bonus = 0;
		commandsDestroyed = 0;
		coinsEarned = 0;
		actingBC = null;
		expEarned = 0;
		itemsEarned.Clear ();
		preGotNextCharInLine = false;
		itemToBeUsed = null;
		targetFriendlies.Clear ();
		targOpposed.Clear ();
	}

	// Update is called once per frame
	void Update ()
	{
		switch (currentBState)
		{
			case BATTLESTATE.ResolveStatusEffects:
				if (statusEffectsResolved == actingBC.statusEffectsList.Count) // when all effects have been resolved
				{
					statusEffectsResolved = 0;
					List<StatusEffect> removeEffectsList = new List<StatusEffect> ();

					foreach (StatusEffect effectIter in actingBC.statusEffectsList)
					{
						if (effectIter.turns == 0)
						{
							removeEffectsList.Add (effectIter);
						}
					}

					foreach (StatusEffect removeEffectIter in removeEffectsList)
					{
						actingBC.statusEffectsList.Remove (removeEffectIter);
						Destroy (removeEffectIter.gameObject);
					}

					if (actingBC.sheet.hp > 0 && actingBC.loseTurn == false)
					{
						if (playerCharacters.Contains (actingBC))
						{
							currentBState = BATTLESTATE.InitPlayerDecide;
						}
						else
						{
							currentBState = BATTLESTATE.EnemyDecide;
						}
					}
					else
					{
						actingBC.loseTurn = false;
						currentBState = BATTLESTATE.InitKill;
					}
				}
				else
				{
					actingBC.statusEffectsList [statusEffectsResolved]._applyEffect ();
				}
				break;
			case BATTLESTATE.InitPlayerDecide:
				_initPlayerChoices ();
				currentBState = BATTLESTATE.PlayerDecide;
				break;
			case BATTLESTATE.PlayerDecide:
				break;
			case BATTLESTATE.InitPlayerAttack:
				_destroyAllButtonsAndDropDowns ();
				if (actingBC.sheet.spells.Contains (activeAttack))
				{
					actingBC.sheet.sp -= activeAttack.spCost;
				}

				if (itemToBeUsed != null)
				{
					MainEngine.self._removeItem (itemToBeUsed, 1);
				}
				_setWait (BATTLESTATE.PlayerAttack, standardWaitTime);//slight pause before executing any attack
				currentAState = ATTACKSTATE.InitAttack;
				break;
			case BATTLESTATE.PlayerAttack:
				activeAttack._battleFunction ();
				break;
			case BATTLESTATE.EnemyDecide:
				currentAState = ATTACKSTATE.InitAttack;
				//need Ai for enemies ***
				activeAttack = actingBC.sheet.abilities [0];
				//need Ai for enemies ***
				switch (activeAttack.targetType)
				{
					case TARGETING.ChooseAlly:
						for (int i = 0; i < activeAttack.numberOfTargets && i < enemyCharacters.Count; i++)
						{ //need Ai
							targetFriendlies.Add (enemyCharacters [i]);
						}
						break;

					case TARGETING.ChooseEnemy:
						for (int i = 0; i < activeAttack.numberOfTargets && i < playerCharacters.Count; i++)
						{ //need Ai
							targOpposed.Add (playerCharacters [i]);
						}
						break;

					case TARGETING.AllAllies:
						targetFriendlies.AddRange (enemyCharacters);
						break;
					case TARGETING.AllCharacters:
						targetFriendlies.AddRange (enemyCharacters);
						targOpposed.AddRange (playerCharacters);
						break;
					case TARGETING.AllEnemies:
						targOpposed.AddRange (playerCharacters);
						break;
					case TARGETING.FirstAlly:
						targetFriendlies.Add (enemyCharacters [0]);
						break;
					case TARGETING.FirstEnemy:
						targOpposed.Add (playerCharacters [0]);
						break;
					case TARGETING.Self:
						targetFriendlies.Add (actingBC);
						break;
				}
				currentBState = BATTLESTATE.EnemyAttack;
				break;
			case BATTLESTATE.EnemyAttack:
				activeAttack._battleFunction ();
				break;
			case BATTLESTATE.InitKill:
				resetVarsForTurnEnd();
				bool madeExplosion = false;
				float tombStoneHeight = .35f, tombStoneForward = -.25f;
				foreach (BattleCharacter playerC in playerCharacters)
				{
					if (playerC.sheet.hp <= 0)
					{
						Explosion boom = (Instantiate (explosionPrefab, playerC.hud.transform.position, Quaternion.identity) as GameObject).GetComponent<Explosion> ();
						boom.transform.SetParent (battleCanvas.transform, true);
						boom.transform.localScale = Vector3.one * 2.25f;
						boom.tombStonePos = new Vector3 (playerC.transform.position.x, tombStoneHeight, tombStoneForward);
						boom.killTarget = playerC.hud.gameObject;
						madeExplosion = true;
					}
				}
				foreach (BattleCharacter enemyC in enemyCharacters)
				{
					if (enemyC.sheet.hp <= 0)
					{
						Explosion boom = (Instantiate (explosionPrefab, enemyC.hud.transform.position, Quaternion.identity) as GameObject).GetComponent<Explosion> ();
						boom.transform.SetParent (battleCanvas.transform, true);
						boom.transform.localScale = Vector3.one * 2.25f;
						boom.tombStonePos = new Vector3 (enemyC.transform.position.x, tombStoneHeight, 0);
						boom.killTarget = enemyC.hud.gameObject;
						madeExplosion = true;
					}
				}

				if (madeExplosion)
				{
					currentBState = BATTLESTATE.TombStoneWait;
				}
				else
				{
					_setWait (BATTLESTATE.AdjustLineUp, standardWaitTime);
				}
				break;
			case BATTLESTATE.TombStoneWait:
				break;
			case BATTLESTATE.AdjustLineUp:
				bool lineUpComplete = true;

				foreach (BattleCharacter enemyC in enemyCharacters)
				{
					_listenForLineUp (enemyC);
					enemyC.hud.transform.position = enemyC._calcHudPosition();
					if (enemyC.transform.localPosition.x != _getLinePos(enemyC).x)
					{
						lineUpComplete = false;
					}
				}

				foreach (BattleCharacter playerC in playerCharacters)
				{
					_listenForLineUp (playerC);
					playerC.hud.transform.position = playerC._calcHudPosition();
					if (playerC.transform.localPosition.x != _getLinePos(playerC).x)
					{
						lineUpComplete = false;
					}
				}
				if (lineUpComplete == true)
				{
					if (playerCharacters.Count == 0)
					{
						currentBState = BATTLESTATE.PlayerLose;
						//Engine.self.CurrentSaveInstance._uploadValues ();
						Debug.Log("game over");
						//Engine.self._initiateSceneChange (Engine.self.CurrentSaveInstance.SavedSceneName, doorEnum.SavePoint);
					}
					else if (enemyCharacters.Count == 0)
					{
						currentBState = BATTLESTATE.PlayerWin;
						expEarned = Mathf.RoundToInt (expEarned / (float)playerCharacters.Count);
						expEarned = 10;
						GameObject spoilsDisplay = Instantiate (spoilsPrefab);
						spoilsDisplay.transform.SetParent (battleCanvas.transform, false);
					}
					else
					{
						_initNextTurn ();
					}
				}
				break;
			case BATTLESTATE.PlayerWin:
				//Destroy (MainEngine.self.EncounterOverworldEnemy);
				break;
			case BATTLESTATE.PlayerLose:
				break;
			case BATTLESTATE.Flee:
				break;
			case BATTLESTATE.Wait:
				break;
		}
	}

	void _initNextTurn ()
	{
		if (preGotNextCharInLine == false)
		{
			actingBC = _getNextInLineForTurn (actingBC);
		}
		preGotNextCharInLine = false;
		_setWait (BATTLESTATE.ResolveStatusEffects, standardWaitTime);
	}

	public BattleCharacter _getNextInLineForTurn (BattleCharacter givenChar)
	{
		if (enemyCharacters.IndexOf (givenChar) == enemyCharacters.Count - 1)
		{
			return playerCharacters [0];
		}
		else if (playerCharacters.IndexOf (givenChar) == playerCharacters.Count - 1)
		{
			return enemyCharacters [0];
		}
		else if (enemyCharacters.IndexOf (givenChar) > -1)
		{
			return enemyCharacters [enemyCharacters.IndexOf (givenChar) + 1];
		}
		else if (playerCharacters.IndexOf (givenChar) > -1)
		{
			return playerCharacters [playerCharacters.IndexOf (givenChar) + 1];
		}
		else
		{
			Debug.Log ("error  _getNextInLineForTurn");
			return null;
		}
	}

	void _listenForLineUp (BattleCharacter givenBC)
	{
		if(currentBState == BATTLESTATE.AdjustLineUp)
		{
			givenBC.hud.transform.position = givenBC._calcHudPosition();
		}

		if(iTween.Count(actingBC.gameObject) == 0)
		{
			givenBC.hud.transform.position = givenBC._calcHudPosition(); // safety check
			if (currentBState == BATTLESTATE.PlayerAttack || currentBState == BATTLESTATE.EnemyAttack)
			{
				if(givenBC.sheet.charSpecies == SPECIES.Rat)//consider removing this after demoing the game
				{
					//givenBC.BodyAnimation.Play("Rat_Idle1", PlayMode.StopAll);
				}
				currentBState = BATTLESTATE.InitKill;
				bool shouldFlipPlayer = actingBC.transform.localEulerAngles.y != 0 && playerCharacters.Contains(actingBC);
				bool shouldFlipEnemy = Mathf.RoundToInt (actingBC.transform.localEulerAngles.y) != 180 && enemyCharacters.Contains(actingBC);
				if (shouldFlipPlayer || shouldFlipEnemy)
				{
					actingBC.transform.Rotate (0, 180, 0);
				}
			}
		}
	}

	void _decideFirstTurn()
	{
		currentBState = BATTLESTATE.InitPlayerDecide;
		actingBC = playerCharacters[0];
	}

	void _initPlayerChoices()
	{
		Dropdown abilityDD = (Instantiate (dropDownPrefab, mainDDPosition, Quaternion.identity) as GameObject).GetComponent<Dropdown> ();
		abilityDD.transform.SetParent (battleCanvas.transform, false);
		abilityDD.AddOptions (actingBC.sheet._attacksToOptions (actingBC.sheet.abilities));

		Dropdown spellDD = (Instantiate (dropDownPrefab, mainDDPosition + ddOffsetPosition, Quaternion.identity) as GameObject).GetComponent<Dropdown> ();
		spellDD.transform.SetParent (battleCanvas.transform, false);
		spellDD.AddOptions (actingBC.sheet._attacksToOptions (actingBC.sheet.spells));

		Dropdown itemDD = (Instantiate (dropDownPrefab, mainDDPosition - ddOffsetPosition, Quaternion.identity) as GameObject).GetComponent<Dropdown> ();
		itemDD.transform.SetParent (battleCanvas.transform, false);
		itemDD.AddOptions (MainEngine.self._battleItemsToOptions (false));

		Button abilityButton = (Instantiate (buttonPrefab, mainDDPosition + buttonOffsetPosition, Quaternion.identity) as GameObject).GetComponent<Button> ();
		abilityButton.GetComponentInChildren<Text> ().text = "Abilities";
		abilityButton.transform.SetParent (battleCanvas.transform, false);

		Vector3 spellButtonPosition = mainDDPosition + buttonOffsetPosition + ddOffsetPosition;
		Button spellButton = (Instantiate (buttonPrefab, spellButtonPosition, Quaternion.identity) as GameObject).GetComponent<Button> ();
		spellButton.GetComponentInChildren<Text> ().text = "Spells";
		spellButton.transform.SetParent (battleCanvas.transform, false);

		Vector3 itemButtonPosition = mainDDPosition + buttonOffsetPosition - ddOffsetPosition;
		Button itemButton = (Instantiate (buttonPrefab, itemButtonPosition, Quaternion.identity) as GameObject).GetComponent<Button> ();
		itemButton.GetComponentInChildren<Text> ().text = "Items";
		itemButton.transform.SetParent (battleCanvas.transform, false);

		Vector3 fleeButtonPosition = mainDDPosition + buttonOffsetPosition - ddOffsetPosition * 2;
		Button fleeButton = (Instantiate (buttonPrefab, fleeButtonPosition, Quaternion.identity) as GameObject).GetComponent<Button> ();
		fleeButton.GetComponentInChildren<Text> ().text = "Flee";
		fleeButton.transform.SetParent (battleCanvas.transform, false);

		abilityButton.onClick.AddListener (
			delegate {
				if (actingBC.sheet.abilities.Count > 0)
				{
					_activateOption (actingBC.sheet.abilities [abilityDD.value]);
				}
				else
				{
					bEngineAudio.PlayOneShot (buzzClip);
				}
			});

		spellButton.onClick.AddListener (
			delegate {
				if (actingBC.sheet.spells.Count > 0 && actingBC.sheet.sp >= actingBC.sheet.spells [spellDD.value].spCost)
				{
					_activateOption (actingBC.sheet.spells [spellDD.value]);
				}
				else
				{
					bEngineAudio.PlayOneShot (buzzClip);
				}
			});
		itemButton.onClick.AddListener (
			delegate {
				if (MainEngine.self.playerUsableItems.Count > 0)
				{
					itemToBeUsed = MainEngine.self.playerUsableItems [itemDD.value];
					_activateOption (itemToBeUsed.itemAttack);
				}
				else
				{
					bEngineAudio.PlayOneShot (buzzClip);
				}
			});
		fleeButton.onClick.AddListener (
			delegate {
				_activateOption (actingBC.sheet.retreat);
			});
	}

	void _activateOption(Attack attackInQuestion)
	{
		_destroyAllButtonsAndDropDowns ();

		if (attackInQuestion.targetType == TARGETING.ChooseEnemy)
		{

			foreach (BattleCharacter iterEnemy in enemyCharacters)
			{
				_generateTargetChoiceButton (iterEnemy, attackInQuestion);
			}
			_generateBackButton (attackInQuestion);
		}
		else if (attackInQuestion.targetType == TARGETING.ChooseAlly)
		{
			foreach (BattleCharacter iterAlly in playerCharacters)
			{
				_generateTargetChoiceButton (iterAlly, attackInQuestion);
			}
			_generateBackButton (attackInQuestion);
		}
		else
		{ // non-choosing attacks immediately initiate execution of the attack
			switch (attackInQuestion.targetType)
			{
				case TARGETING.AllAllies:
					targetFriendlies.AddRange (playerCharacters);
					break;
				case TARGETING.AllCharacters:
					targetFriendlies.AddRange (playerCharacters);
					targOpposed.AddRange (enemyCharacters);
					break;
				case TARGETING.AllEnemies:
					targOpposed.AddRange (enemyCharacters);
					break;
				case TARGETING.FirstAlly:
					targetFriendlies.Add (playerCharacters [0]);
					break;
				case TARGETING.FirstEnemy:
					targOpposed.Add (enemyCharacters [0]);
					break;
				case TARGETING.Self:
					targetFriendlies.Add (actingBC);
					break;
			}
			activeAttack = attackInQuestion;
			currentBState = BATTLESTATE.InitPlayerAttack;
		}
	}

	void _destroyAllButtonsAndDropDowns()
	{
		foreach (Dropdown foundDD in FindObjectsOfType<Dropdown>())
		{
			Destroy (foundDD.gameObject);
		}

		foreach (Button foundB in FindObjectsOfType<Button>())
		{
			Destroy (foundB.gameObject);
		}
	}

	void _generateTargetChoiceButton (BattleCharacter givenTarget, Attack selectedAttack)
	{
		Vector3 targetButtonPosition = RectTransformUtility.WorldToScreenPoint (Camera.main, givenTarget.transform.position + choiceButtonOffset);
		Button targetButton = (Instantiate (buttonPrefab, targetButtonPosition, Quaternion.identity) as GameObject).GetComponent<Button> ();
		targetButton.transform.SetParent (battleCanvas.transform, true);
		targetButton.transform.localScale = Vector3.one;//when setting the parent, true keeps the position correct, but enlargers the scale, this is an easy fix
		targetButton.GetComponentInChildren<Text> ().text = givenTarget.sheet.charName;
		targetButton.onClick.AddListener (
			delegate {
				if (enemyCharacters.Contains (givenTarget))
				{
					targOpposed.Add (givenTarget);
					if (selectedAttack.numberOfTargets == targOpposed.Count || targOpposed.Count == enemyCharacters.Count)
					{
						activeAttack = selectedAttack;
						currentBState = BATTLESTATE.InitPlayerAttack;
					}
				}
				else
				{
					targetFriendlies.Add (givenTarget);
					if (selectedAttack.numberOfTargets == targetFriendlies.Count || targetFriendlies.Count == playerCharacters.Count)
					{
						activeAttack = selectedAttack;
						currentBState = BATTLESTATE.InitPlayerAttack;
					}
				}
				Destroy (targetButton.gameObject);
			}
		);
	}

	void _generateBackButton (Attack selectedAttack)//remember the back button is only used during Target selection
	{
		Vector3 backButtonPosition = RectTransformUtility.WorldToScreenPoint (Camera.main, arenaCenter+Vector3.up);
		Button backButton = (Instantiate (buttonPrefab, backButtonPosition, Quaternion.identity) as GameObject).GetComponent<Button> ();
		backButton.transform.SetParent (battleCanvas.transform, true);
		backButton.transform.localScale = Vector3.one;//when setting the parent, true keeps the position correct, but enlargers the scale, this is an easy fix
		backButton.GetComponentInChildren<Text> ().text = "Back";
		backButton.onClick.AddListener (
			delegate {
				if (targOpposed.Count > 0)
				{
					_generateTargetChoiceButton (targOpposed [targOpposed.Count - 1], selectedAttack);
					targOpposed.RemoveAt (targOpposed.Count - 1);
				}
				else if (targetFriendlies.Count > 0)
				{
					_generateTargetChoiceButton (targetFriendlies [targetFriendlies.Count - 1], selectedAttack);
					targOpposed.RemoveAt (targetFriendlies.Count - 1);
				}
				else
				{
					_destroyAllButtonsAndDropDowns ();
					_initPlayerChoices ();
				}
			}
		);
	}

	public void _setWait (BATTLESTATE? givenNextState, float waitTime)
	{
		currentBState = BATTLESTATE.Wait;
		postWaitBState = givenNextState;
		Invoke ("_finishWait", waitTime);
	}

	public void _setWait (ATTACKSTATE? givenNextState, float waitTime)
	{
		postWaitBState = currentBState;
		currentBState = BATTLESTATE.Wait;
		postWaitAState = givenNextState;
		Invoke ("_finishWait", waitTime);
	}

	void _finishWait ()
	{
		currentBState = postWaitBState;
		if (postWaitAState != null)
		{
			currentAState = postWaitAState;
		}

		postWaitBState = null;
		postWaitAState = null;
	}

	public void _setDelayedStateChange (BATTLESTATE? givenNextState, float waitTime)
	{
		delayedBState = givenNextState;
		Invoke ("_finishDelay", waitTime);
	}

	public void _setDelayedStateChange (ATTACKSTATE? givenNextState, float waitTime)
	{
		delayedBState = currentBState;
		delayedAState = givenNextState;
		Invoke ("_finishDelay", waitTime);
	}

	void _finishDelay ()
	{
		currentBState = delayedBState;
		if (delayedAState != null)
		{
			currentAState = delayedAState;
		}

		delayedBState = null;
		delayedAState = null;
	}

	Vector3 _getLinePos(BattleCharacter givenChar)
	{
		Vector3 spaceFromCenter;
		float baseOffset = .75f; // half of the gap distance between the first member of the player party and the first member of the enemy party

		if(playerCharacters.Contains(givenChar))
		{
			spaceFromCenter = new Vector3((playerCharacters.IndexOf(givenChar) + baseOffset) * charSpacing, givenChar.elevation, 0);
			return arenaCenter - spaceFromCenter;
		}
		else
		{
			if(!enemyCharacters.Contains(givenChar))
			{
				Debug.Log("Character not found in either list");
			}

			spaceFromCenter = new Vector3((enemyCharacters.IndexOf(givenChar) + baseOffset) * charSpacing, givenChar.elevation, 0);
			return arenaCenter + spaceFromCenter;
		}
	}

	public void _damageTarget (BattleCharacter targ, int givenDamage)
	{
		int damageDealt = givenDamage; // later this will be modified by weakness/resistance
		int damageTaken = Mathf.Max (1, damageDealt - targ._calcBattleDef()); // minimum 1 damage is always taken
		targ.sheet.hp -= damageTaken;
		targ.sheet.hp = Mathf.Max (0, targ.sheet.hp);//minimum hp is 0
		Vector3 damagePosition = RectTransformUtility.WorldToScreenPoint (Camera.main, targ.transform.position);
		Damage damageScript = (Instantiate (damagePrefab) as GameObject).GetComponent<Damage> ();
		if (playerCharacters.Contains (targ))
		{
			damageScript.scaleDirection = -1;
		}
		damageScript.transform.localPosition = damagePosition;
		damageScript.transform.SetParent (battleCanvas.transform, true);
		damageScript.transform.localScale = Vector3.zero;//when setting the parent, true keeps the position correct, but enlargers the scale, this is an easy fix
		damageScript.damageLabel.text = damageTaken.ToString ();
	}

	void invokableDelegate()
	{
		invokeAction();
	}

	public void _squirmingClaws()
	{
		switch (currentAState)
		{
			case ATTACKSTATE.InitAttack:
				RapidCommand command = (Instantiate (rapidCommandPrefab, commandPosition, Quaternion.identity) as GameObject).GetComponent<RapidCommand> ();
				command._setAttributes("z", 3f, -1, true, Random.Range(0, 22));
				Vector3 swipePos = new Vector3(targOpposed[0].transform.position.x, actingBC.transform.position.y, actingBC.transform.position.z);
				swipePos.x += targOpposed[0].transform.right.x * (targOpposed[0].bcCollider.bounds.extents.x + actingBC.bcCollider.bounds.extents.x);
				iTween.MoveTo(actingBC.gameObject, iTween.Hash(iT.MoveTo.position, swipePos, iT.MoveTo.speed, 1, iT.MoveTo.easetype, "easeInOutQuad"));
				currentAState = ATTACKSTATE.MovePreAction;
				break;
			case ATTACKSTATE.MovePreAction:
				if(iTween.Count(actingBC.gameObject) == 0)
				{
					if(targOpposed[0].elevation == 0)
					{
						_setWait (ATTACKSTATE.ActionState, standardWaitTime); // pause briefly before swiping at enemy
					}
					else // miss the target
					{
						actingBC.transform.Rotate (0, 180, 0);
						_setWait (ATTACKSTATE.MovePostAction, standardWaitTime); // pause briefly before returning due to non-contact
					}
				}
				break;
			case ATTACKSTATE.ActionState:
				if (commandsDestroyed > 0)
				{
					currentAState = ATTACKSTATE.ApplyAttack;
					bonus = bonus / 6; // 6 is an arbitrarily choesn number
				}
				break;
			case ATTACKSTATE.ApplyAttack:
				bool mystics = false;

				if (mystics == true)
				{
					_damageTarget (actingBC, 1);
					_setWait (ATTACKSTATE.HandleFail, Damage.popTime + standardWaitTime);
					break;
				}

				if (bonus > -1)
				{
					bonus -= 1;
					_damageTarget (targOpposed [0], activeAttack.baseDamage + actingBC._calcBattlePow());
					_setWait (ATTACKSTATE.ApplyAttack, Damage.popTime + 2*standardWaitTime); // pause then swing, this will happen repeatedly until player runs out of bonus
				}
				else
				{
					actingBC.transform.Rotate (0, 180, 0);
					_setWait (ATTACKSTATE.MovePostAction, standardWaitTime); // small wait to visually seperate attacking from returning
					iTween.MoveTo(actingBC.gameObject, iTween.Hash(iT.MoveTo.position, _getLinePos(actingBC), iT.MoveTo.speed, 1, iT.MoveTo.easetype, "easeInOutQuad"));
				}
				break;
			case ATTACKSTATE.HandleFail:
				actingBC.transform.Rotate (0, 180, 0);
				currentAState = ATTACKSTATE.MovePostAction;
				break;
			case ATTACKSTATE.MovePostAction:
				_listenForLineUp (actingBC);
				break;
		}
	}

	public void _plagueBite()
	{
		switch (currentAState)
		{
			case ATTACKSTATE.InitAttack:
				PressCommand command = (Instantiate (pressCommandPrefab, commandPosition, Quaternion.identity) as GameObject).GetComponent<PressCommand> ();
				command._setAttributes("x", 3f, -1, true, Random.Range(0, 22));
				Vector3 swipePos = new Vector3(targOpposed[0].transform.position.x, actingBC.transform.position.y, actingBC.transform.position.z);
				swipePos.x += targOpposed[0].transform.right.x * (targOpposed[0].bcCollider.bounds.extents.x + actingBC.bcCollider.bounds.extents.x);
				iTween.MoveTo(actingBC.gameObject, iTween.Hash(iT.MoveTo.position, swipePos, iT.MoveTo.speed, 1, iT.MoveTo.easetype, "easeInOutQuad"));
				currentAState = ATTACKSTATE.MovePreAction;
				break;
			case ATTACKSTATE.MovePreAction:
				if(iTween.Count(actingBC.gameObject) == 0)
				{
					if(targOpposed[0].elevation == 0)
					{
						_setWait (ATTACKSTATE.ActionState, standardWaitTime); // pause briefly before swiping at enemy
					}
					else // miss the target
					{
						actingBC.transform.Rotate (0, 180, 0);
						_setWait (ATTACKSTATE.MovePostAction, standardWaitTime); // pause briefly before returning due to non-contact
					}
				}
				break;
			case ATTACKSTATE.ActionState:
				if (commandsDestroyed > 0)
				{
					currentAState = ATTACKSTATE.ApplyAttack;
				}
				break;
			case ATTACKSTATE.ApplyAttack:
				bool mystics = false;

				if (mystics == true)
				{
					_damageTarget (actingBC, 1);
					_setWait (ATTACKSTATE.HandleFail, Damage.popTime + standardWaitTime);
					break;
				}

				_damageTarget (targOpposed [0], activeAttack.baseDamage + actingBC._calcBattlePow());

				if (bonus == 1)
				{
					Poison effect = Instantiate (statusEffectPrefab).AddComponent<Poison> ();
					effect.turns = 2;
					targOpposed [0]._addStatusEffect (effect);
				}
				actingBC.transform.Rotate (0, 180, 0);
				_setWait (ATTACKSTATE.MovePostAction, standardWaitTime); // small wait to visually seperate attacking from returning
				iTween.MoveTo(actingBC.gameObject, iTween.Hash(iT.MoveTo.position, _getLinePos(actingBC), iT.MoveTo.speed, 1, iT.MoveTo.easetype, "easeInOutQuad"));

				break;
			case ATTACKSTATE.HandleFail:
				actingBC.transform.Rotate (0, 180, 0);
				currentAState = ATTACKSTATE.MovePostAction;
				break;
			case ATTACKSTATE.MovePostAction:
				_listenForLineUp (actingBC);
				break;
		}
	}

	public void _sewerStench()
	{
		switch (currentAState)
		{
			case ATTACKSTATE.InitAttack:
				ChargeCommand command = (Instantiate (chargeCommandPrefab) as GameObject).GetComponent<ChargeCommand> ();
				command._setAttributes("z", 6f, -1, true, Random.Range(-1, 3));
				command._setChargeSpecificAttributes(true, 1f, true, 0f);
				currentAState = ATTACKSTATE.ActionState;
				break;
			case ATTACKSTATE.MovePreAction:
				break;
			case ATTACKSTATE.ActionState:
				if (commandsDestroyed > 0)
				{
					currentAState = ATTACKSTATE.ApplyAttack;
				}
				break;
			case ATTACKSTATE.ApplyAttack:
				Stench effect = Instantiate (statusEffectPrefab).AddComponent<Stench> ();
				effect.turns = 1 + Mathf.Max(0, bonus);
				targetFriendlies [0]._addStatusEffect (effect);
				currentAState = ATTACKSTATE.MovePostAction;
				break;
			case ATTACKSTATE.MovePostAction:
				_listenForLineUp (actingBC);
				break;
		}
	}

	public void _piedPiper()
	{
		switch (currentAState)
		{
			case ATTACKSTATE.InitAttack:

				Instantiate (pipeCommandPrefab);
				_setWait(ATTACKSTATE.ActionState, standardWaitTime);
				break;
			case ATTACKSTATE.MovePreAction:
				break;
			case ATTACKSTATE.ActionState:
				if (!FindObjectOfType<PipeCommand> ())
				{
					bonus = Mathf.Max(1, (bonus+1)/2);
					for(int i = 0; i < bonus; i++)
					{
						float xOffset = (actingBC.transform.position.x + .1f * i);// * 1.5f) + (-3f * actingBC.transform.right.x);
						GameObject pRat = Instantiate(pipeRatPrefab);
						pRat.transform.position = new Vector3(xOffset, actingBC.transform.position.y, 0);
						pRat.transform.rotation = actingBC.transform.rotation;
					}
					currentAState = ATTACKSTATE.ApplyAttack;
				}
				break;
			case ATTACKSTATE.ApplyAttack:
				if (!FindObjectOfType<PipeRat> ())
				{
					currentAState = ATTACKSTATE.MovePostAction;
				}
				break;
			case ATTACKSTATE.MovePostAction:
				if (!FindObjectOfType<Damage> ())
				{
					_listenForLineUp (actingBC);
				}
				break;
		}
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
