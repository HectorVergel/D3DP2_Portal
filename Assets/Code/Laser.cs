using UnityEngine;

public class Laser : MonoBehaviour
{
    public LineRenderer m_LaserRenderer;
    public LayerMask m_LaserLayerMask;
    public float m_MaxLaserDistance = 250.0f;

    float m_Offset;

    public void ShootLaser()
    {
        Ray l_Ray = new Ray(m_LaserRenderer.transform.position + (transform.forward * m_Offset), m_LaserRenderer.transform.forward);

        float l_LaserDistance = m_MaxLaserDistance;
        RaycastHit l_RayHit;
        if (Physics.Raycast(l_Ray, out l_RayHit, l_LaserDistance, m_LaserLayerMask.value))
        {
            l_LaserDistance = Vector3.Distance(m_LaserRenderer.transform.position, l_RayHit.point);

            if (l_RayHit.collider.tag == "RefractionCube")
            {
                l_RayHit.collider.GetComponent<RefractionCube>().CreateRefraction();
            }
            if(l_RayHit.collider.tag == "Portal")
            {
                if(l_RayHit.collider.GetComponent<Portal>() != null && l_RayHit.collider.GetComponent<Portal>().m_MirrorPortal.gameObject.activeSelf)
                    l_RayHit.collider.GetComponent<Portal>().ShowLaser( l_RayHit.point, m_LaserRenderer.transform.forward, m_MaxLaserDistance);
            }
            if(l_RayHit.collider.tag == "Player")
            {
                l_RayHit.collider.GetComponent<FPSPlayerController>().OnDie();
            }
            if (l_RayHit.collider.tag == "Turret")
            {
                l_RayHit.collider.GetComponent<Turret>().OnDie();
            }

        }
        m_LaserRenderer.SetPosition(0, new Vector3(0.0f, 0.0f, m_Offset));
        m_LaserRenderer.SetPosition(1, new Vector3(0.0f, 0.0f, l_LaserDistance));
    }

    public void SetOffset(float _Offset)
    {
        m_Offset = _Offset;
    }
   

}
