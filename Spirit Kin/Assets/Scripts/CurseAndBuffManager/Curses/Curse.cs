using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Curse
{
    public bool isApplied = false;
    public bool removeFlag = false;

    public virtual void invokeCurse () {}

    public virtual void removeCurse () {}
}
