using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class follow : MonoBehaviour
{
    public Transform Target;

    Vector3 _velocity = Vector3.zero;

    public float speed;

    public float MinModifier = 7;

    public float MaxModifier = 11;

    public bool _isFollowing = false;

    // private void OnTriggerStay(Collider other){
    //     if (other.CompareTag("Player")){
    //         _isFollowing = true;
    //     }
    // }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void StartFollowing()
    {
        // GetComponent<Animator>().
        _isFollowing = true;
    }

    // Update is called once per frame
    async void Update()
    {
        if (_isFollowing)
        {
            float step = speed * Time.deltaTime;
            transform.position =
                Vector3
                    .MoveTowards(transform.position,
                    new Vector3(Target.position.x,
                        Target.position.y + 10,
                        Target.position.z),
                    step);
        }
    }
}
