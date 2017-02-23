using UnityEngine;
using System.Collections;

public class PipeRat : MonoBehaviour {

	public SpriteRenderer spRenderer;
	public Rigidbody2D rBody;

	int damageValue = 1;
	float moveSpeed;

	// Use this for initialization
	void Start ()
	{
		moveSpeed = Random.Range(1.5f, 3f);
		rBody.velocity = transform.right * moveSpeed;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(spRenderer.isVisible == false)
		{
			Destroy(gameObject);
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		
		Debug.Log("pipeRat hit");
		BattleCharacter hitBC = other.gameObject.GetComponent<BattleCharacter>();

		if (hitBC != null && BattleEngine.self.targOpposed.Contains(hitBC))
		{
			BattleEngine.self._damageTarget(hitBC, damageValue);
		}
	}
}
