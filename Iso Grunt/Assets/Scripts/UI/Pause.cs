using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Reflection;

public class Pause : MonoBehaviour
{
	public static Pause self;
	public PlayerHud[] huds;
	public Text coinLabel;
	public Button useItemButton, saveButton;
	public Dropdown pauseItemDD;

	// Use this for initialization
	void Start ()
	{
		self = this;
		for(int i = 0; i < huds.Length; i++)
		{
			if(MainEngine.self.partySheets.Count-1 < i)
			{
				Destroy(huds[i].gameObject);
			}
			else
			{
				huds[i].sheet = MainEngine.self.partySheets[i];
			}
		}

		if(MainEngine.self.currentGameState != GAMESTATE.Paused)
		{
			Destroy(useItemButton.gameObject);
			Destroy(saveButton.gameObject);
		}
		else
		{
			pauseItemDD.AddOptions(MainEngine.self._battleItemsToOptions (false));
			//saveButton.onClick.AddListener(delegate {MainEngine.self._saveFile();});

			useItemButton.onClick.AddListener (
				delegate {
					Item currentSelectedItem = null;
					bool isOverride = false;

					if (MainEngine.self.playerUsableItems.Count > 0)
					{
						currentSelectedItem = MainEngine.self.playerUsableItems[pauseItemDD.value];
						MethodInfo mInfo = currentSelectedItem.itemAttack.GetType().GetMethod("_overworldFunction");
						isOverride = !mInfo.Equals(mInfo.GetBaseDefinition());
					}

					if (currentSelectedItem != null && isOverride == true)
					{
						bool depleteItemType = currentSelectedItem.amount == 1;
						currentSelectedItem.itemAttack._overworldFunction();
						MainEngine.self._removeItem (currentSelectedItem, 1);
						pauseItemDD.ClearOptions();
						pauseItemDD.AddOptions(MainEngine.self._battleItemsToOptions (false));
						if(depleteItemType == true)
						{
							pauseItemDD.value -= 1;
						}
					}
					else
					{
						MainEngine.self.audioSource.PlayOneShot (MainEngine.self.buzzClip);
					}
				});
		}
	}
	
	// Update is called once per frame
	void Update ()
	{			
		if(MainEngine.self.currentGameState == GAMESTATE.EnterScene)
		{
			Destroy(gameObject);
		}
		else
		{
			coinLabel.text = "Coins : " + MainEngine.self.playerCoins;

			if(MainEngine.self.currentGameState == GAMESTATE.Paused)
			{
				if(Input.GetKeyDown("p"))
				{
					Destroy(gameObject);
					Time.timeScale = 1;
					MainEngine.self.currentGameState = GAMESTATE.OverWorldPlay;
				}
			}
		}
	}

	void OnDestroy()
	{
		if(pauseItemDD != null)
		{
			pauseItemDD.Hide(); // needed to prevent leftover blockers
		}
	}
}
