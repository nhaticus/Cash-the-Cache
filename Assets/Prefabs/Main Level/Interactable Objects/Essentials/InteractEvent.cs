using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
 * Allows developers to set the Interact function to do anything
 * EX: Door Interact() opens
 * EX: Apple Interact() stolen
 */

public interface InteractEvent : IEventSystemHandler
{
    void Interact();
}
