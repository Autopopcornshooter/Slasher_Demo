using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteMap : MonoBehaviour
{

    private void Update()
    {
        Vector3 pos_deviation = PlayerCtrl.Instance().transform.position - transform.position;
        int x_dev= (int)(pos_deviation.x / 10);
        int z_dev= (int)(pos_deviation.z / 10);
        if (x_dev != 0)
        {
            transform.position += new Vector3((pos_deviation.x/10)*10,0,0);
        }
        if (z_dev != 0)
        {
            transform.position += new Vector3(0, 0, (pos_deviation.z / 10) * 10);
        }
    }

}
