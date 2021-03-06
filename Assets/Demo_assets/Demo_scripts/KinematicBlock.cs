﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinematicBlock : MonoBehaviour
{
    // DO NOT SUKA USE CHILDREN WITH THEIR OWN RIGIDBODIES!!!!!!!

    [HideInInspector] public enum AngleType { Up = 1, Left = 2, Down = 3, Right = 0};

    [HideInInspector] public AngleType typeOfAngle = AngleType.Up;

    [HideInInspector] public float delay = 1000f;
    [HideInInspector] public float expansion = 2f;
    [HideInInspector] public float expansionTime = 1000f;

    public bool reverseAtEnd = false;
    public bool cycle = false;
    public float delayAtEnd = 500f;
    public bool mayKill = false;
    public float killDelay;
    public Transform killZoneTrfm;
    private KillZone killZone;
    private BoxCollider2D killZoneCol;

    public Transform[] startChain;
    public Transform[] endChain;

    private bool triggered = false;
    private bool finished = false;
    private bool delaying = false;
    private bool blocked = false;
    private float t;

    private Vector3 newPos, Pos;

    private Rigidbody2D rb;
    private Transform trfm;
    private BoxCollider2D bc2d;

    private bool direct = true;

    private bool killRoutine = false;
    private bool virginS = true;
    private bool virignE = true;

    // Use this for initialization
    void Start()
    {
        trfm = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        bc2d = GetComponent<BoxCollider2D>();
        if (mayKill)
        {
            killZone = killZoneTrfm.GetComponent<KillZone>();
            killZoneCol = killZoneTrfm.GetComponent<BoxCollider2D>();
            killZoneCol.enabled = false;
            killDelay /= 1000;
            killZone.kinematic = this;
        }
        expansionTime += 1;
        expansionTime /= 1000;
        delay /= 1000;
        delayAtEnd /= 1000;
    }

    void FixedUpdate()
    {
        if (triggered && !finished)
        {
            //rb.AddForce(direction, ForceMode2D.Force);
            //rb.AddForce(direction*2);
            t += Time.deltaTime;
            rb.MovePosition(Vector2.Lerp(Pos, newPos, t / expansionTime));

            if (/*trfm.position == newPos && trfm.rotation.eulerAngles == newRot ||*/ t >= expansionTime)
            {
                finished = true;
                delaying = false;
                // УБИВАНИЕ!!!!!! АРРРР
                if (mayKill && direct)
                {
                    killZone.killDelay = killDelay;
                    StartCoroutine(EnableKillZone(cycle ? delayAtEnd : 1000));
                }

                if (mayKill && !killZone.triggered || !mayKill)
                {
                    if (!cycle)
                        bc2d.enabled = true;
                    if (virignE)
                    {
                        virignE = false;
                        foreach (Transform t in endChain)
                            StartCoroutine(t.GetComponent<KinematicBlock>().StartAnim(false));
                    }
                    //endChain = new Transform[0];
                    if (reverseAtEnd)
                    {
                        t = 0;
                        if (typeOfAngle == AngleType.Up)
                        {
                            typeOfAngle = AngleType.Down;
                        }
                        else if (typeOfAngle == AngleType.Left)
                        {
                            typeOfAngle = AngleType.Right;
                        }
                        else if (typeOfAngle == AngleType.Down)
                        {
                            typeOfAngle = AngleType.Up;
                        }
                        else if (typeOfAngle == AngleType.Right)
                        {
                            typeOfAngle = AngleType.Left;
                        }

                        triggered = false;

                        if (direct)
                        {
                            StartCoroutine(StartAnim(true));
                        }
                        else if (cycle)
                        {
                            StartCoroutine(StartAnim(false));
                        }
                        direct = !direct;
                    }
                }
            }
        }
    }

    public void GentleStop()
    {
        StopAnim();


        //START
        foreach (Transform t in startChain)
                t.GetComponent<KinematicBlock>().GentleStop();

        //END
        foreach (Transform t in endChain)
                t.GetComponent<KinematicBlock>().GentleStop();
    }

    public void Block()
    {
        blocked = true;
    }

    public bool isTriggered()
    {
        return (triggered && delaying) && !finished && !blocked;
    }

    public void StopAnim()
    {
        StopAllCoroutines();
        finished = true;
    }

    public Vector3 getVectorAngle()
    {
        if (typeOfAngle == AngleType.Up)
        {
            return Vector3.up;
        }
        else if (typeOfAngle == AngleType.Left)
        {
            return Vector3.left;
        }
        else if (typeOfAngle == AngleType.Down)
        {
            return Vector3.down;
        }
        else// if (typeOfAngle == AngleType.Right)
        {
            return Vector3.right;
        }
    }

    IEnumerator EnableKillZone(float time)
    {
        if (!killRoutine)
        {
            killRoutine = true;
            killZoneCol.enabled = true;
            yield return new WaitForSeconds(time);
            killZoneCol.enabled = false;
        }
        killRoutine = false;
    }

    IEnumerator StartAnim(bool reverse)
    {
        delaying = true;
        if (!triggered)
        {
            bc2d.enabled = false;
            if (virginS)
            {
                virginS = false;
                foreach (Transform t in startChain)
                    StartCoroutine(t.GetComponent<KinematicBlock>().StartAnim(reverse));
            }
            //startChain = new Transform[0];
            if (reverse)
                yield return new WaitForSeconds(delayAtEnd);
            else yield return new WaitForSeconds(delay);

            Pos = trfm.position;
            newPos = Pos + getVectorAngle() * expansion;

            triggered = true;
            finished = false;
        }
        else
        {
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            StartCoroutine(StartAnim(false));
        }
    }
}

