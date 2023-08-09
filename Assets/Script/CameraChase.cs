using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using static UnityEngine.GraphicsBuffer;

public class CameraChase : MonoBehaviour
{
   
   
    [SerializeField]
    private float shakeDuration;
    [SerializeField]
    private float shakeStrength;
    private void Start()
    {
    }

    // Update is called once per frame
    private void FixedUpdate()
    {


    }
    public void CamShake()
    {
        transform.DOShakePosition(shakeDuration,shakeStrength);
    }

}
