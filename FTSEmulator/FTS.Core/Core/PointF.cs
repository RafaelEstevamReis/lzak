﻿using System;

namespace FTS.Core
{
    public class PointF 
    {
        public float X { get; set; }
        public float Y { get; set; }

        public PointF() { }

        public PointF(float X, float Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }
}
