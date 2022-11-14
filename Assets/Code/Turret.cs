using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public Laser m_Laser;
    public float m_AlifeAngleInDegrees = 30.0f;

    //public AudioClip m_DieAudio;
    //public ParticleSystem m_DieParticles;

    void Update()
    {
        bool l_LaserAlife = Vector3.Dot(transform.up, Vector3.up) > Mathf.Cos(m_AlifeAngleInDegrees * Mathf.Deg2Rad);

        m_Laser.gameObject.SetActive(l_LaserAlife);

        if (l_LaserAlife)
        {
            m_Laser.ShootLaser();
        }


    }


    public void OnDie()
    {
        //Añadir particula y sonido
        Destroy(gameObject);
    }
}

