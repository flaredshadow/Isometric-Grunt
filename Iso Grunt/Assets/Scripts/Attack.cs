using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class Attack
{
	public string attackName;

	public int baseDamage, baseHealing, spCost, numberOfTargets;

	public TARGETING targetType;

	public virtual void _battleFunction()
	{

	}

	public virtual void _overworldFunction()
	{
	}
}

[Serializable]
public class SquirmingClaws : Attack
{
	public SquirmingClaws()
	{
		attackName = "Squirming Claws";
		baseDamage = 1;
		baseHealing = 0;
		spCost = 0;
		numberOfTargets = 1;
		targetType = TARGETING.ChooseEnemy;
	}

	public override void _battleFunction()
	{
		BattleEngine.self._squirmingClaws();
	}

	public override void _overworldFunction()
	{
		Debug.Log("Oworld test success");
	}
}

[Serializable]
public class PlagueBite : Attack
{
	public PlagueBite()
	{
		attackName = "Plague Bite";
		baseDamage = 2;
		baseHealing = 0;
		spCost = 0;
		numberOfTargets = 1;
		targetType = TARGETING.ChooseEnemy;
	}

	public override void _battleFunction()
	{
		BattleEngine.self._plagueBite();
	}

	public override void _overworldFunction()
	{
		Debug.Log("Oworld test success");
	}
}

[Serializable]
public class SewerStench : Attack
{
	public SewerStench()
	{
		attackName = "Sewer Stench";
		baseDamage = 0;
		baseHealing = 0;
		spCost = 2;
		numberOfTargets = 1;
		targetType = TARGETING.Self;
	}

	public override void _battleFunction()
	{
		BattleEngine.self._sewerStench();
	}
}

[Serializable]
public class PiedPiper : Attack
{
	public PiedPiper()
	{
		attackName = "Pied Piper";
		baseDamage = 0;
		baseHealing = 0;
		spCost = 3;
		numberOfTargets = 4;
		targetType = TARGETING.AllEnemies;
	}

	public override void _battleFunction()
	{
		BattleEngine.self._piedPiper();
	}
}

[Serializable]
public class Swoop : Attack
{
	public Swoop()
	{
		attackName = "Swoop";
		baseDamage = 2;
		baseHealing = 0;
		spCost = 3;
		numberOfTargets = 1;
		targetType = TARGETING.ChooseEnemy;
	}

	public override void _battleFunction()
	{
		BattleEngine.self._swoop();
	}
}

[Serializable]
public class ScentOfBlood : Attack
{
	public ScentOfBlood()
	{
		attackName = "Scent Of Blood";
		baseDamage = 0;
		baseHealing = 0;
		spCost = 1;
		numberOfTargets = 1;
		targetType = TARGETING.Self;
	}

	public override void _battleFunction()
	{
		BattleEngine.self._scentOfBlood();
	}
}

[Serializable]
public class EchoScreech : Attack
{
	public EchoScreech()
	{
		attackName = "Echo Screech";
		baseDamage = 0;
		baseHealing = 0;
		spCost = 1;
		numberOfTargets = 4;
		targetType = TARGETING.AllEnemies;
	}

	public override void _battleFunction()
	{
		BattleEngine.self._echoScreech();
	}
}

[Serializable]
public class NightFlight : Attack
{
	public NightFlight()
	{
		attackName = "Night Flight";
		baseDamage = 50;
		baseHealing = 8;
		spCost = 1;
		numberOfTargets = 1;
		targetType = TARGETING.FirstEnemy;

	}

	public override void _battleFunction()
	{
		BattleEngine.self._nightFlight();
	}
}

[Serializable]
public class TuskFling : Attack
{
	public TuskFling()
	{
		attackName = "Tusk Fling";
		baseDamage = 2;
		baseHealing = 0;
		spCost = 1;
		numberOfTargets = 1;
		targetType = TARGETING.FirstEnemy;

	}

	public override void _battleFunction()
	{
		BattleEngine.self._tuskFling();
	}
}

[Serializable]
public class BodySlam : Attack
{
	public BodySlam()
	{
		attackName = "Body Slam";
		baseDamage = 2;
		baseHealing = 0;
		spCost = 1;
		numberOfTargets = 1;
		targetType = TARGETING.ChooseEnemy;

	}

	public override void _battleFunction()
	{
		BattleEngine.self._bodySlam();
	}
}

[Serializable]
public class MudCannonBall : Attack
{
	public MudCannonBall()
	{
		attackName = "Mud Cannon Ball";
		baseDamage = 2;
		baseHealing = 2;
		spCost = 1;
		numberOfTargets = 8;
		targetType = TARGETING.AllCharacters;

	}

	public override void _battleFunction()
	{
		BattleEngine.self._mudCannonBall();
	}
}

[Serializable]
public class ThreeLittlePigs : Attack
{
	public ThreeLittlePigs()
	{
		attackName = "Three Little Pigs";
		baseDamage = 0;
		baseHealing = 0;
		spCost = 1;
		numberOfTargets = 4;
		targetType = TARGETING.AllAllies;

	}

	public override void _battleFunction()
	{
		BattleEngine.self._threeLittlePigs();
	}
}

[Serializable]
public class TalonDrop : Attack
{
	public TalonDrop()
	{
		attackName = "Talon Drop";
		baseDamage = 0;
		baseHealing = 0;
		spCost = 1;
		numberOfTargets = 1;
		targetType = TARGETING.ChooseEnemy;

	}

	public override void _battleFunction()
	{
		BattleEngine.self._talonDrop();
	}
}

[Serializable]
public class Flee : Attack
{
	public Flee()
	{
		attackName = "Flee";
		baseDamage = 0;
		baseHealing = 0;
		spCost = 0;
		numberOfTargets = 1;
		targetType = TARGETING.Self;
	}

	public override void _battleFunction()
	{
		BattleEngine.self._flee();
	}
}

[Serializable]
public class PoisonTest : Attack
{
	public PoisonTest()
	{
		attackName = "Poison Test";
		baseDamage = 0;
		baseHealing = 0;
		spCost = 0;
		numberOfTargets = 1;
		targetType = TARGETING.FirstEnemy;
	}

	public override void _battleFunction()
	{
		BattleEngine.self._poisonTest();
	}
}

[Serializable]
public class HealTest : Attack
{
	public HealTest()
	{
		attackName = "Heal Test";
		baseDamage = 0;
		baseHealing = 2;
		spCost = 0;
		numberOfTargets = 1;
		targetType = TARGETING.Self;
	}

	public override void _battleFunction()
	{
		BattleEngine.self._healTest();
	}
}