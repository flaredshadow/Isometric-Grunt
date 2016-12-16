using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class StatSheet {

	public string charName;
	public SPECIES charSpecies;
	public List<Attack> abilities = new List<Attack>();
	public List<Attack> spells = new List<Attack>();
	public int exp, maxExp, maxExpGrowth, level, hp, maxHp, sp, maxSp, pow, def, electivePoints = 0,
	hpGain, spGain, powGain, defGain, electivePointsGain, expWorth, coinWorth, flightHeight;
	public Attack retreat;

	public StatSheet(string givenName, SPECIES givenSpecies)
	{
		charName = givenName;

		switch(givenSpecies)
		{
			case SPECIES.Rat:
				maxHp = 1;
				maxSp = 5;
				abilities.Add(new SquirmingClaws());
				abilities.Add(new PiedPiper());
				abilities.Add(new PoisonTest());
				spells.Add(new PlagueBite());
				spells.Add(new SewerStench());
				//potentialItems.Add(new Potion());
				//potentialItemsChances.Add(.5f);
				break;
		}

		hp = maxHp;
		sp = maxSp;
		exp = 0;
	}

	public List<Dropdown.OptionData> _attacksToOptions(List<Attack> givenAttackList)
	{
		List<Dropdown.OptionData> odList = new List<Dropdown.OptionData>();
		foreach(Attack atk in givenAttackList)
		{
			string attackText = atk.attackName;
			if(givenAttackList == spells)
			{
				attackText += " : " + atk.spCost + " SP";
			}
			odList.Add(new Dropdown.OptionData(){text = attackText});
		}
		return odList;
	}

	public void _rankUp()
	{
		maxHp += hpGain;
		hp = maxHp;
		maxSp += spGain;
		sp = maxSp;
		pow += powGain;
		def += defGain;
		electivePoints += electivePointsGain;
	}

	//possibly add equipment stat modifiers in future
	public int _calcMaxHp()
	{
		return maxHp;
	}

	public int _calcMaxSp()
	{
		return maxSp;
	}

	public int _calcPow()
	{
		return pow;
	}

	public int _calcDef()
	{
		return def;
	}
}
