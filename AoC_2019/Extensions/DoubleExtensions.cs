﻿using System;

namespace AoC_2019.Extensions
{
    public static class DoubleExtensions
    {
        const double Delta = 0.0000001;

        public static bool DoubleEquals(this double left, double right)
        {
            return Math.Abs(left - right) < Delta;
        }
    }
}