using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class InterfaceManager : MonoBehaviour
{
    [Header("References")]
    public Image m_DieImage;
    public GameObject m_ButtonRetry;
    [Header("Fade")]
    public float m_AlphaSpeed;
    private void Start()
    {
        GameController.GetGameController().SetInterface(this);
    }
    public void SetDieInterface()
    {
        Cursor.visible = true;
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float l_CurrentAlpha = 0.0f;

        while (m_DieImage.color.a <= 1.0f)
        {
            l_CurrentAlpha += m_AlphaSpeed * Time.deltaTime;
            m_DieImage.color = new Color(m_DieImage.color.r, m_DieImage.color.g, m_DieImage.color.b, l_CurrentAlpha);
            yield return null;
        }
        GameController.GetGameController().RestartGame();
        GameController.GetGameController().GetPlayer().m_CharacterController.enabled = false;
        yield return new WaitForSeconds(4f);
        m_ButtonRetry.SetActive(true);
    }

    IEnumerator FadeOut()
    {
        float l_CurrentAlpha = 1.0f;
        while (m_DieImage.color.a >= 0f)
        {
            l_CurrentAlpha -= m_AlphaSpeed * Time.deltaTime;
            m_DieImage.color = new Color(m_DieImage.color.r, m_DieImage.color.g, m_DieImage.color.b, l_CurrentAlpha);
            yield return null;
        }


    }

    public void OnRetryClick()
    {
        GameController.GetGameController().GetPlayer().m_CharacterController.enabled = true;
        Cursor.visible = false;
        StartCoroutine(FadeOut());
    }
}




