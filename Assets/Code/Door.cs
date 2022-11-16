using UnityEngine;

public class Door : MonoBehaviour
{

    public Animation m_MyAnimation;
    public AnimationClip m_OpenDoor;
    public AnimationClip m_CloseDoor;
    public bool m_Open = false;

    public Transform m_Chekcpoint;
    public void OpenDoor()
    {

        if (!m_Open)
        {
            m_MyAnimation.Play(m_OpenDoor.name);
            m_Open = true;

        }


    }

    public void CloseDoor()
    {
        if (m_Open)
        {
            m_MyAnimation.PlayQueued(m_CloseDoor.name);
            m_Open = false;

        }

    }

    void SetNewCheckpoint()
    {
        GameController.GetGameController().GetPlayer().m_CheckPoint = this.m_Chekcpoint;
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            SetNewCheckpoint();
        }
    }
}
