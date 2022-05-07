using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOnArrow : MonoBehaviour
{
    private SpriteRenderer SR;
    public Sprite PossibleArrow, LockArrow;
    private void Start() {
        SR = GetComponent<SpriteRenderer>();
        SR.sprite = PossibleArrow;
    }
    private void LateUpdate () {
        transform.LookAt(Camera.main.transform);
        transform.Rotate(0,180,0);
    }
    public void SetLockArrow() {
        SR.sprite = LockArrow;
    }
    public void SetPossibleArrow() {
        SR.sprite = PossibleArrow;
    }
    public void DestoryArrow() {
        Destroy(this.gameObject);
    }
}
