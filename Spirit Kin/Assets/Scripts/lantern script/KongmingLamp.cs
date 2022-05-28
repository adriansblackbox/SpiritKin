using UnityEngine;

public class KongmingLamp : MonoBehaviour
{
    private Transform trans;
    private Vector3 speed;


    void Start()
    {
        trans = transform;
        // 随机一个升空速度
        speed = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(1, 3), Random.Range(-0.5f, 0.5f));
    }


    void Update()
    {
        // 沿着速度方向升空
        trans.position += speed * Time.deltaTime;

        // 高度达到n，自我销毁
        if (trans.position.y > 1000)
        {
            Destroy(gameObject);
        }

    }
}
