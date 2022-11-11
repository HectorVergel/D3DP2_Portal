using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyPortal : MonoBehaviour
{
    public List<Transform> m_ValidPoints;

    public float m_MinValidDistance = 0.3f;
    public float m_MaxValidDistance = 1.2f;
    public float m_MinDotValidAngle = 0.995f;


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
}
