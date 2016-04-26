using UnityEngine;
using System.Collections;

public class Final : MonoBehaviour {

	void Update () {
        if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetKeyDown("space")) {
            Application.Quit();
        }
	}
}
