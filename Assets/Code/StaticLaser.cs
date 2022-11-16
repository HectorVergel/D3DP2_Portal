using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticLaser : MonoBehaviour
{
    public Laser m_Laser;
    

    // Update is called once per frame
    void Update()
    {
        m_Laser.ShootLaser();    
    }
}
