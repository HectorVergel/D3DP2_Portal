using UnityEngine;

public class CompanionSpawner : MonoBehaviour
{
    public Companion m_CompanionPrefab;
    public Transform m_SpawnPosition;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            GameObject.Instantiate(m_CompanionPrefab, m_SpawnPosition.position, m_SpawnPosition.rotation);
        }
    }
}



