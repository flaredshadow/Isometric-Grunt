using UnityEngine;
using System.Collections;

public class DizzyStars : MonoBehaviour {

	public Vector3 rotationSpeed;
	public float crashSpeed, endThresh;
	public GameObject[] stars;

	public BattleCharacter dizzyBattleCharacter;

	bool endPhase;

	// Use this for initialization
	void Start ()
	{
		Invoke("_end", 1f);
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(rotationSpeed);
		if(endPhase == true)
		{
			if (dizzyBattleCharacter.loseTurn == true)
			{
				foreach(GameObject starIter in stars)
				{
					starIter.transform.position = Vector3.MoveTowards(starIter.transform.position, dizzyBattleCharacter.transform.position, crashSpeed);
				}
				if(Vector3.Distance(stars[stars.Length-1].transform.position, dizzyBattleCharacter.transform.position) < endThresh )
				{
					Destroy(gameObject);
				}
			}
			else
			{
				Destroy(gameObject);
			}
		}	
	}

	void _end()
	{
		endPhase = true;
	}
}
