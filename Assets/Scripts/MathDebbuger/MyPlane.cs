using CustomMath;
using System;

namespace CustomMath
{
    public struct MyPlane : IEquatable<MyPlane>
    {
        #region Variables
        private float distance;
        private Vec3 normal;

        public float Distance { get { return distance; } }
        public Vec3 Normal { get { return normal; } }
        public readonly MyPlane flipped
        {
            get { return new MyPlane(-normal, distance); }
        }
        #endregion

        #region Constructors

        public MyPlane(Vec3 inNormal, Vec3 inPoint)
        {
            normal = inNormal.normalized;
            distance = Vec3.Dot(normal, inPoint);
        }

        public MyPlane(Vec3 inNormal, float d)
        {
            normal = inNormal.normalized;
            distance = d;
        }

        public MyPlane(Vec3 a, Vec3 b, Vec3 c)
        {
            normal = Vec3.Cross(b - a, c - a).normalized;
            distance = Vec3.Dot(normal, a);
        }

        #endregion

        #region Constants
        public const float epsilon = 1e-05f;
        public const float sqrEpsilon = epsilon * epsilon;
        #endregion

        #region Operators
        public static bool operator ==(MyPlane lhs, MyPlane rhs)
        {
            return lhs.normal == rhs.normal && (lhs.distance - rhs.distance) * (lhs.distance - rhs.distance) < sqrEpsilon;
        }

        public static bool operator !=(MyPlane lhs, MyPlane rhs)
        {
            return !(lhs == rhs);
        }
        #endregion

        #region Functions

        public static MyPlane Translate(MyPlane plane, Vec3 translation)
        {
            return new MyPlane(plane.normal, Vec3.Dot(translation, plane.normal) + plane.distance);
        }

        public Vec3 ClosestPointOnPlane(Vec3 point)
        {
            return (distance - Vec3.Dot(normal, point)) * normal + point;
        }

        public override bool Equals(object other)
        {
            if (!(other is MyPlane))
            {
                return false;
            }

            return Equals((MyPlane)other);
        }

        public bool Equals(MyPlane other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(normal, distance);
        }

        public void Flip()
        {
            normal = -normal;
        }

        public float GetDistanceToPoint(Vec3 point)
        {
            return Vec3.Dot(point, normal) - distance;
        }

        public bool GetSide(Vec3 point)
        {
            return Vec3.Dot(point, normal) >= distance;
        }

        public bool SameSide(Vec3 inPt0, Vec3 inPt1)
        {
            return GetSide(inPt0) == GetSide(inPt1);
        }

        public void Set3Points(Vec3 a, Vec3 b, Vec3 c)
        {
            normal = Vec3.Cross(b - a, c - a).normalized;
            distance = Vec3.Dot(normal, a);
        }

        public void SetNormalAndPosition(Vec3 inNormal, Vec3 inPoint)
        {
            normal = inNormal.normalized;
            distance = Vec3.Dot(normal, inPoint);
        }

        public void Translate(Vec3 translation)
        {
            distance += Vec3.Dot(translation, normal);
        }

        public override string ToString()
        {
            return "(normal:" + normal.ToString() + ", " + "distance:" + distance.ToString("F2") + ")";
        }
        #endregion
    }
}