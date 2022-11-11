using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Companion : MonoBehaviour
{
    bool m_IsAttached = false;
    Rigidbody m_Rigidbody;
    public float m_OffsetPortalTeleport = 2.0f;
    Portal m_ExitPortal;

    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }
    public void SetAttach(bool _Attach)
    {
        m_IsAttached = _Attach;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Portal")
        {
            if (!m_IsAttached)
            {
                Portal l_Portal = other.GetComponent<Portal>();
                if(l_Portal != m_ExitPortal)
                {
                    Teleport(l_Portal);
                   
                }
                
            }
           
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Portal")
        {
            if(other.GetComponent<Portal>() == m_ExitPortal)
            {
                m_ExitPortal = null;
            }
        }
    }

    void Teleport(Portal _Portal)
    {
        Vector3 l_LocalPosition = _Portal.m_OtherPortalTransform.InverseTransformPoint(transform.position);
        Vector3 l_Direction = _Portal.m_OtherPortalTransform.transform.InverseTransformDirection(transform.forward);

        Vector3 l_LocalVelocity = _Portal.m_OtherPortalTransform.transform.InverseTransformDirection(m_Rigidbody.velocity);
        Vector3 l_WorldVelocity = _Portal.m_MirrorPortal.transform.TransformDirection(l_LocalVelocity);

       

        m_Rigidbody.isKinematic = true;
        transform.forward = _Portal.m_MirrorPortal.transform.TransformDirection(l_Direction);
        Vector3 l_WorldVelocityNormalized = l_WorldVelocity.normalized;
        transform.position = _Portal.m_MirrorPortal.transform.TransformPoint(l_LocalPosition) + l_WorldVelocityNormalized * m_OffsetPortalTeleport;
        transform.localScale *= (_Portal.m_MirrorPortal.transform.localScale.x / _Portal.transform.localScale.x);
        m_Rigidbody.isKinematic = false;
        m_Rigidbody.velocity = l_WorldVelocity;
        m_ExitPortal = _Portal.m_MirrorPortal;
    }
}
