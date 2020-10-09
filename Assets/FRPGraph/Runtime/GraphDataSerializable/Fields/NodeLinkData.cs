using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NodeLinkData
{
    public string ConnectionName;
    public string BaseNodeGuid;
    public string BasePortName;
    public int BasePortNum;
    public string TargetNodeGuid;
    public string TargetPortName;
    public int TargetPortNum;
}
