using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ActionCommand : MonoBehaviour
{
	public Sprite keyUpSprite, keyDownSprite;
	public Text keyLabel;

	protected Image commandImage;

	protected string actionKey;

	protected float destroyTime = -1, prePressWaitTime = 1f;

	protected bool destroyOnWrongKeyPress = true;

	protected int enemyBonus;

	// Use this for initialization
	void Start ()
	{
		if(BattleEngine.self.currentBState == BattleEngine.BATTLESTATE.EnemyAttack)
		{
			BattleEngine.self.bonus = enemyBonus;
			Destroy(gameObject);
			return;
		}

		transform.SetParent (BattleEngine.self.battleCanvas.transform, false);
		commandImage = GetComponent<Image> ();
		commandImage.sprite = keyUpSprite;
		keyLabel.text = actionKey;

		switch (actionKey)
		{
			case "z":
				break;
			case "x":
				commandImage.material = MainEngine.self.greenSwapMat;
				break;
			case "c":
				commandImage.material = MainEngine.self.blueSwapMat;
				break;
			case "v":
				commandImage.material = MainEngine.self.purpleSwapMat;
				break;
			case "right":
				commandImage.material = MainEngine.self.darkGraySwapMat;
				keyLabel.text = "➔";
				break;
			case "left":
				commandImage.material = MainEngine.self.darkGraySwapMat;
				keyLabel.text = "➔";
				keyLabel.transform.Rotate(new Vector3(0, 0, 180));
				break;
			case "up":
				commandImage.material = MainEngine.self.darkGraySwapMat;
				keyLabel.text = "➔";
				keyLabel.transform.Rotate(new Vector3(0, 0, 90));
				break;
			case "down":
				commandImage.material = MainEngine.self.darkGraySwapMat;
				keyLabel.text = "➔";
				keyLabel.transform.Rotate(new Vector3(0, 0, 270));
				break;
		}

		_childStart ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (commandImage.sprite == keyUpSprite)
		{
			keyLabel.rectTransform.anchoredPosition = new Vector2 (0, 8);
		}
		else
		{
			keyLabel.rectTransform.anchoredPosition = new Vector2 (0, 0);
		}

		if (BattleEngine.self.currentBState == BattleEngine.BATTLESTATE.PlayerAttack)
		{
			if (BattleEngine.self.currentAState == BattleEngine.ATTACKSTATE.ActionState)
			{
				if(destroyTime >= 0)
				{
					Destroy (gameObject, destroyTime);
				}
				_activeChildUpdate ();
			}

			if (BattleEngine.self.currentAState == BattleEngine.ATTACKSTATE.ApplyAttack || BattleEngine.self.currentAState == BattleEngine.ATTACKSTATE.MovePostAction)
			{
				Destroy (gameObject);
			}
		}
	}

	public virtual void _childStart ()
	{
		
	}

	public virtual void _activeChildUpdate ()
	{

	}

	public void _switchSprite ()
	{
		if (commandImage.sprite == keyUpSprite)
		{
			commandImage.sprite = keyDownSprite;
		}
		else
		{
			commandImage.sprite = keyUpSprite;
		}
	}

	public void _setAttributes(string givenKey, float givenDestroyTime, float givenPrePressWaitTime, bool givenDestroyOnWrongKeyPress, int givenEnemyBonus)
	{
		actionKey = givenKey;
		destroyTime = givenDestroyTime;
		prePressWaitTime = givenPrePressWaitTime;
		destroyOnWrongKeyPress = givenDestroyOnWrongKeyPress;
		enemyBonus = givenEnemyBonus;
	}

	void OnDestroy()
	{
		BattleEngine.self.commandsDestroyed += 1;
	}
}