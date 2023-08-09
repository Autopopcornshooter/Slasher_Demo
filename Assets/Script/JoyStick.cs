using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JoyStick : MonoBehaviour
{
    [SerializeField]
    private RectTransform joyStickBG;
    [SerializeField]
    private RectTransform joyStick;


    private RectTransform rectTransform;
    private float m_fRadius;
    private Vector2 m_fInputDir;
    private static JoyStick m_pinstance;

    CanvasScaler canvasScaler;
    public static JoyStick Instance()
    {
        return m_pinstance;
    }
    private void Awake()
    {
        InitiateReference();
    }
    private void InitiateReference()
    {
        canvasScaler = GameObject.Find("Canvas").GetComponent<CanvasScaler>();
        rectTransform = joyStickBG.GetComponent<RectTransform>();
        m_pinstance = this;
    }

    private void Start()
    {
        m_fRadius = rectTransform.sizeDelta.x / 2;
        joyStickBG.gameObject.SetActive(false);
        joyStick.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            joyStickBG.gameObject.SetActive(true);
            joyStick.gameObject.SetActive(true);

            Vector2 touchPosition = Input.mousePosition;
            Debug.Log("Touch Position (Screen): " + touchPosition);
            if (touchPosition.x + (joyStickBG.sizeDelta.x / 2) >= canvasScaler.referenceResolution.x)
            {
                touchPosition.x = canvasScaler.referenceResolution.x - (joyStickBG.sizeDelta.x / 2);
            }
            if (touchPosition.x - (joyStickBG.sizeDelta.x / 2) <= 0)
            {
                touchPosition.x = 0 + (joyStickBG.sizeDelta.x / 2);
            }

            if (touchPosition.y + (joyStickBG.sizeDelta.y / 2) >= canvasScaler.referenceResolution.y)
            {
                touchPosition.y = canvasScaler.referenceResolution.y - (joyStickBG.sizeDelta.y / 2);
            }
            if (touchPosition.y - (joyStickBG.sizeDelta.y / 2) <= 0)
            {
                touchPosition.y = 0 + (joyStickBG.sizeDelta.y / 2);
            }
            rectTransform.anchoredPosition = touchPosition;

            OnTouch(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            OnTouch(Input.mousePosition);
        }
        else
        {
            joyStick.localPosition = Vector2.zero;
            m_fInputDir = Vector2.zero;
            joyStickBG.gameObject.SetActive(false);
            joyStick.gameObject.SetActive(false);
        }
    }

    
    void OnTouch(Vector2 eventdata_pos)
    {
        m_fInputDir = eventdata_pos - rectTransform.anchoredPosition;
        m_fInputDir = Vector3.ClampMagnitude(m_fInputDir, m_fRadius);
        joyStick.localPosition = m_fInputDir;
    }
    public Vector3 GetMoveDir()
    {
        Vector3 moveDir = new Vector3(m_fInputDir.x,0,m_fInputDir.y);
        return moveDir / m_fRadius;
    }
}

