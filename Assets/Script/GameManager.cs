using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int curr_Time;

    private static GameManager p_instance;
    private void Awake()
    {
        if(p_instance == null)
        {
            p_instance = this;
        }
    }
    public static GameManager Instance()
    {
        return p_instance;
    }
    // Start is called before the first frame update
    void Start()
    {
        curr_Time = 0;
        StartCoroutine(TimeProcess());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public int GetTime()
    {
        return curr_Time;
    }
   IEnumerator TimeProcess()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(1.0f);
            curr_Time++;
        }
    }
}
