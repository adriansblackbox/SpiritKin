using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lockable_Targets : MonoBehaviour
{
    public Transform Player;
    private List<GameObject> _possibleTargets = new List<GameObject>();
    private Vector3 _minDistance;
    private Transform _Target;
    void Update()
    {
    }

    public Transform AssessTarget(){
        if(_possibleTargets == null)
            return null;
        if(_possibleTargets.Count.Equals(0))
            return null;
        _minDistance = new Vector3(10000f, 10000f, 10000f);
        for(int i = 0; i < _possibleTargets.Count; i++){
            if(( FindObjectOfType<Player_Controller>().CinemachineCameraTarget.transform.forward.normalized - (_possibleTargets[i].transform.position - Player.transform.position).normalized).magnitude < _minDistance.magnitude){
                _minDistance = _possibleTargets[i].transform.position - Player.transform.position;
                _Target = _possibleTargets[i].transform;
            }
        }
        return _Target;
    }
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Enemy")){
            _possibleTargets.Add(other.gameObject);
        }
    }
    private void OnTriggerExit(Collider other) {
        if(other.gameObject.CompareTag("Enemy")){
           _possibleTargets.Remove(other.gameObject);
        }
    }
}
