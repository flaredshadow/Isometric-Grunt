using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RapidCommand : ActionCommand
{
	bool isAnimating = false;

	public override void _activeChildUpdate()
	{
		if(isAnimating == false)
		{
			InvokeRepeating("_switchSprite", 0, .1f);
			isAnimating = true;
		}

		if(Input.GetKeyDown(actionKey))
		{
			BattleEngine.self.bonus += 1;
		}
	}
}