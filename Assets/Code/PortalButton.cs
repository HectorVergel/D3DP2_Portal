using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PortalButton : MonoBehaviour
{
    public UnityEvent m_Action;
    public Door m_Door;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            m_Action.Invoke();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Companion")
        {
            m_Door.OpenDoor();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Companion")
        {
            m_Door.CloseDoor();
        }
    }
}
