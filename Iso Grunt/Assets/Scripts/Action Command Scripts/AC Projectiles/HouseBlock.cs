using UnityEngine;
using System.Collections;

public class HouseBlock : MonoBehaviour {

	public SpriteRenderer spRenderer;
	public Rigidbody rbody;
	public Material[] blockMats;

	HouseMaker myHouseMaker;

	public HouseMaker MyHouseMaker {
		get {
			return myHouseMaker;
		}
		set {
			myHouseMaker = value;
		}
	}

	int blockType;

	// Use this for initialization
	void Start ()
	{
		for( int i = 0; i < Random.Range(0, 3); i++)
		{
			_move();
		}

		blockType = Random.Range(0, blockMats.Length);
		spRenderer.material = blockMats[blockType];

		InvokeRepeating("_changeType", 0, .5f);
		InvokeRepeating("_move", 0, Random.Range(.2f, .6f));
		Invoke("_drop", 2f);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(myHouseMaker.CurrentBlock == this && Input.GetKeyDown("z"))
		{
			_drop();
		}
	}

	void OnCollisionEnter(Collision other)
	{
		HouseBlock otherBlock = other.gameObject.GetComponent<HouseBlock>();
		if(transform.position.y > other.gameObject.transform.position.y && otherBlock != null)
		{
			if(blockType > otherBlock.blockType)
			{
				myHouseMaker.BlockTypeCount[otherBlock.blockType] -= 1;
				Destroy(other.gameObject);
			}
		}
	}

	void _move()
	{
		if(transform.position.x < spRenderer.sprite.bounds.size.x)
		{
			transform.position += Vector3.right*spRenderer.sprite.bounds.size.x;
		}
		else
		{
			transform.position -= Vector3.right*spRenderer.sprite.bounds.size.x*2;
		}

	}

	void _changeType()
	{
		blockType += 1;
		if(blockType == blockMats.Length)
		{
			blockType = 0;
		}
		spRenderer.material = blockMats[blockType];
	}

	void _drop()
	{
		rbody.useGravity = true;
		rbody.velocity += Vector3.down * blockType * 2;
		myHouseMaker.BlockTypeCount[blockType] += 1;
		myHouseMaker.CurrentBlock = null;
		if(myHouseMaker != null)
		{
			myHouseMaker.Invoke("_makeBlock", .5f);
		}
		CancelInvoke("_changeType");
		CancelInvoke("_move");
		CancelInvoke("_drop");
	}
}
