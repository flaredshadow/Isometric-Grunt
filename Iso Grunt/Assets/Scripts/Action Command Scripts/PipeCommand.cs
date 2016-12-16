using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PipeCommand : MonoBehaviour {

	int maxTries = 7;
	float waitTimeBetweenKeys = .5f;
	string[] keys = {"z", "x", "c", "v"};

	// Use this for initialization
	void Start ()
	{
		transform.SetParent (BattleEngine.self.battleCanvas.transform, false);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(BattleEngine.self.currentAState == BattleEngine.ATTACKSTATE.ActionState && !IsInvoking("makePressCommand"))
		{
			if (!FindObjectOfType<PressCommand>())
			{
				if(BattleEngine.self.commandsDestroyed < maxTries)
				{
					Invoke("makePressCommand", waitTimeBetweenKeys);
				}
				else
				{
					Destroy(gameObject);
				}
			}
		}
	}

	void OnDestroy()
	{
		BattleEngine.self._setWait(BattleEngine.ATTACKSTATE.ActionState, 1f);
	}

	void makePressCommand()
	{
		float holeSpacing = 16f;
		PressCommand command = Instantiate(BattleEngine.self.pressCommandPrefab).GetComponent<PressCommand>();
		(command.transform as RectTransform).anchoredPosition = ((transform as RectTransform).rect.xMin + BattleEngine.self.commandsDestroyed*holeSpacing) * Vector2.right;
		command._setAttributes(keys[Random.Range(0, keys.Length)], 3f, .75f, true, Random.Range(BattleEngine.self.bonus, BattleEngine.self.bonus+2));
	}
}
