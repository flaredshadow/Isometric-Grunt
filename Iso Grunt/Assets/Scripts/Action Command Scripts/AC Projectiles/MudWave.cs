using UnityEngine;
using System.Collections;

public class MudWave : MonoBehaviour {

	public MeshRenderer mRenderer;

	float waveSpeed = .1f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = Vector3.MoveTowards(transform.position, transform.position+transform.right, waveSpeed);
		if(mRenderer.isVisible == false)
		{
			Destroy(gameObject);
			BattleEngine.self.attackSubState -= 1;
		}
	}
}
