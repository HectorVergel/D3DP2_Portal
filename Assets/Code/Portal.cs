using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{


    public Camera m_Camera;
    public Transform m_OtherPortalTransform;
    public Portal m_MirrorPortal;
    public Laser m_Laser;
    public FPSPlayerController m_Player;
    public float m_OffsetNearPlane;

    public List<Transform> m_ValidPoints;

    public float m_MinValidDistance = 0.3f;
    public float m_MaxValidDistance = 1.2f;
    public float m_MinDotValidAngle = 0.995f;

    private bool m_IsRefrecting;

    private void Start()
    {
        m_Laser.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (m_IsRefrecting)
        {
            m_MirrorPortal.m_Laser.gameObject.SetActive(true);
        }
        else
        {
            m_MirrorPortal.m_Laser.gameObject.SetActive(false);
        }
        m_IsRefrecting = false;
    }
    public bool IsValidPosition(Vector3 StartPosition, Vector3 forward, float MaxDistance, LayerMask PortalLayerMask, out Vector3 Position, out Vector3 Normal)
    {


        Ray l_Ray = new Ray(StartPosition, forward);
        RaycastHit l_RayCastHit;
        bool l_Valid = false;
        Position = Vector3.zero;
        Normal = Vector3.forward;

        if (Physics.Raycast(l_Ray, out l_RayCastHit, MaxDistance, PortalLayerMask.value))
        {
            if (l_RayCastHit.collider.tag == "DrawableWall")
            {
                
                l_Valid = true;
                Normal = l_RayCastHit.normal;
                Position = l_RayCastHit.point;
                transform.position = Position;
                transform.rotation = Quaternion.LookRotation(Normal);
            }

            for (int i = 0; i < m_ValidPoints.Count; i++)
            {
                Vector3 l_Direction = m_ValidPoints[i].position - StartPosition;
                l_Direction.Normalize();

                l_Ray = new Ray(StartPosition, l_Direction);

                if (Physics.Raycast(l_Ray, out l_RayCastHit, MaxDistance, PortalLayerMask.value))
                {
                    
                    if (l_RayCastHit.collider.tag == "DrawableWall")
                    {
                        
                        float l_Distance = Vector3.Distance(Position, l_RayCastHit.point);

                        float l_DotAngle = Vector3.Dot(Normal, l_RayCastHit.normal);
                        if (!(l_Distance >= m_MinValidDistance && l_Distance <= m_MaxValidDistance && l_DotAngle > m_MinDotValidAngle))
                        {
                            
                            l_Valid = false;
                        }
                    }

                    else
                        l_Valid = false;

                }
                else
                    l_Valid = false;
            }
        }

        return l_Valid;
    }
    private void LateUpdate()
    {

        Vector3 l_WorldPosition = m_Player.m_Camera.transform.position;
        Vector3 l_LocalPosition = m_OtherPortalTransform.InverseTransformPoint(l_WorldPosition);
        m_MirrorPortal.m_Camera.transform.position = m_MirrorPortal.transform.TransformPoint(l_LocalPosition);

        Vector3 l_WorldDirection = m_Player.m_Camera.transform.forward;
        Vector3 l_LocalDirection = m_OtherPortalTransform.InverseTransformDirection(l_WorldDirection);
        m_MirrorPortal.m_Camera.transform.forward = m_MirrorPortal.transform.TransformDirection(l_LocalDirection);

        float l_Distance = Vector3.Distance(m_MirrorPortal.m_Camera.transform.position, m_MirrorPortal.transform.position);
        m_MirrorPortal.m_Camera.nearClipPlane = l_Distance + m_OffsetNearPlane;

    }

    public void ShowLaser( Vector3 _LaserWorldPosition, Vector3 _LaserWorldDirection, float _Distance)
    {
       if(m_IsRefrecting) return;

        m_IsRefrecting = true;
        Vector3 l_LocalPosition = m_OtherPortalTransform.InverseTransformPoint(_LaserWorldPosition);
        m_MirrorPortal.m_Laser.transform.position = m_MirrorPortal.transform.TransformPoint(l_LocalPosition);

        Vector3 l_LocalDirection = m_OtherPortalTransform.InverseTransformDirection(_LaserWorldDirection);
        m_MirrorPortal.m_Laser.transform.forward = m_MirrorPortal.transform.TransformDirection(l_LocalDirection);

        m_MirrorPortal.m_Laser.m_LaserRenderer.SetPosition(1, new Vector3(0.0f, 0.0f, _Distance));
        float l_Distance;
        Plane l_Plane = new Plane(transform.forward, transform.position);
        Ray l_Ray = new Ray(_LaserWorldPosition, _LaserWorldDirection);
        l_Plane.Raycast(l_Ray, out l_Distance);

        m_MirrorPortal.m_Laser.transform.Translate(m_MirrorPortal.m_Laser.transform.forward * -l_Distance);
        m_MirrorPortal.m_Laser.ShootLaser();
        
        
    }

   
}
