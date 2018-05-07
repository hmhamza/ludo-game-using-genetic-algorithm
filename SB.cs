using UnityEngine;
using System.Collections;

public class SB : MonoBehaviour {
	
	public Vector3 initialPos;
	public Vector3 pos;
	public int index;
	public bool isPug;
	public GameObject gameObj;
	public string color;
	
	void Start () {
		index=-1;
		isPug=false;
	}
	
	void Update () {
		
		rigidbody.freezeRotation = true;
		pos.y=(float)0.05;
		this.rigidbody.MovePosition(pos);
	}
	
	public void ChangePosition(Vector3 p){
		pos=p;
	}
	
	public void ResetPosition(){
		index=-1;
		pos=initialPos;
	}
}
