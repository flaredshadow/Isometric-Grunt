using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StatSheet {

	public string charName;
	public SPECIES charSpecies;
	public List<Attack> abilities = new List<Attack>();
	public List<Attack> spells = new List<Attack>();
	public int exp, maxExp, maxExpGrowth, level, hp, maxHp, sp, maxSp, pow, def, electivePoints = 0,
	hpGain, spGain, powGain, defGain, electivePointsGain, expWorth, coinWorth, flightHeight;

	public StatSheet(string givenName, SPECIES givenSpecies)
	{
		charName = givenName;

		switch(givenSpecies)
		{
			case SPECIES.Rat:
				maxHp = 5;
				maxSp = 5;
				break;
		}

		hp = maxHp;
		sp = maxSp;
		exp = 0;
	}
}
