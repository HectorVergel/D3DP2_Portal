using UnityEngine;

public class Door : MonoBehaviour
{

    public Animation m_MyAnimation;
    public AnimationClip m_OpenDoor;
    public AnimationClip m_CloseDoor;
    public bool m_Open = false;
    public void OpenDoor()
    {

        if (!m_Open)
        {
            Debug.Log("Open");
            m_MyAnimation.Play(m_OpenDoor.name);
            m_Open = true;

        }


    }

    public void CloseDoor()
    {
        if (m_Open)
        {
            Debug.Log("Close");
            m_MyAnimation.PlayQueued(m_CloseDoor.name);
            m_Open = false;

        }

    }
}
