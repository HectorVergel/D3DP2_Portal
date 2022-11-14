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

    [Header("Crosshair")]
    public Image m_CurrentCrosshair;
    public Sprite m_CrosshairFull;
    public Sprite m_CrosshairEmpty;
    public Sprite m_CrosshairOrange;
    public Sprite m_CrosshairBlue;
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


    public void ChangeCrosshairState(CROSSHAIR_STATES _State)
    {
        switch (_State)
        {
            case CROSSHAIR_STATES.Empty:
                m_CurrentCrosshair.sprite = m_CrosshairEmpty;
                break;
            case CROSSHAIR_STATES.Full:
                m_CurrentCrosshair.sprite = m_CrosshairFull;
                break;
            case CROSSHAIR_STATES.Orange:
                m_CurrentCrosshair.sprite = m_CrosshairOrange;
                break;
            case CROSSHAIR_STATES.Blue:
                m_CurrentCrosshair.sprite = m_CrosshairBlue;
                break;

        }
    }
}


public enum CROSSHAIR_STATES
{
    Empty,
    Full,
    Orange,
    Blue
}




