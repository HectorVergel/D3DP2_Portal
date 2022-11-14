using UnityEngine;

public class Door : MonoBehaviour
{

    public Animation m_MyAnimation;
    public AnimationClip m_OpenDoor;
    public AnimationClip m_CloseDoor;
    bool m_Open = false;
    public void OpenDoor()
    {
        if (!m_Open)
        {
            m_Open = true;
            m_MyAnimation.Play(m_OpenDoor.name);
        }
        
    }

    public void CloseDoor()
    {
        if (m_Open)
        {
            m_Open = true;
            m_MyAnimation.Play(m_CloseDoor.name);
        }
    }
}
