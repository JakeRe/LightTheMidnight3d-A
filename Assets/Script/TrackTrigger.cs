using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TrackTrigger
{
    public delegate void OnTriggerExit(Collider other);
    private List<TriggerFrame> Triggers = new List<TriggerFrame>();
    private GameObject origin;

    public TrackTrigger(GameObject origin) 
    {
        this.origin = origin;
    }

    /// <summary>
    /// Called in "OnTriggerEnter" to start tracking the trigger
    /// </summary>
    /// <param name="other">The other collider passed into OnTriggerEvent</param>
    /// <param name="callback">The "OnTriggerExit" function</param>
    public void AddTrigger(Collider other, OnTriggerExit callback) 
    {
        if (!Contains(other)) 
        {
            Triggers.Add(new TriggerFrame(other, callback));
        }
    }

    /// <summary>
    /// Hooked into the Physics Update loop. This will check the triggers to see if they need to be removed.
    /// </summary>
    public void Update() 
    {
        // Check Triggers. 
        foreach (var trigger in Triggers) 
        {
            // If they haven't been called last frame...
            if (trigger.calledLastFrame == false) 
            {
                // Register them for removal
                if (trigger.removed != true)
                {
                    // And run a callback
                    TriggerCallback(trigger);
                    trigger.removed = true;
                }
                else 
                {
                    // If they need to be removed and havent been, remove them here.
                    Triggers.Remove(trigger);
                }
            }
            // Set all triggers to false.
            trigger.calledLastFrame = false;
        }
    }

    /// <summary>
    /// Called in the OnTriggerStay loop. This lets us know the trigger still exists
    /// </summary>
    /// <param name="other"></param>
    public void TriggerUpdate(Collider other) 
    {
        // Set the trigger to True
        var tFrame = Find(other);
        if (tFrame != null)
        { 
            tFrame.calledLastFrame = true;
        }
    }

    /// <summary>
    /// Called in the OnTriggerExit method to let us know the trigger has been removed.
    /// </summary>
    /// <param name="other"></param>
    public void RemoveTrigger(Collider other) 
    {
        Triggers.RemoveAll(x => x.collider.Equals(other));
    }

    private void TriggerCallback(TriggerFrame frame) 
    {
        frame.callback.Invoke(frame.collider);
    }

    private TriggerFrame Find(Collider other) 
    {
        return Triggers.FirstOrDefault(x => x.collider.Equals(other));
    }

    private bool Contains(Collider other)
    {
        return Triggers.Any(x => x.collider.Equals(other));
    }

    private class TriggerFrame : System.IEquatable<TriggerFrame>
    {
        public Collider collider;
        public bool calledLastFrame;
        public bool removed;
        public OnTriggerExit callback;

        public TriggerFrame(Collider other, OnTriggerExit callback) 
        {
            collider = other;
            this.callback = callback;
            calledLastFrame = true;
            removed = false;
        }

        public bool Equals(TriggerFrame other)
        {
            return collider.Equals(other.collider);
        }
    }
}