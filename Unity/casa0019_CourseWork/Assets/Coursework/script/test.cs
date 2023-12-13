using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public GameObject myPrefab;
    // Start is called before the first frame update
    void Start()
    {
        GameObject prefabInstance = Instantiate(myPrefab);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
