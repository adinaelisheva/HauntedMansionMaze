using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

    public int speed;
    public Setup global;

    private Vector3 moveDirection = Vector3.zero;
    private Vector3 forward = Vector3.zero;
    private Vector3 right = Vector3.zero;

    RaycastHit rayHitForward;

    GameObject rayHitForwardObject;    
    string rayHitString;  
    Ray rayForward;

    public AudioClip ghostSound;
    public AudioClip zombieSound;
    public AudioClip pickupSound;
    AudioSource soundEffectAudio;

    void Start() {
        soundEffectAudio = GetComponent<AudioSource>();
    }

	// Update is called once per frame
    void Update () {
        forward = transform.forward;
        right = new Vector3(forward.z, 0, -forward.x);

        var h = Input.GetAxisRaw("Horizontal");
        var v = Input.GetAxisRaw("Vertical");

        if(h != 0) {
            //rotate but don't move forward
            var dir = h * right + v * forward;
            moveDirection = Vector3.RotateTowards(moveDirection, dir, 200 * Mathf.Deg2Rad * Time.deltaTime, 1000);
            transform.rotation = Quaternion.LookRotation(moveDirection);
        } else if(v != 0) {
            //move forward only - no sideways motion
            var dir = v * forward;
            moveDirection = Vector3.RotateTowards(moveDirection, dir, 200 * Mathf.Deg2Rad * Time.deltaTime, 1000);
            var movement = moveDirection * Time.deltaTime * speed;

            //do raycasting to make sure we can go forward (aka, don't move through walls)
            rayForward = new Ray(transform.position, movement);
            bool move = true;
            if(Physics.Raycast(rayForward, out rayHitForward, 1.75f)) {
                if(rayHitForward.collider.gameObject.CompareTag("Wall")) {
                    move = false;
                }
            }
            if(move) {
                transform.position += movement;
            }

            global.showFootprints((int)transform.position.x, (int)transform.position.z);
        }



	}

    void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.CompareTag("Ghost") && global.mirrorCount > 0) {
            other.gameObject.SetActive(false);
            global.ghostCount--;
            global.mirrorCount--;
            soundEffectAudio.PlayOneShot(ghostSound);

        } else if(other.gameObject.CompareTag("Zombie") && global.flowerCount > 0) {
            other.gameObject.SetActive(false);
            global.zombieCount--;
            global.flowerCount--;
            soundEffectAudio.PlayOneShot(zombieSound);

        } else if(other.gameObject.CompareTag("Mirror")) {
            other.gameObject.SetActive(false);
            global.mirrorCount++;
            soundEffectAudio.PlayOneShot(pickupSound);

        } else if(other.gameObject.CompareTag("Flower")) {
            other.gameObject.SetActive(false);
            global.flowerCount++;
            soundEffectAudio.PlayOneShot(pickupSound);

        } else if(other.gameObject.CompareTag("WinTrigger")) {
            SceneManager.LoadScene("End");
        }
    }
}
