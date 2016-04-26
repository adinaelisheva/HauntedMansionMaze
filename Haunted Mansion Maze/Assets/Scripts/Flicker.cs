using UnityEngine;
using System.Collections;

public class Flicker : MonoBehaviour {

    int speed, cycle, moves, i;

    float mult;

    void Start () {

        speed = 7; //smaller # = faster movement. essentially: move every x updates
        cycle = 3; //number of moves before motion reverses
        mult = 0.02f; //fraction of a meter per move
        moves = Random.Range(0,cycle);
        i = 0;

        //set up start position
        for(int j = 0; j < moves; j++) {
            transform.Translate(Vector3.up * mult);
        }   
        
    }

    void Update () {
        if(i == 0) {
            transform.Translate(Vector3.up * mult);
            moves++;
        }
        i = (i+1)%speed;
        if(moves > cycle) {
            //reverse direction and reset count
            moves = 0;
            mult *= -1;
        }
    }
}
