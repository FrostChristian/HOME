using UnityEngine;
using System.Collections;

namespace BuildingConstructor {
public class RotateBC : MonoBehaviour {

	public float rotSpeed=3f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	private void Update () {
		transform.Rotate(new Vector3(0,rotSpeed*Time.deltaTime,0));

	}
}

}
