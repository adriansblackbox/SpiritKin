using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private Vector3 velocity = Vector3.up;
    private Rigidbody rb;
    private Vector3 startPosition;
    private bool isFollowing = false;

    private int coinValue;

    void Start()
    {
        coinValue = Random.Range(1, 3);
        
        startPosition = this.transform.position;
        velocity *= Random.Range(4f, 6f);//random upward velocity
        velocity += new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));//random outward velocity

        rb = this.GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
    }

    void Update(){
        rb.position += velocity * Time.deltaTime;//update position

        Quaternion deltaRotation = Quaternion.Euler(new Vector3(Random.Range(-150f, 150f), Random.Range(150f, 250f), Random.Range(-150f, 150f)) * Time.deltaTime);//random rotation
        rb.MoveRotation(rb.rotation * deltaRotation);//slight rotation

        if(velocity.y <-4f){
            velocity.y = -4f;
            // Debug.Log("1");
        }else
        {
            velocity -= Vector3.up * 5 * Time.deltaTime;//gravity
            // Debug.Log("2");
        }

        if(Mathf.Abs(rb.position.y - startPosition.y) < 0.25f ){
            //remove all forces
            rb.velocity = Vector3.zero;
            //make the collider is trigger
            this.GetComponent<Collider>().isTrigger = true;

            isFollowing = true;
            

        }
        if(isFollowing){
            //fly to player
            
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(GameObject.Find("Player").transform.position.x, GameObject.Find("Player").transform.position.y+2, GameObject.Find("Player").transform.position.z), 100f * Time.deltaTime);
        }
    }

    //destroy the object when colliding with the player
    void OnTriggerEnter(Collider collision){
        if(collision.gameObject.tag == "Player"){
            collision.gameObject.GetComponent<PlayerStats>().addCoins(coinValue);
            Destroy(this.gameObject);
        }
    }
}
