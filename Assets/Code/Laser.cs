using UnityEngine;

public class Laser : MonoBehaviour
{
    
    public void ShootLaser(LineRenderer _Laser, LayerMask _Mask, float _Distance)
    {
        Ray l_Ray = new Ray(_Laser.transform.position, _Laser.transform.forward);

        float l_LaserDistance = _Distance;
        RaycastHit l_RayHit;
        if (Physics.Raycast(l_Ray, out l_RayHit, l_LaserDistance, _Mask.value))
        {
            l_LaserDistance = Vector3.Distance(_Laser.transform.position, l_RayHit.point);

            if (l_RayHit.collider.tag == "RefractionCube")
            {
                l_RayHit.collider.GetComponent<RefractionCube>().CreateRefraction();
            }
            if(l_RayHit.collider.tag == "Portal")
            {
                
                l_RayHit.collider.GetComponent<Portal>().ShowLaser(l_RayHit.normal,l_RayHit.point, _Laser.transform.forward, true, _Distance);
            }
           
        }
        _Laser.SetPosition(1, new Vector3(0.0f, 0.0f, l_LaserDistance));
    }


}
