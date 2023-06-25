using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacement : MonoBehaviour
{
    RaycastHit hit;
    Vector3 movePoint;
    public bool moving = true;
    // Start is called before the first frame update
    void Start()
    {
       /* Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 50000.0f, (1 << 8)))
        {
            transform.position = hit.point;
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 50000.0f, (1 << 8)))
            {
                transform.position = hit.point;
                transform.rotation = Quaternion.LookRotation(hit.normal);
            }

            if (Input.GetMouseButtonUp(0))
            {
                moving = false;
            }

            if (Input.GetMouseButton(1))
            {
                Destroy(gameObject);
            }    
        }
        else
        {
            if (Input.GetMouseButtonUp(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 50000.0f, (1<<0)))
                {
                    //Debug.Log("hitlayer" + hit.transform.gameObject.layer);

                    if (this.gameObject == hit.transform.gameObject)
                    {
                        moving = true;
                    }
                }
            }

        }

    }
}
