using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Damage : MonoBehaviour {

	public static float popTime = 1.25f;

	public Text damageLabel;
	public Sprite healSprite;

	public int scaleDirection = 1;

	Vector3 scaleGrowth;

	// Use this for initialization
	void Start ()
	{
		Destroy(gameObject, popTime);
		if(scaleDirection < 0)
		{
			damageLabel.transform.Rotate(0, 180, 0);
		}
		scaleGrowth = new Vector3(.1f * scaleDirection, .1f, 0);
	}

	// Update is called once per frame
	void Update ()
	{
		if(Mathf.Abs(transform.localScale.x) < 1)
		{
			transform.localScale += scaleGrowth;
		}
	}

	public void _setToHeal()
	{
		GetComponent<Image>().sprite = healSprite;
		damageLabel.color = Color.white;
		damageLabel.rectTransform.anchoredPosition = new Vector2(0, -8); 
	}
}
