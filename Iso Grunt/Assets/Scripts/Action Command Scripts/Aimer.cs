using UnityEngine;
using System.Collections;

public class Aimer : MonoBehaviour {

	float rotationSpeed = 2f, maxAngle, minAngle, startAngle;

	// Use this for initialization
	void Start ()
	{
		transform.localRotation = Quaternion.Euler(new Vector3(0, 0, startAngle));
	}
	
	// Update is called once per frame
	void Update ()
	{
		transform.Rotate(0, 0, rotationSpeed);

		float negatableZAngle = transform.localEulerAngles.z;
		negatableZAngle = (negatableZAngle > 180) ? negatableZAngle - 360 : negatableZAngle;
		if((Mathf.Sign(rotationSpeed) == 1 && transform.localRotation.eulerAngles.z >= maxAngle) || (Mathf.Sign(rotationSpeed) != 1 && negatableZAngle <= minAngle))
		{
			rotationSpeed *= -1;
		}

		if(BattleEngine.self.currentAState == BattleEngine.ATTACKSTATE.ApplyAttack || BattleEngine.self.currentAState == BattleEngine.ATTACKSTATE.MovePostAction)
		{
			Destroy(gameObject);
		}
	}

	public void _setAimer(float givenRotSpeed, float givenMaxAng, float givenMinAng, float givenStartAng)
	{
		rotationSpeed = givenRotSpeed;
		maxAngle = givenMaxAng;
		minAngle = givenMinAng;
		startAngle = givenStartAng;
	}
}
