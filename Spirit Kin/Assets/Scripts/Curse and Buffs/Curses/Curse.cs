using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Curse
{
    public string type;
    public Sprite image;
    public bool active;
    public bool isApplied;
    public bool removeFlag;
    public float penaltyValue;

    //public Curse () {}

    public virtual void invokeCurse () {}

    public virtual void updateCurse () {}

    public virtual void updateCurse (float newValue) {}

    public virtual void removeCurse () {}
}
