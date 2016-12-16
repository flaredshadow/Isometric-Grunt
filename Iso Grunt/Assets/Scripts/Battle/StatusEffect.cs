using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StatusEffect : MonoBehaviour {
	
	public enum EFFECTSTATE {InitApply, ActivelyApply, FinishApply};

	public GameObject dizzyStarsPrefab;
	public Sprite poisonIcon, paralysisIcon, swordIcon, shieldIcon;

	public BattleCharacter owner;

	public EFFECTSTATE currentEffectState = EFFECTSTATE.InitApply;

	protected string statusName;
	public int turns, maxHpBuff, maxSpBuff, powBuff, defBuff;

	public Sprite icon;

	// Use this for initialization
	void Start () {
		transform.GetChild(0).GetComponent<Image>().sprite = icon;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void _applyEffect()
	{
		switch(currentEffectState)
		{
			case EFFECTSTATE.InitApply:
				_applyInitChildEffect();
				break;
			case EFFECTSTATE.ActivelyApply:
				_applyActivelyChildEffect();
				break;
			case EFFECTSTATE.FinishApply:
				_applyFinishChildEffect();
				break;
		}
	}

	public virtual void _applyInitChildEffect()
	{
		currentEffectState = EFFECTSTATE.ActivelyApply;
	}

	public virtual void _applyActivelyChildEffect()
	{
		currentEffectState = EFFECTSTATE.FinishApply;
	}

	public virtual void _applyFinishChildEffect()
	{
		currentEffectState = EFFECTSTATE.InitApply;
		turns -= 1;
		BattleEngine.self.statusEffectsResolved += 1;
	}
}

public class Poison : StatusEffect
{
	public Poison()
	{
		statusName = "Poisoned";
		icon = poisonIcon;
	}

	public override void _applyInitChildEffect()
	{
		base._applyInitChildEffect();
		BattleEngine.self._damageTarget(owner, 1);
	}

	public override void _applyActivelyChildEffect()
	{
		if(!FindObjectOfType<Damage>())
		{
			base._applyActivelyChildEffect();
		}
	}

	public override void _applyFinishChildEffect()
	{
		//Debug.Log("waiting on effect resolution");
		BattleEngine.self._setWait(BattleEngine.BATTLESTATE.ResolveStatusEffects, .5f);
		base._applyFinishChildEffect();
	}
}

public class Stench : StatusEffect
{
	public Stench()
	{
		statusName = "Stench";
		icon = shieldIcon;
		defBuff = 2;
	}
}

public class Ravenous : StatusEffect
{
	public Ravenous()
	{
		statusName = "Ravenous";
		icon = swordIcon;
		//pow is buffed based on bonus
	}
}

public class House : StatusEffect
{
	public House()
	{
		statusName = "House";
		icon = shieldIcon;
		//def is buffed based on bonus
	}
}

public class Dizzy : StatusEffect
{
	public Dizzy()
	{
		statusName = "Dizzy";
		icon = paralysisIcon;
	}

	public override void _applyInitChildEffect()
	{
		base._applyInitChildEffect();
		owner.loseTurn = Random.Range(0,2) == 0 ? false : true;
		DizzyStars stars = (Instantiate(dizzyStarsPrefab, owner.transform.position + Vector3.up, Quaternion.identity) as GameObject).GetComponent<DizzyStars>();
		stars.transform.position = owner.transform.position + Vector3.up;
		stars.dizzyBattleCharacter = owner;

	}

	public override void _applyActivelyChildEffect()
	{
		if(!FindObjectOfType<DizzyStars>())
		{
			base._applyActivelyChildEffect();
		}
	}

	public override void _applyFinishChildEffect()
	{
		//Debug.Log("waiting on effect resolution");
		BattleEngine.self._setWait(BattleEngine.BATTLESTATE.ResolveStatusEffects, .5f);
		base._applyFinishChildEffect();
	}
}