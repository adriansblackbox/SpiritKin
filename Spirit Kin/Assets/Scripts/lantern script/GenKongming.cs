using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenKongming : MonoBehaviour
{
    /// <summary>
    /// 孔明灯预设
    /// </summary>
    public GameObject lanternObj;

    private Transform trans;
    private Transform selfTrans;

    

    void Start()
    {
        selfTrans = transform;


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
            yield return new WaitForSeconds(5);
        }
    }

    private void Update()
    {
        
    }
}
