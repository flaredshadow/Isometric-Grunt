using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

	[HideInInspector]
	public GameObject killTarget;

	[HideInInspector]
	public Vector3 tombStonePos;

	Animator anim;

	bool spawnTombStone = false;

	// Use this for initialization
	void Start ()
	{
		anim = GetComponent<Animator>();
		if(killTarget.GetComponent<PlayerHud>() != null)
		{
			spawnTombStone = true;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= .4f)
		{
			Destroy(killTarget);
			if(spawnTombStone == true)
			{
				Instantiate(BattleEngine.self.tombStonePrefab, tombStonePos, Quaternion.identity);
				spawnTombStone = false;
			}
		}

		if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
		{
			Destroy(gameObject);
		}
	}
}
