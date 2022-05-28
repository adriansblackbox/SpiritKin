using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenKongming : MonoBehaviour
{
    /// <summary>
    /// 孔明灯预设
    /// </summary>
    public GameObject lanternObj;
    /// <summary>
    /// 高空摄像机
    /// </summary>
    public GameObject cam2;

    private Transform selfTrans;
    /// <summary>
    /// 主摄像机的Transform
    /// </summary>
    private Transform camTrans;
    /// <summary>
    /// 主摄像机
    /// </summary>
    private Camera cam;
    

    void Start()
    {
        selfTrans = transform;
        cam = Camera.main;
        camTrans = cam.transform;

        //初始创建n个孔明灯
        for (int i = 0; i < 5; ++i)
        {
            var go = Instantiate(lanternObj);
            go.transform.position = new Vector3(Random.Range(-100, 100), Random.Range(50, 100), Random.Range(-100, 100));
            go.transform.SetParent(selfTrans, false);
        }

        // 协程无限循环创建孔明灯
        StartCoroutine(StartGen());
    }


    private IEnumerator StartGen()
    {
        while (true)
        {
            var go = Instantiate(lanternObj);
            go.transform.position = new Vector3(Random.Range(-30, 30), 0, Random.Range(-30, 30));
            go.transform.SetParent(selfTrans, false);
            yield return new WaitForSeconds(1);
        }
    }

    private void Update()
    {


       
    }
}
