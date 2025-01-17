﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sardine : MonoBehaviour
{
    private int id;
    private FishManager fishManager = default;
    private List<Sardine> sardineList = default;

    // hold rigidbody to reduce calling GetComponent
    private Rigidbody2D _rigidbody2D;

    private void Start()
    {
        RandomizeSpeed();
    }

    private void FixedUpdate()
    {
        if (fishManager == null)
            return;

        var rb = GetRigidbody2D();

        Vector2 cf = GetCohesionForce();
        rb.AddForce(cf);

        Vector2 sxf = GetStrongXForce();
        rb.AddForce(sxf);

        Vector2 ksf = GetKeepSpeedForce();
        rb.AddForce(ksf);

        fixDirection();
    }

    public void RandomizeSpeed(bool useForce = true)
    {
        // init speed
        float ang = Random.Range(0, 359);
        float spd = ParamsSO.Entity.sardineSpeed;

        Vector2 v;
        v.x = Mathf.Cos(Mathf.Deg2Rad * ang) * spd;
        v.y = Mathf.Sin(Mathf.Deg2Rad * ang) * spd;

        if (useForce)
            GetRigidbody2D().AddForce(v);
        else
            GetRigidbody2D().velocity = v;
    }

    private Vector2 GetCohesionForce()
    {
        List<Sardine> sardines = fishManager.GetSardinesByRange(
            this, ParamsSO.Entity.cohesionRange);

        // Use 3 to deal with position.
        Vector3 centerPoint = new Vector2();
        foreach (Sardine sardine in sardines)
        {
            centerPoint += sardine.transform.position;
        }
        centerPoint /= sardines.Count;

        return (centerPoint - transform.position) * ParamsSO.Entity.cohesionPower;
    }

    private Vector2 GetStrongXForce()
    {
        Vector2 v = GetRigidbody2D().velocity;
        if (v.x < v.y * 2)
        {
            float xs = ParamsSO.Entity.sardineSpeed / 3 * this.GetDirectionX();
            v = new Vector2(xs, 0);
        }
        return v;
    }

    private Vector2 GetKeepSpeedForce()
    {
        Vector2 v = GetRigidbody2D().velocity;
        float ratio = ParamsSO.Entity.sardineSpeed - v.magnitude;

        return v * ratio;
    }

    // Flip Image by speed for x-axies
    private void fixDirection()
    {
        Vector2 scale = transform.localScale;
        float sx = transform.localScale.x;
        float vx = GetRigidbody2D().velocity.x;

        // ignore small speed to avoid flipping chain
        if (Mathf.Abs(vx) < 0.001)
            return;

        if (vx * sx < 0)
            return;

        //Change Direction
        scale.x = sx * -1;
        transform.localScale = scale;
    }

    public int GetID()
    {
        return this.id;
    }

    public void SetID(int _id)
    {
        this.id = _id;
    }

    public void SetFishManager(FishManager fm)
    {
        this.fishManager = fm;
    }

    public void SetSardineList(List<Sardine> _sardineList)
    {
        this.sardineList = _sardineList;
    }

    // Use local rigidbody. Avoid calling GetComponent()
    private Rigidbody2D GetRigidbody2D()
    {
        if (this._rigidbody2D == null)
            this._rigidbody2D = GetComponent<Rigidbody2D>();
        return this._rigidbody2D;
    }

    private float GetDirectionX()
    {
        float f = Mathf.Sign(GetRigidbody2D().velocity.x);
        if (f == 0)
            return 1f; // fix output -1 or 1
        return f;
    }
}