using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ColorChanger : NetworkBehaviour
{
    private MeshRenderer _mr;

    // Start is called before the first frame update
    void Start()
    {
        _mr = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        ChangeColor(Color.red);
        RpcChangeColor(Color.red);
    }

    private void OnTriggerExit(Collider other)
    {
        ChangeColor(Color.yellow);
        RpcChangeColor(Color.yellow);
    }

    void ChangeColor(Color c)
    {
        _mr.material.color = c;
    }
    
    [ClientRpc]
    void RpcChangeColor(Color c)
    {
        ChangeColor(c);
    }
}
