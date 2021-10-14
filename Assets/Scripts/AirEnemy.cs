using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirEnemy : Enemy
{

    // Update is called once per frame
    void Update()
    {
        RotateTo();
        MoveTo();
        Fly();
    }

    protected void Fly()
    {
        float flySpeed = 0;
        if (this.transform.position.y < 2.0)
        {
            flySpeed = 1.0f;
        }

        this.transform.Translate(new Vector3(0, flySpeed * Time.deltaTime, 0));
    }
}
