using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITriggerCheckable
{
    bool playerNoticed { get; set; }
    bool playerRecognized { get; set; }
    bool playerLost { get; set; }
}
