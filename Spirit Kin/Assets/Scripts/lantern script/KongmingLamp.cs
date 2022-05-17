using UnityEngine;

public class KongmingLamp : MonoBehaviour
{
    
    private Vector3 speed;

    void Start()
    {
        // 随机一个升空速度
        speed = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(1, 3), Random.Range(-0.5f, 0.5f));

    }


    void Update()
    {
        // Ascending speed
        transform.position += speed * Time.deltaTime;

        // destroyed gameObject when reach to n
        if (transform.position.y > 102)
        {
            //lanternObj.SetActive(false);
            Destroy(gameObject);
        }
    }
}
