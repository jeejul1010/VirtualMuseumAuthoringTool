using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class WallPlacement : MonoBehaviour
{
    public GameObject ToggleButton;

    RaycastHit hit;
    Vector3 movePoint;
    public bool moving = true;
    bool stretch = false;
    bool constructionMode = false;

    float startpt = 0;
    float endpt = 0;
    Vector3 walldirection = Vector3.forward;

    // Start is called before the first frame update
    void Start()
    {
/*        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 50000.0f, (1 << 8)))
        {
            transform.position = hit.point;
            transform.Translate(Vector3.up * 3.4f / 2.0f);
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        if (ToggleButton.activeSelf)
        {
            constructionMode = true;            
        }
        else
        {
            constructionMode = false;
        }

        if (constructionMode)
        {
            if (moving)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, 50000.0f, (1 << 8)))
                {
                    if (stretch)
                    {
                        endpt = Vector3.Dot(hit.point, walldirection);

                        var size = endpt - startpt;

                        if (size > 1)
                        {
                            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, size + 1);
                            if (walldirection == Vector3.forward)
                                transform.position = new Vector3(transform.position.x, transform.position.y, startpt + (size / 2.0f));
                            else
                                transform.position = new Vector3(startpt + (size / 2.0f), transform.position.y, transform.position.z);
                        }
                        else if (size < -1)
                        {
                            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, size - 1);
                            if (walldirection == Vector3.forward)
                                transform.position = new Vector3(transform.position.x, transform.position.y, startpt + (size / 2.0f));
                            else
                                transform.position = new Vector3(startpt + (size / 2.0f), transform.position.y, transform.position.z);
                        }
                    }
                    else
                    {
                        transform.position = hit.point;
                        transform.Translate(Vector3.up * 3.4f / 2.0f);
                    }

                    //transform.rotation = Quaternion.LookRotation(hit.normal);
                }

                if (Input.GetKeyDown(KeyCode.R))
                {
                    transform.Rotate(Vector3.up, 90);
                    if (walldirection == Vector3.forward)
                    {
                        walldirection = Vector3.right;
                    }
                    else
                    {
                        walldirection = Vector3.forward;
                    }
                }
                if (Input.GetMouseButtonDown(0))
                {
                    stretch = true;
                    startpt = Vector3.Dot(hit.point, walldirection);
                }
                if (Input.GetMouseButtonUp(0))
                {
                    //Instantiate(paintPrefab, transform.position, transform.rotation);
                    //Destroy(gameObject);
                    moving = false;
                    stretch = false;
                    this.gameObject.layer = 8;
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
                    if (Physics.Raycast(ray, out hit, 50000.0f, (1 << 8)))
                    {

                        if (this.gameObject == hit.transform.gameObject)
                        {
                            this.gameObject.layer = 0;
                            moving = true;
                        }
                    }
                }

            }
        }

    }
}
