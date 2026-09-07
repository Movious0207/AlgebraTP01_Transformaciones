using UnityEngine;
using CustomMath;
using System;
using Unity.VisualScripting;

namespace CustomMath
{
    public struct Quat : IEquatable<Quat>
    {
        #region Variables
        public float x;
        public float y;
        public float z;
        public float w;

        private const float epsilon = 1e-05f;
        #endregion

        #region Properties
        public static Quat identity
        {
            get
            {
                return new Quat(0,0,0,1);
            }
        }
        public Vec3 eulerAngles
        {
            get
            {
                float x = Mathf.Atan2(2*(this.w*this.x + this.y*this.z),1-2 *(this.x*this.x + this.y * this.y));
                float y = Mathf.Asin(2 * (this.w * this.y - this.x * this.z));
                float z = Mathf.Atan2(2*(this.w*this.z + this.x*this.y),1-2 *(this.y*this.y + this.z * this.z));
                return new Vec3();
            }
        }
        public Quat normalized
        {
            get
            {
                float length = Mathf.Sqrt(w * w + x * x + y * y + z * z);
        
                if (length == 0f) return identity;

                return new Quat(x / length, y / length, z / length, w / length);
            }
        }
        #endregion

        #region Constructors 
        public Quat (float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
    
        public Quat (Quat quat)
        {
            this.x = quat.x;
            this.y = quat.y;
            this.z = quat.z;
            this.w = quat.w;
        }

        public Quat (Quaternion quat)
        {
            this.x = quat.x;
            this.y = quat.y;
            this.z = quat.z;
            this.w = quat.w;
        }
        #endregion

        #region Operators
        public bool Equals(Quat other)
        {
            return x == other.x && y == other.y && z == other.z && w == other.w;
        }
        
        public static bool operator ==(Quat lhs, Quat rhs)
        {
            float difX = lhs.x - rhs.x;
            float difY = lhs.y - rhs.y;
            float difZ = lhs.z - rhs.z;
            float difW = lhs.w - rhs.w;

            return difX * difX + difY * difY + difZ * difZ + difW * difW < epsilon * epsilon;
        }
        public static bool operator !=(Quat lhs, Quat rhs)
        {
            return !(lhs == rhs);
        }
        public static Vec3 operator *(Quat rotation, Vec3 point)
        {
            float x = rotation.x * 2f;
            float y = rotation.y * 2f;
            float z = rotation.z * 2f;
    
            float xx = rotation.x * x;
            float yy = rotation.y * y;
            float zz = rotation.z * z;
    
            float xy = rotation.x * y;
            float xz = rotation.x * z;
            float yz = rotation.y * z;
    
            float wx = rotation.w * x;
            float wy = rotation.w * y;
            float wz = rotation.w * z;

            Vec3 result;
            result.x = (1f - (yy + zz)) * point.x + (xy - wz) * point.y + (xz + wy) * point.z;
            result.y = (xy + wz) * point.x + (1f - (xx + zz)) * point.y + (yz - wx) * point.z;
            result.z = (xz - wy) * point.x + (yz + wx) * point.y + (1f - (xx + yy)) * point.z;

            return result;
        }
        public static Quat operator *(Quat lhs, Quat rhs)
        {
            float w = lhs.w * rhs.w - lhs.x * rhs.x - lhs.y * rhs.y - lhs.z * rhs.z;
            float x = lhs.w * rhs.x + lhs.x * rhs.w + lhs.y * rhs.z - lhs.z * rhs.y;
            float y = lhs.w * rhs.y - lhs.x * rhs.z + lhs.y * rhs.w + lhs.z * rhs.x;
            float z = lhs.w * rhs.z + lhs.x * rhs.y - lhs.y * rhs.x + lhs.z * rhs.w;
            return new Quat(x,y,z,w);
        }
        public static implicit operator Quaternion(Quat myQuat)
        {
            return new Quaternion(myQuat.x,myQuat.y,myQuat.z,myQuat.w);
        }
        public static implicit operator Quat(Quaternion quat)
        {
            return new Quat(quat.x,quat.y,quat.z,quat.w);
        }
        #endregion


        #region StaticMethods
        public static float Angle(Quat a, Quat b)
        {
            float min = Mathf.Min(Mathf.Abs(Dot(a,b)), 1.0f);
    
            return Mathf.Acos(min) * 2f * 57.29578f;
        }
        public static Quat AngleAxis(float angle, Vec3 axis)
        {
            angle = angle * (Mathf.PI / 180);
            return new Quat(axis.x * Mathf.Sin(angle/2),axis.y * Mathf.Sin(angle/2),axis.z * Mathf.Sin(angle/2),Mathf.Cos(angle/2));
        }
        public static float Dot(Quat a, Quat b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
        }
        public static Quat Euler(Vec3 euler)
        {
            return EulerRotation(new Vec3 { x = euler.x * (float)Math.PI / 180f, y = euler.y * (float)Math.PI / 180f, z = euler.z * (float)Math.PI / 180f });
        }

        public static Quat EulerAngles(Vec3 euler)
        {
            return Euler(euler);
        }

        public static Quat EulerRotation(Vec3 euler)
        {
            float cy = (float)Math.Cos(euler.y * 0.5);
            float sy = (float)Math.Sin(euler.y * 0.5);
            float cp = (float)Math.Cos(euler.x * 0.5);
            float sp = (float)Math.Sin(euler.x * 0.5);
            float cr = (float)Math.Cos(euler.z * 0.5);
            float sr = (float)Math.Sin(euler.z * 0.5);

            return new Quat(
                cy * cp * sr - sy * sp * cr,
                sy * cp * sr + cy * sp * cr,
                sy * cp * cr - cy * sp * sr,
                cy * cp * cr + sy * sp * sr
            );
        }
        public static Quat FromToRotation(Vec3 fromDirection, Vec3 toDirection)
        {
            Vec3 axis = Vec3.Cross(fromDirection.normalized, toDirection.normalized).normalized;
    
            float angle = Vec3.Angle(fromDirection, toDirection);
    
            return Quat.AngleAxis(angle, axis);
        }
        public static Quat Inverse(Quat rotation)
        {
            Quat normal = rotation.normalized;
            return new Quat(-normal.x,-normal.y,-normal.z,normal.w);
        }
        public static Quat Lerp(Quat a, Quat b, float t)
        {
            return LerpUnclamped(a,b,Mathf.Clamp01(t));
        }
        public static Quat LerpUnclamped(Quat a, Quat b, float t)
        {
            if (Dot(a,b) < 0.0f)
            {
                b.x = -b.x;
                b.y = -b.y;
                b.z = -b.z;
                b.w = -b.w;
            }

            float x = a.x + (b.x - a.x) * t;
            float y = a.y + (b.y - a.y) * t;
            float z = a.z + (b.z - a.z) * t;
            float w = a.w + (b.w - a.w) * t;

            float length = (float)System.Math.Sqrt(x * x + y * y + z * z + w * w);

            if (length > 0.0f)
            {
                return new Quat(x / length, y / length, z / length, w / length);
            }

            return Quat.identity;
        }
        public static Quat LookRotation(Vec3 forward)
        {
            return LookRotation(forward,Vec3.Up);
        }
        public static Quat LookRotation(Vec3 forward, Vec3 upwards)
        {
            // 1. Normalizar el vector de avance
            Vec3 z = forward.normalized;
    
            // 2. Calcular la derecha (Right) usando cross product
            Vec3 x = Vec3.Cross(upwards, z).normalized;
    
            // 3. Recalcular el arriba (Up) verdadero, ortogonal a los otros dos
            Vec3 y = Vec3.Cross(z, x);

            // 4. Crear la matriz de rotación 3x3 a partir de los vectores ortonormales
            // Matriz column-major
            float m00 = x.x, m01 = y.x, m02 = z.x;
            float m10 = x.y, m11 = y.y, m12 = z.y;
            float m20 = x.z, m21 = y.z, m22 = z.z;

            float trace = m00 + m11 + m22;
            Quat q = Quat.identity;

            // 5. Conversión de matriz a Cuaternión (según el estándar de motores 3D)
            if (trace > 0.0f)
            {
                float s = Mathf.Sqrt(trace + 1.0f);
                q.w = s * 0.5f;
                s = 0.5f / s;
                q.x = (m21 - m12) * s;
                q.y = (m02 - m20) * s;
                q.z = (m10 - m01) * s;
            }
            else
            {
                if ((m00 > m11) && (m00 > m22))
                {
                    float s = Mathf.Sqrt(1.0f + m00 - m11 - m22);
                    q.x = s * 0.5f;
                    s = 0.5f / s;
                    q.y = (m10 + m01) * s;
                    q.z = (m20 + m02) * s;
                    q.w = (m21 - m12) * s;
                }
                else if (m11 > m22)
                {
                    float s = Mathf.Sqrt(1.0f + m11 - m00 - m22);
                    q.y = s * 0.5f;
                    s = 0.5f / s;
                    q.x = (m10 + m01) * s;
                    q.z = (m21 + m12) * s;
                    q.w = (m02 - m20) * s;
                }
                else
                {
                    float s = Mathf.Sqrt(1.0f + m22 - m00 - m11);
                    q.z = s * 0.5f;
                    s = 0.5f / s;
                    q.x = (m20 + m02) * s;
                    q.y = (m21 + m12) * s;
                    q.w = (m10 - m01) * s;
                }
            }

            return q.normalized;

        }
        public static Quat Normalize(Quat q)
        {
            return q.normalized;
        }
        public static Quat RotateTowards(Quat from, Quat to, float maxDegreesDelta)
        {
            float angle = Quat.Angle(from, to);
    
            if (angle == 0.0f) 
            {
                return to;
            }

            float t = Mathf.Min(1.0f, maxDegreesDelta / angle);
    
            return Quat.SlerpUnclamped(from, to, t);
        }
        public static Quat Slerp(Quat a, Quat b, float t)
        {
            return SlerpUnclamped(a,b,Mathf.Clamp01(t));
        }
        public static Quat SlerpUnclamped(Quat a, Quat b, float t)
        {
            float dot = Quat.Dot(a, b);

            if (dot > 0.9995f)
            {
                Quat lerpQuat = new Quat(
                   a.x + (b.x - a.x) * t,
                    a.y + (b.y - a.y) * t,
                    a.z + (b.z - a.z) * t,
                    a.w + (b.w - a.w) * t
                );
                return lerpQuat.normalized;
            }

            if (dot < 0f)
            {
                b = new Quat(-b.x, -b.y, -b.z, -b.w);
                dot = -dot;
            }

            float theta = Mathf.Acos(dot);
            float sinTheta = Mathf.Sin(theta);

            if (Mathf.Abs(sinTheta) < 0.001f)
            {
                return a;
            }

            float ratioA = Mathf.Sin((1f - t) * theta) / sinTheta;
            float ratioB = Mathf.Sin(t * theta) / sinTheta;

            Quat result = new Quat(
                ratioA * a.x + ratioB * b.x,
                ratioA * a.y + ratioB * b.y,
                ratioA * a.z + ratioB * b.z,
                ratioA * a.w + ratioB * b.w
            );

            return result.normalized;
        }
        public static Vec3 ToEulerAngles(Quat rotation)
        {
            return rotation.eulerAngles;
        }
        #endregion

        #region InstanceMethods
        public void Normalize()
        {
            this = normalized;
        }
        public void Set(float newX, float newY, float newZ, float newW)
        {
            this.x = newX;
            this.y = newY;
            this.z = newZ;
            this.w = newW;
        }
        public void SetAxisAngle(Vec3 axis, float angle)
        {
            float halfAngle = angle * 0.5f;
            float s = (float)Math.Sin(halfAngle);
    
            this.x = axis.x * s;
            this.y = axis.y * s;
            this.z = axis.z * s;
            this.w = (float)Math.Cos(halfAngle);
        }
        public void SetEulerAngles(Vec3 euler)
        {
            this = Euler(euler);
        }
        public void SetEulerRotation(Vec3 euler)
        {
            this = EulerRotation(euler);
        }
        public void SetFromToRotation(Vec3 fromDirection, Vec3 toDirection)
        {
            this = FromToRotation(fromDirection, toDirection);
        }
        public void SetLookRotation(Vec3 view)
        {
            this = LookRotation(view, Vec3.Up);
        }
        public void SetLookRotation(Vec3 view, Vec3 up)
        {
            this = LookRotation(view, up);
        }
        public void ToAngleAxis(out float angle, out Vec3 axis)
        {
            Quat q = this.w > 1.0f ? this.normalized : this;
            
            angle = 2.0f * (float)Math.Acos(q.w) * Mathf.Rad2Deg;
            float s = (float)Math.Sqrt(1.0f - q.w * q.w);

            if (s < 0.001f) 
            {
                axis = Vec3.Right;
            }
            else 
            {
                axis = new Vec3(q.x / s, q.y / s, q.z / s);
            }
        }
        public Vec3 ToEuler()
        {
            Vec3 euler = Vec3.Zero;

            float sqw = this.w * this.w;
            float sqx = this.x * this.x;
            float sqy = this.y * this.y;
            float sqz = this.z * this.z;

            float unit = sqx + sqy + sqz + sqw; 
        
            float test = this.x * this.w - this.y * this.z;

            const float rad2Deg = 57.29578f;

            if (test > 0.4995f * unit)
            {
                euler.y = 2.0f * Mathf.Atan2(this.y, this.w) * rad2Deg;
                euler.x = 90.0f;
                euler.z = 0.0f;
                return euler;
            }
            if (test < -0.4995f * unit) 
            {
                euler.y = -2.0f * Mathf.Atan2(this.y, this.w) * rad2Deg;
                euler.x = -90.0f;
                euler.z = 0.0f;
                return euler;
            }

            euler.y = Mathf.Atan2(2.0f * (this.y * this.w + this.x * this.z), sqw - sqx - sqy + sqz) * rad2Deg;

            euler.x = Mathf.Asin(Mathf.Clamp(2.0f * test / unit, -1.0f, 1.0f)) * rad2Deg;
        
            euler.z = Mathf.Atan2(2.0f * (this.z * this.w + this.x * this.y), sqw - sqx + sqy - sqz) * rad2Deg;

            return euler;
        }
        #endregion

        
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode() ^ w.GetHashCode();
        }
    }
}