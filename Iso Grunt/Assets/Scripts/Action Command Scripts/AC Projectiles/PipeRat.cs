using UnityEngine;
using System.Collections;

public class PipeRat : MonoBehaviour {

	public SpriteRenderer spRenderer;
	public Rigidbody rBody;

	int moveSpeed = 5;

	// Use this for initialization
	void Start ()
	{
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
}
