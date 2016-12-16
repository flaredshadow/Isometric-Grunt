using UnityEngine;
using System.Collections;

public class TombStone : MonoBehaviour {

	public static int totalTombStones;
	public static float popTime = 1.5f;

	GameObject deadCharacter;

	// Use this for initialization
	void Start ()
	{
		totalTombStones += 1;
		float yEnd = GetComponent<BoxCollider>().bounds.extents.y/2f + BattleEngine.self.arenaCenter.y;
		Vector3 destination = new Vector3(transform.position.x, yEnd, transform.position.z);
		iTween.MoveTo(gameObject, iTween.Hash(iT.MoveTo.position, destination, iT.MoveTo.easetype, "easeInExpo", iT.MoveTo.time, 2));
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void OnTriggerEnter (Collider other)
	{
		if(other.GetComponent<BattleCharacter>() != null)
		{
			deadCharacter = other.gameObject;
			Destroy(gameObject, 1.5f);
		}
	}

	void OnDestroy()
	{
		totalTombStones -=1;
		BattleCharacter bc = deadCharacter.GetComponent<BattleCharacter>();

		if(BattleEngine.self.actingBC == deadCharacter.GetComponent<BattleCharacter>() && BattleEngine.self.preGotNextCharInLine == false)
		{
			BattleEngine.self.preGotNextCharInLine = true;
			while(BattleEngine.self.actingBC.sheet.hp <= 0)
			{
				BattleEngine.self.actingBC = BattleEngine.self._getNextInLineForTurn(bc);
				if(BattleEngine.self.actingBC == deadCharacter.GetComponent<BattleCharacter>())
				{
					Debug.Log("Everyone is dead");
					break;
				}
			}
		}

		if(BattleEngine.self.enemyCharacters.Contains(bc))
		{
			BattleEngine.self.enemyCharacters.Remove(bc);
			BattleEngine.self.expEarned += bc.sheet.expWorth;
			BattleEngine.self.coinsEarned += bc.sheet.coinWorth;
		}
		else
		{
			BattleEngine.self.playerCharacters.Remove(bc);
		}
			
		Destroy(deadCharacter);

		if(totalTombStones == 0)
		{
			BattleEngine.self._setWait (BattleEngine.BATTLESTATE.AdjustLineUp, BattleEngine.self.standardWaitTime);
		}
	}
}
