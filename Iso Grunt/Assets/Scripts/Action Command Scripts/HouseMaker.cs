using UnityEngine;
using System.Collections;

public class HouseMaker : MonoBehaviour {

	public GameObject houseBlockPrefab;

	HouseBlock currentBlock;

	public HouseBlock CurrentBlock {
		get {
			return currentBlock;
		}
		set {
			currentBlock = value;
		}
	}

	int[] blockTypeCount = {0, 0, 0};

	public int[] BlockTypeCount {
		get {
			return blockTypeCount;
		}
		set {
			blockTypeCount = value;
		}
	}

	// Use this for initialization
	void Start ()
	{
		_makeBlock();
		Destroy(gameObject, 25f);
		//InvokeRepeating("_makeBlock", 1f, 5f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void _makeBlock()
	{
		float spawnY = 12f;
		currentBlock = (Instantiate(houseBlockPrefab, Vector3.up * spawnY, Quaternion.identity) as GameObject).GetComponent<HouseBlock>();
		currentBlock.transform.SetParent(transform);
		currentBlock.MyHouseMaker = this;
	}

	void OnDestroy()
	{
		BattleEngine.self.currentAState = BattleEngine.ATTACKSTATE.MovePostAction;

		int blockCountBonus = 0;
		int sturdyDefBonus = 3;

		if(blockTypeCount[0] == 0) // no straw
		{
			if(blockTypeCount[1] == 0) // no sticks
			{
				sturdyDefBonus = 10;
				blockCountBonus = blockTypeCount[2];
			}
			else
			{
				sturdyDefBonus = 5;
				blockCountBonus = blockTypeCount[1] + blockTypeCount[2];
			}
		}
		else
		{
			blockCountBonus = blockTypeCount[0] + blockTypeCount[1] + blockTypeCount[2];
		}

		foreach(BattleCharacter bcIter in BattleEngine.self.targetFriendlies)
		{
			House effect = Instantiate (BattleEngine.self.statusEffectPrefab).AddComponent<House> ();
			effect.turns = 1 + Mathf.Max(0, blockCountBonus/3 + 1);
			effect.defBuff = sturdyDefBonus;
			bcIter._addStatusEffect(effect);
		}
	}
}
