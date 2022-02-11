using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curses : MonoBehaviour
{
    public List<Curse> curseArray = new List<Curse>();

    // Update is called once per frame
    void Update()
    {
        if(curseArray.Count > 0){
            CurseHandler();
        }
    }

    public void CurseHandler () {
        curseArray.ForEach(x =>
            {
                if(!x.isApplied) x.invokeCurse();
                if(x.removeFlag) {
                    x.removeCurse();
                    curseArray.Remove(x);
                }
            } 
        );
    }
}
