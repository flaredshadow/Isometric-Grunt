using UnityEngine;
using System.Collections;

public class EchoBlast : MonoBehaviour {

	public float movementSpeed;

	float scalingSpeed = .001f;

	// Use this for initialization
	void Start ()
	{
		if(BattleEngine.self.bonus > 7)
		{
			scalingSpeed = .005f;
		}
		if(BattleEngine.self.bonus > 10)
		{
			scalingSpeed = .01f;
		}
	}
	
	// Update is called once per frame
	void Update () {
		transform.localScale += Vector3.one * scalingSpeed;
		transform.position -= Vector3.up * movementSpeed;

		if(transform.position.y < 0)
		{
			Destroy(gameObject);
		}
	}

	void OnTriggerEnter (Collider other)
	{
		BattleCharacter hitBC = other.GetComponent<BattleCharacter>();
		if(hitBC != null && BattleEngine.self.targOpposed.Contains(hitBC))
		{
			Dizzy effect = Instantiate (BattleEngine.self.statusEffectPrefab).AddComponent<Dizzy> ();
			effect.turns = 3;
			hitBC._addStatusEffect (effect);
			BattleEngine.self._damageTarget(hitBC, 1);
		}
	}
}
