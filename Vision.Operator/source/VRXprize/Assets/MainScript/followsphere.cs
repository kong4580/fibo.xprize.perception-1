using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followsphere : MonoBehaviour
{
    public GameObject vrcam;
    public GameObject sphere;
    // Start is called before the first frame update
    void Start()
    {
        
        sphere.transform.position = vrcam.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        sphere.transform.position = vrcam.transform.position;
    }
}
