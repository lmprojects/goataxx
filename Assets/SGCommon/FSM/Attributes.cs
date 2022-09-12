using UnityEngine;
using System.Collections;
using System;

public class UIEventTarget : Attribute 
{
    public Type TargetType { get; set; }
    public string TargetName { get; set; }

    public UIEventTarget(Type type, string name)
    {
        this.TargetType = type;
        this.TargetName = name;
    }
}
