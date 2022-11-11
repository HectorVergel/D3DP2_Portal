using UnityEngine;

public class RefractionCube : Laser
{
    public LineRenderer m_Laser;
    public LayerMask m_LaserLayerMask;
    public float m_MaxLaserDistance = 250.0f;

    bool m_RefractionEnabled;


    private void Update()
    {
        m_Laser.gameObject.SetActive(m_RefractionEnabled);
        m_RefractionEnabled = false;
    }
    public void CreateRefraction()
    {
        if (m_RefractionEnabled)
            return;

        ShootLaser(m_Laser,m_LaserLayerMask,m_MaxLaserDistance);
        m_RefractionEnabled = true;
    }
}




