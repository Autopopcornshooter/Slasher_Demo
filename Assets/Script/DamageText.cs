using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.Rendering;
using System.Runtime.CompilerServices;

public class DamageText : MonoBehaviour
{

    private GameObject curr_target;
    [SerializeField]
    private float rePosition_up;
    [SerializeField]
    private float moveForce;
    [SerializeField]
    private float lifespan;



    private RectTransform rectTransform;
    private Text UIText;
    private int temp_additional=0;
    Vector2 screenPosition;
    // Start is called before the first frame update
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        UIText = GetComponent<Text>();
    }
    void Start()
    {
       
    }
    public void ShootText(GameObject target, float Damage)
    {
        curr_target = target;
        StartCoroutine(DeadTime());
        UIText.text = Damage.ToString();
        rectTransform.position = screenPosition;
        UIText.DOFade(0, lifespan);
    }

    private void FixedUpdate()
    {
        screenPosition = Camera.main.WorldToScreenPoint(curr_target.transform.position);
        rectTransform.position = screenPosition+ Vector2.up*(rePosition_up+moveForce * temp_additional++);
        
    }
    IEnumerator DeadTime()
    {
        yield return new WaitForSecondsRealtime(lifespan);
        Destroy(gameObject);
    }
}
