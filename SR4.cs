using UnityEngine;
using System.Collections;

public class SR4 : MonoBehaviour {

	public Vector3 pos;
	public int index;
	public string color;
	
	void Start () {
	
		pos=this.transform.position;
		index=-1;
		color="Red";
	}
	
	void Update () {
		this.rigidbody.MovePosition(pos);
	}
	
	public void ChangePosition(Vector3 p){
		var y = this.transform.position.y ;
		pos=p;
		pos.y=y;
	}
}
