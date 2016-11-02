using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Transition : MonoBehaviour {

	public Image trImage;

	CreativeSpore.RpgMapEditor.TeleporterBehaviour actingTeleporter; // the teleporter that spawned this Transition
	GameObject actingPlayer; // the player, presumably singleton, that will be ultimately teleported

	float growthRate = .01f, growthDirection;
	bool isOpaque = false;

	// Use this for initialization
	void Start ()
	{
		// set image size to cover the whole screen
		(transform as RectTransform).sizeDelta = new Vector2(Screen.width, Screen.height);
		transform.SetParent(FindObjectOfType<Canvas>().transform, false);
	}
	
	// Update is called once per frame
	void Update ()
	{
		Color tempColor = trImage.color;
		tempColor.a += growthDirection;
		trImage.color = tempColor;

		// when the Transition is fully clear -> pop
		if((growthDirection < 0 && trImage.color.a <= 0))
		{
			Destroy(gameObject);
		}
		else if (growthDirection > 0 && trImage.color.a >= 1 && !isOpaque) // when the transition is fully dark
		{
			isOpaque = true;

			if(actingTeleporter != null)
			{
				actingTeleporter.DoTeleport( actingPlayer );
			}
		}
	}

	void OnDestroy()
	{
		if(growthDirection < 0)
		{
			if(SceneManager.GetActiveScene() == SceneManager.GetSceneByName(MainEngine.self.battleSceneName))
			{
				MainEngine.self._setState(MainEngine.playSME.Battling);
			}
			else
			{
				MainEngine.self._setState(MainEngine.playSME.Roaming);
			}
		}
	}

	public void initFadeToBlack(CreativeSpore.RpgMapEditor.TeleporterBehaviour givenTport, GameObject givenPlayer)
	{
		MainEngine.self.playState = MainEngine.playSME.Transitioning;
		actingTeleporter = givenTport;
		actingPlayer = givenPlayer;

		growthDirection = growthRate;

		Color tempColor = trImage.color;
		tempColor.a = 0;
		trImage.color = tempColor;
	}

	public void initUnFade()
	{
		growthDirection = -growthRate;

		Color tempColor = trImage.color;
		tempColor.a = 1;
		trImage.color = tempColor;
	}

	
}
