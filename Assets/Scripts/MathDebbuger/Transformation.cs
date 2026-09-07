using System.Collections;
using UnityEngine;
using System;
namespace CustomMath
{
    public struct MyTransform : IEquatable<MyTransform>
    {

        public float x;
        public float y;
        public float z;
        public float w;


        #region constants
        public const float epsilon = 1e-05f;
        #endregion
    }
}