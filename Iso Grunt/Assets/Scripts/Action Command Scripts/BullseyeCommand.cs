using UnityEngine;
using System.Collections;

public class BullseyeCommand : MonoBehaviour {

	public SpriteRenderer spRenderer;
	public Rigidbody rBody;

	int durability;

	float destroyTime = -1f, verticalBounceSpeed, verticalMin, verticalMax;

	bool clickable, keepPostAction;

	// Use this for initialization
	void Start ()
	{
		if(clickable == true)
		{
			spRenderer.material = MainEngine.self.purpleSwapMat;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(verticalBounceSpeed > 0)
		{
			_bounceVertically();
		}

		if (BattleEngine.self.currentBState == BattleEngine.BATTLESTATE.PlayerAttack)
		{
			if (BattleEngine.self.currentAState == BattleEngine.ATTACKSTATE.ActionState)
			{
				if(destroyTime >= 0)
				{
					Destroy (gameObject, destroyTime);
				}
			}

			if (BattleEngine.self.currentAState == BattleEngine.ATTACKSTATE.ApplyAttack || BattleEngine.self.currentAState == BattleEngine.ATTACKSTATE.MovePostAction)
			{
				if(keepPostAction == false)
				{
					Destroy (gameObject);
				}
			}
		}
	}

	void OnMouseDown()
	{
		if(clickable == true && BattleEngine.self.currentBState == BattleEngine.BATTLESTATE.PlayerAttack && BattleEngine.self.currentAState == BattleEngine.ATTACKSTATE.ActionState)
		{
			durability -= 1;
			if(durability < 1)
			{
				Destroy(gameObject);
				if(spRenderer.material != MainEngine.self.darkGraySwapMat)
					BattleEngine.self.bonus += 1;
			}
		}
	}

	public void _beginVerticalBounce(float givenSpeed, float givenVMin, float givenVmax)
	{
		verticalBounceSpeed = givenSpeed;
		rBody.velocity = Vector3.up * verticalBounceSpeed;
		verticalMin = givenVMin;
		verticalMax = givenVmax;
	}

	void _bounceVertically()
	{
		bool shouldGoUp = transform.position.y <  verticalMin && rBody.velocity.y < 0;
		bool shouldGoDown = transform.position.y > verticalMax && rBody.velocity.y > 0;
		if(shouldGoUp || shouldGoDown)
		{
			rBody.velocity *= -1;
		}
	}

	public void _setAttributes(int givenDurability, float givenDestroyTime, bool givenClickable, bool givenKeepPostAction)
	{
		durability = givenDurability;
		destroyTime = givenDestroyTime;
		clickable = givenClickable;
		keepPostAction = givenKeepPostAction;
	}
}
