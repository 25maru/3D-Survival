﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

namespace cakeslice
{
    public class OutlineAnimation : MonoBehaviour
    {
        OutlineEffect outline;
        bool pingPong = false;

        // Use this for initialization
        void Start()
        {
            outline = GetComponent<OutlineEffect>();
        }

        // Update is called once per frame
        void Update()
        {
            Color c = outline.lineColor0;

            if(pingPong)
            {
                c.a += Time.deltaTime;

                if(c.a >= 1)
                    pingPong = false;
            }
            else
            {
                c.a -= Time.deltaTime;

                if(c.a <= 0)
                    pingPong = true;
            }

            c.a = Mathf.Clamp01(c.a);
            outline.lineColor0 = c;
            outline.lineColor2.a = c.a;
            outline.UpdateMaterialsPublicProperties();
        }
    }
}