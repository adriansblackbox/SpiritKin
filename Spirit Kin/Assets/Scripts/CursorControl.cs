using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorControl : MonoBehaviour
{
    private bool islocked;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        islocked = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (islocked)
            {
                Cursor.lockState = CursorLockMode.None;
                islocked = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                islocked = true;
            }
        }
    }
}
