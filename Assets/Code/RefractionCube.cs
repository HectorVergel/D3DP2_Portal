using UnityEngine;

public class RefractionCube : MonoBehaviour
{
    public Laser m_Laser;
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

        m_Laser.ShootLaser();
        m_RefractionEnabled = true;
    }
}




