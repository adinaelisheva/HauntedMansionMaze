using UnityEngine;
using System.Collections;

public class HoverCredits : MonoBehaviour {

    public int moves;

    int speed, cycle, i;

    float mult;

    void Start () {

        speed = 1; //smaller # = faster movement. essentially: move every x updates
        cycle = 80; //number of moves before motion reverses
        mult = 0.03f; //fraction of a meter per move
        i = 0;

        int t = 0;
        while(t < moves) {
            t++;
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

