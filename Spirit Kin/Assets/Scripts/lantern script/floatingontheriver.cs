using UnityEngine;

public class floatingontheriver : MonoBehaviour
{
    //---------------
        // 绕Z轴的摇摆的速度
    float z_Speed = 1.0f;
    // 绕X轴的摇摆的速度
    float x_Speed = 1.0f;


    void Start()
    {

    }


    void Update()
    {

        //---------------------------------
        // 绕Z轴摇晃  
        if (this.transform.eulerAngles.z >= 0.1 && this.transform.eulerAngles.z <= 100)
        {
            z_Speed = -z_Speed;
        }
        else if (this.transform.eulerAngles.z <= (360 - 40) && this.transform.eulerAngles.z >= 100)
        {
            z_Speed = -z_Speed;
        }
        // 绕X轴摇晃  
        if (this.transform.eulerAngles.x >= 0.1 && this.transform.eulerAngles.x <= 100)
        {
            x_Speed = -x_Speed;
        }
        else if (this.transform.eulerAngles.x >= 100 && this.transform.eulerAngles.x <= (360 - 40))
        {
            x_Speed = -x_Speed;
        }
        this.transform.Rotate(z_Speed * Time.deltaTime, 0, z_Speed * Time.deltaTime);

    }
}
