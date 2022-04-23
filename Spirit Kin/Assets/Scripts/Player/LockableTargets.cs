using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockableTargets : MonoBehaviour
{
    public Transform Player;
    public List<GameObject> _possibleTargets = new List<GameObject>();
    private Vector3 _minDistance;
    private Transform _Target;
    public LayerMask EnemyLayer;
    void Update()
    {
    }
    public Transform AssessTarget(){
        //sphere cast and check enemies in cast
        //append each enemy into a list
        float rayLength = 200f;
        RaycastHit[] hit;
        hit = Physics.SphereCastAll(this.transform.position, 20f, FindObjectOfType<PlayerController>().CinemachineCameraTarget.transform.forward, rayLength, EnemyLayer);
        foreach(RaycastHit enemy in hit)
        {
            _possibleTargets.Add(enemy.transform.gameObject);
        }
        if(_possibleTargets == null)
            return null;
        if(_possibleTargets.Count.Equals(0))
            return null;
        return _possibleTargets[0].transform;
    }
    public void ClearTargetList(){
        _possibleTargets.Clear();
    }
}
