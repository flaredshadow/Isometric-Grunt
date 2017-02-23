using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Reflection;

public class PlayerHud : MonoBehaviour {

	public static int hudsRankingUp = 0;

	public GameObject plusPrefab;
	public Text nameLabel, rankLabel, hpLabel, spLabel, expLabel, powLabel, defLabel, electiveLabel;
	public Button useSpellButton;
	public Dropdown hudSpellDD;
	public LayoutGroup statusEffectsLayoutGroup;
	public StatSheet sheet;

	[HideInInspector]
	public BattleCharacter owningBattleCharacter;

	// Use this for initialization
	void Start ()
	{
		if(useSpellButton != null)
		{
			if(MainEngine.self.currentGameState != GAMESTATE.Paused)
			{
				Destroy(useSpellButton.gameObject);
			}
			else
			{
				useSpellButton.GetComponentInChildren<Dropdown>().AddOptions (sheet._attacksToOptions (sheet.spells));
				useSpellButton.onClick.AddListener (
					delegate {
						Attack currentSelectedSpell = null;
						bool isOverride = false;

						if(sheet.spells.Count > 0)
						{
							currentSelectedSpell = sheet.spells[hudSpellDD.value];
							MethodInfo mInfo = currentSelectedSpell.GetType().GetMethod("_overworldFunction");
							isOverride = !mInfo.Equals(mInfo.GetBaseDefinition());
						}
						if (currentSelectedSpell != null && sheet.sp >= sheet.spells[hudSpellDD.value].spCost && isOverride == true)
						{
							sheet.spells[hudSpellDD.value]._overworldFunction();
							sheet.sp -= sheet.spells[hudSpellDD.value].spCost;
						}
						else
						{
							
							MainEngine.self.audioSource.PlayOneShot (MainEngine.self.buzzClip);
						}
					});
			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(MainEngine.self.currentGameState == GAMESTATE.EnterScene) // && !MainEngine.self.CurrentSceneName.Equals(Engine.self.BattleSceneName))
		{
			Destroy(gameObject);
		}
		else
		{
			_updateLabels();
		}
	}

	void _updateLabels()
	{
		nameLabel.text = sheet.charName;
		if(expLabel != null) // in overworld
		{
			rankLabel.text = "Rank : " + sheet.charSpecies;
			hpLabel.text = "HP : " + sheet.hp + " / " + sheet._calcMaxHp() + " (" + sheet.maxHp + ")";
			spLabel.text = "SP : " + sheet.sp + " / " + sheet._calcMaxSp()  + " (" + sheet.maxSp + ")";
			expLabel.text = "EXP : " + sheet.exp + " / " + sheet.maxExp;
			powLabel.text = "POW : " + sheet._calcPow()  + " (" + sheet.pow + ")";
			defLabel.text = "DEF : " + sheet._calcDef() + " (" + sheet.def + ")";
			electiveLabel.text = "Elective Points : " + sheet.electivePoints;
		}
		else
		{
			rankLabel.text = "R : " + sheet.charSpecies;
			hpLabel.text = "HP : " + sheet.hp + " / " + owningBattleCharacter._calcBattleMaxHp();
			spLabel.text = "SP : " + sheet.sp + " / " + owningBattleCharacter._calcBattleMaxSp();
		}
	}

	public void _makeStatAdders()
	{
		Vector3 spacing = new Vector3(70, 0, 0); // the distance between the stat text and the column of plusses
		List<Button> plusList = new List<Button>();
		int numberOfStats = 4;
		hudsRankingUp += 1;//static counter of all huds that are in the process of ranking up

		for(int i = 0; i < numberOfStats; i++)
		{
			Button plus = (Instantiate(plusPrefab) as GameObject).GetComponent<Button>();
			plusList.Add(plus);
			plus.transform.SetParent(transform, false); // untested fix : attached to this instead of the canvas
			plus.onClick.AddListener(
				delegate
					{
						sheet.electivePoints -= 1;
						if(sheet.electivePoints == 0)
						{
							hudsRankingUp -= 1;
							if(hudsRankingUp == 0)
							{
								Spoils.self.currentSpoilsState = Spoils.SPOILSSTATE.AddExp;
							}

							foreach(Button plusButton in plusList)
							{
								Destroy(plusButton.gameObject);
							}
						}
					}
			);
		}

		plusList[0].transform.position = hpLabel.transform.position + spacing;
		plusList[0].onClick.AddListener(delegate{sheet.maxHp += 1; sheet.hp = sheet._calcMaxHp();});

		plusList[1].transform.position = spLabel.transform.position + spacing;
		plusList[1].onClick.AddListener(delegate{sheet.maxSp += 1; sheet.sp = sheet._calcMaxSp();});

		plusList[2].transform.position = powLabel.transform.position + spacing;
		plusList[2].onClick.AddListener(delegate{sheet.pow += 1;});

		plusList[3].transform.position = defLabel.transform.position + spacing;
		plusList[3].onClick.AddListener(delegate{sheet.def += 1;});
	}

	void OnDestroy()
	{
		if(hudSpellDD != null)
		{
			hudSpellDD.Hide(); // needed to prevent leftover blockers
		}
	}
}
