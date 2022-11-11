using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Laser
{
    public LineRenderer m_Laser;
    public LayerMask m_LaserLayerMask;
    public float m_AlifeAngleInDegrees = 30.0f;

    public float m_MaxLaserDistance = 250.0f;
    void Update()
    {
        bool l_LaserAlife = Vector3.Dot(transform.up, Vector3.up) > Mathf.Cos(m_AlifeAngleInDegrees * Mathf.Deg2Rad);

        m_Laser.gameObject.SetActive(l_LaserAlife);

        if (l_LaserAlife)
        {
            ShootLaser(m_Laser, m_LaserLayerMask, m_MaxLaserDistance);
        }


    }
}

