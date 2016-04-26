using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {

    int speed, i, amt, rotation;

    void Start() {
        speed = 3; //smaller # = faster movement. essentially: move every x updates
        amt = 5; //degrees to rotate per update
        i = 0;
    }
	
	// Update is called once per frame
	void Update () {
        if(i == 0) {
            transform.rotation = Quaternion.AngleAxis(amt, Vector3.up) * transform.rotation;
        }
        i = (i+1)%speed;
	}
}
