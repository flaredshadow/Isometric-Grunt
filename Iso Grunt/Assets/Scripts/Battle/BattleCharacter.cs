using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class BattleCharacter : MonoBehaviour {

	public static float elevationSpacing = 3.0f;

	public StatSheet sheet;
	public float elevation, pointFoundThresh = .02f;
	public List<BattleCharacter> team;
	public SpriteRenderer spRenderer;
	public Rigidbody2D rBody;
	public BoxCollider2D bcCollider;
	public bool loseTurn;
	public PlayerHud hud;
	public GameObject hitGO;
	public List<StatusEffect> statusEffectsList = new List<StatusEffect>();

	// Use this for initialization
	void Start () {
		adjustCollider();
		Vector3 hudPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.localPosition.x * Vector3.right);
		hudPosition.y = Screen.height/8f; // 0 for screen y axis is bottom screen edge
		hud = (Instantiate(MainEngine.self.playerHudPrefab, hudPosition, Quaternion.identity) as GameObject).GetComponent<PlayerHud>();
		hud.transform.SetParent(BattleEngine.self.battleCanvas.transform, true);
		hud.transform.localScale = Vector3.one;
		hud.sheet = sheet;
		hud.owningBattleCharacter = this;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void adjustCollider()
	{
		bcCollider.size = spRenderer.sprite.bounds.size;
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		//Debug.Log("hit");
		hitGO = other.gameObject;
	}

	public void _setSheet(StatSheet givenSheet)
	{
		sheet = givenSheet;
		elevation = sheet.flightHeight * elevationSpacing;

		//need to set sprite to match species
	}

	public Vector3 _calcHudPosition()
	{
		Vector3 hudPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.localPosition.x * Vector3.right);
		hudPosition.y = Screen.height/8f; // 0 for screen y axis is bottom screen edge
		return hudPosition;
	}

	public bool _approach(Vector3 givenDestination, float givenSpeed) // returns true to designate arrival at destination
	{
		if(Vector3.Distance(transform.position, givenDestination) <= pointFoundThresh)
		{
			transform.position = givenDestination;
			return true;
		}
		else
		{
			iTween.MoveTo(gameObject, iTween.Hash(iT.MoveTo.position, givenDestination, iT.MoveTo.speed, givenSpeed*5, iT.MoveTo.easetype, "easeInOutQuad"));
			return false;
		}
	}

	public void _addStatusEffect(StatusEffect givenStatusEffect)
	{
		foreach(StatusEffect effectsIter in statusEffectsList)
		{
			if(givenStatusEffect.GetType() == effectsIter.GetType())
			{
				effectsIter.maxHpBuff = givenStatusEffect.maxHpBuff;
				effectsIter.maxSpBuff = givenStatusEffect.maxSpBuff;
				effectsIter.powBuff = givenStatusEffect.powBuff;
				effectsIter.defBuff = givenStatusEffect.defBuff;
				effectsIter.turns += givenStatusEffect.turns;
				Destroy(givenStatusEffect.gameObject);
				return;
			}
		}

		if(statusEffectsList.Count < hud.GetComponentInChildren<GridLayoutGroup>().constraintCount * 2) // 10 is the current maximum that can be fit neatly under the hud
		{
			statusEffectsList.Add(givenStatusEffect);
			givenStatusEffect.owner = this;
			givenStatusEffect.transform.SetParent(hud.statusEffectsLayoutGroup.transform);
			givenStatusEffect.transform.localScale = Vector3.one;
		}
		else
		{
			BattleEngine.self.bEngineAudio.PlayOneShot(BattleEngine.self.buzzClip);
			Debug.Log("Maximum amount of status effects");
		}
	}

	public int _sumAllMaxHpBuffs()
	{
		int totalMaxHpBuff = 0;
		foreach(StatusEffect effectIter in statusEffectsList)
		{
			totalMaxHpBuff += effectIter.maxHpBuff;
		}
		return totalMaxHpBuff;
	}

	public int _sumAllMaxSpBuffs()
	{
		int totalMaxSpBuff = 0;
		foreach(StatusEffect effectIter in statusEffectsList)
		{
			totalMaxSpBuff += effectIter.maxSpBuff;
		}
		return totalMaxSpBuff;
	}

	public int _sumAllPowBuffs()
	{
		int totalPowBuff = 0;
		foreach(StatusEffect effectIter in statusEffectsList)
		{
			totalPowBuff += effectIter.powBuff;
		}
		return totalPowBuff;
	}

	public int _sumAllDefBuffs()
	{
		int totalDefBuff = 0;
		foreach(StatusEffect effectIter in statusEffectsList)
		{
			totalDefBuff += effectIter.defBuff;
		}
		return totalDefBuff;
	}

	public int _calcBattleMaxHp()
	{
		return sheet._calcMaxHp() + _sumAllMaxHpBuffs();
	}

	public int _calcBattleMaxSp()
	{
		return sheet._calcMaxSp() + _sumAllMaxSpBuffs();
	}

	public int _calcBattlePow()
	{
		return sheet._calcPow() + _sumAllPowBuffs();
	}

	public int _calcBattleDef()
	{
		return sheet._calcDef() + _sumAllDefBuffs();
	}
}
