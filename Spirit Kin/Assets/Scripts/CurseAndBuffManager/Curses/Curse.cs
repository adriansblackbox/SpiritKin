using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Curse
{
    public string type;
    public Image image;
    public bool active;
    public bool isApplied;
    public bool removeFlag;

    public Curse () {}

    public virtual void invokeCurse () {}

    public virtual void removeCurse () {}
}
