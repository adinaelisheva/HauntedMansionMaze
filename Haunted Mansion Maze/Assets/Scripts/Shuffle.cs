using UnityEngine;
using System.Collections;

public class Shuffle : MonoBehaviour {

    int speed, cycle, moves, i;

    float mult;

    void Start () {

        speed = 15; //smaller # = faster movement. essentially: move every x updates
        cycle = 5; //number of moves before motion reverses
        mult = 0.02f; //fraction of a meter per move
        moves = 0;
        i = 0;

    }

    void Update () {
        if(i == 0) {
            transform.Translate(Vector3.forward * mult);
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