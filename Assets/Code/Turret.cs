using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public Laser m_Laser;
    public float m_AlifeAngleInDegrees = 30.0f;

    private Vector3 m_StartPosition;
    private Quaternion m_StartRotation;

    //public AudioClip m_DieAudio;
    public ParticleSystem m_DieParticles;
    bool m_LaserAlife;
    private void Start()
    {
        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;
    }
    void Update()
    {
        if(m_Laser != null)
        {
            m_LaserAlife = Vector3.Dot(transform.up, Vector3.up) > Mathf.Cos(m_AlifeAngleInDegrees * Mathf.Deg2Rad);

            m_Laser.gameObject.SetActive(m_LaserAlife);

            if (m_LaserAlife)
            {
                m_Laser.ShootLaser();
            }
        }
        


    }

    public void RestartGame()
    {
        gameObject.SetActive(true);
        transform.position = m_StartPosition;
        transform.rotation = m_StartRotation;
    }
    public void OnDie()
    {
        ParticleSystem l_Particle = Instantiate(m_DieParticles, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
        Destroy(l_Particle.gameObject, 0.45f);
    }


    public void DisableTurret()
    {
        Debug.Log("DISABLE");
        Destroy(m_Laser.gameObject);
        m_LaserAlife = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Turret")
        {
            DisableTurret();
        }
    }

}

