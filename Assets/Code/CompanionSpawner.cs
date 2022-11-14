using UnityEngine;

public class CompanionSpawner : MonoBehaviour
{
    public Companion m_CompanionPrefab;
    public Transform m_SpawnPosition;

    public void SpawnCompanionCube()
    {

        GameObject.Instantiate(m_CompanionPrefab, m_SpawnPosition.position, m_SpawnPosition.rotation);

    }
}



