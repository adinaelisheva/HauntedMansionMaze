using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour {
    
	void Update () {
        if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetKeyDown("space")) {
            SceneManager.LoadScene("Main");
        }

	}
}
