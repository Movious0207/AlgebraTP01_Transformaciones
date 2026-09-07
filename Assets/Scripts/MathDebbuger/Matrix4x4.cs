using CustomMath;
using System;
using Unity.VisualScripting;
using System.Numerics;

namespace CustomMath
{
    public struct Mat4x4 : IEquatable<Mat4x4>
    {
        #region Variables

        public float m00, m10, m20, m30; 
        public float m01, m11, m21, m31;
        public float m02, m12, m22, m32;
        public float m03, m13, m23, m33;

        private const float epsilon = 1e-05f;

        #endregion

        #region Properties
        public static Mat4x4 Zero
        {
            get
            {
                return new Mat4x4(Vector4.Zero,Vector4.Zero,Vector4.Zero,Vector4.Zero);
            }
        }
        public static Mat4x4 Identity
        {
            get
            {
                return new Mat4x4(
                new Vector4(1, 0, 0, 0),
                new Vector4(0, 1, 0, 0),
                new Vector4(0, 0, 1, 0),
                new Vector4(0, 0, 0, 1)
                );
            }
        }
        public Mat4x4 inverse
        {
          get
            {
                return Inverse(this);
            }  
        } 
        public float determinant
        {
          get
            {
                return Determinant(this);
            }  
        } 
        public bool isIdentity
        {
          get
            {
                return this == Identity;
            }  
        } 
        public Quat rotation
        {
          get
            {
                Vec3 x = new Vec3(this.m00, this.m10, this.m20).normalized;
                Vec3 y = new Vec3(this.m01, this.m11, this.m21).normalized;
                Vec3 z = new Vec3(this.m02, this.m12, this.m22).normalized;

                float m00 = x.x; float m01 = y.x; float m02 = z.x;
                float m10 = x.y; float m11 = y.y; float m12 = z.y;
                float m20 = x.z; float m21 = y.z; float m22 = z.z;

                float trace = m00 + m11 + m22;
                Quat q = Quat.identity;

                if (trace > 0.0f)
                {
                    float s = UnityEngine.Mathf.Sqrt(trace + 1.0f);
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
                        float s = UnityEngine.Mathf.Sqrt(1.0f + m00 - m11 - m22);
                        q.x = s * 0.5f;
                        s = 0.5f / s;
                        q.y = (m10 + m01) * s;
                        q.z = (m20 + m02) * s;
                        q.w = (m21 - m12) * s;
                    }
                    else if (m11 > m22)
                    {
                        float s = UnityEngine.Mathf.Sqrt(1.0f + m11 - m00 - m22);
                        q.y = s * 0.5f;
                        s = 0.5f / s;
                        q.x = (m10 + m01) * s;
                        q.z = (m21 + m12) * s;
                        q.w = (m02 - m20) * s;
                    }
                    else
                    {
                        float s = UnityEngine.Mathf.Sqrt(1.0f + m22 - m00 - m11);
                        q.z = s * 0.5f;
                        s = 0.5f / s;
                        q.x = (m20 + m02) * s;
                        q.y = (m21 + m12) * s;
                        q.w = (m10 - m01) * s;
                    }
                }

                return q.normalized;
            }  
        }
        public Vec3 lossyScale
        {
          get
            {
                float scaleX = UnityEngine.Mathf.Sqrt(this.m00 * this.m00 + this.m10 * this.m10 + this.m20 * this.m20);
                float scaleY = UnityEngine.Mathf.Sqrt(this.m01 * this.m01 + this.m11 * this.m11 + this.m21 * this.m21);
                float scaleZ = UnityEngine.Mathf.Sqrt(this.m02 * this.m02 + this.m12 * this.m12 + this.m22 * this.m22);

                return new Vec3(scaleX, scaleY, scaleZ);
            }  
        } 
        public Mat4x4 transpose
        {
          get
            {
                Mat4x4 result = Mat4x4.Identity;

                result.m00 = this.m00;
                result.m10 = this.m01;
                result.m20 = this.m02;
                result.m30 = this.m03;

                result.m01 = this.m10;
                result.m11 = this.m11;
                result.m21 = this.m12;
                result.m31 = this.m13;

                result.m02 = this.m20;
                result.m12 = this.m21;
                result.m22 = this.m22;
                result.m32 = this.m23;

                result.m03 = this.m30;
                result.m13 = this.m31;
                result.m23 = this.m32;
                result.m33 = this.m33;

                return result;
            }  
        } 
        #endregion

        #region Constructors 
        public Mat4x4(Vector4 column0, Vector4 column1, Vector4 column2, Vector4 column3)
        {
            m00 = column0.X; m10 = column0.Y; m20 = column0.Z; m30 = column0.W;
            m01 = column1.X; m11 = column1.Y; m21 = column1.Z; m31 = column1.W;
            m02 = column2.X; m12 = column2.Y; m22 = column2.Z; m32 = column2.W;
            m03 = column3.X; m13 = column3.Y; m23 = column3.Z; m33 = column3.W;
        }

        public Mat4x4(Matrix4x4 unityMatrix)
        {
            m00 = unityMatrix.M11; m10 = unityMatrix.M21; m20 = unityMatrix.M31; m30 = unityMatrix.M41;
            m01 = unityMatrix.M12; m11 = unityMatrix.M22; m21 = unityMatrix.M32; m31 = unityMatrix.M42;
            m02 = unityMatrix.M13; m12 = unityMatrix.M23; m22 = unityMatrix.M33; m32 = unityMatrix.M43;
            m03 = unityMatrix.M14; m13 = unityMatrix.M24; m23 = unityMatrix.M34; m33 = unityMatrix.M44;
        }
        #endregion

        #region Operators
        public static Vector4 operator *(Mat4x4 lhs, Vector4 vector)
            {
                return new Vector4(
                    lhs.m00 * vector.X + lhs.m01 * vector.Y + lhs.m02 * vector.Z + lhs.m03 * vector.W,
                    lhs.m10 * vector.X + lhs.m11 * vector.Y + lhs.m12 * vector.Z + lhs.m13 * vector.W,
                    lhs.m20 * vector.X + lhs.m21 * vector.Y + lhs.m22 * vector.Z + lhs.m23 * vector.W,
                    lhs.m30 * vector.X + lhs.m31 * vector.Y + lhs.m32 * vector.Z + lhs.m33 * vector.W
                );
            }

            public static Mat4x4 operator *(Mat4x4 lhs, Mat4x4 rhs)
            {
                Mat4x4 res;
                res.m00 = lhs.m00 * rhs.m00 + lhs.m01 * rhs.m10 + lhs.m02 * rhs.m20 + lhs.m03 * rhs.m30;
                res.m01 = lhs.m00 * rhs.m01 + lhs.m01 * rhs.m11 + lhs.m02 * rhs.m21 + lhs.m03 * rhs.m31;
                res.m02 = lhs.m00 * rhs.m02 + lhs.m01 * rhs.m12 + lhs.m02 * rhs.m22 + lhs.m03 * rhs.m32;
                res.m03 = lhs.m00 * rhs.m03 + lhs.m01 * rhs.m13 + lhs.m02 * rhs.m23 + lhs.m03 * rhs.m33;

                res.m10 = lhs.m10 * rhs.m00 + lhs.m11 * rhs.m10 + lhs.m12 * rhs.m20 + lhs.m13 * rhs.m30;
                res.m11 = lhs.m10 * rhs.m01 + lhs.m11 * rhs.m11 + lhs.m12 * rhs.m21 + lhs.m13 * rhs.m31;
                res.m12 = lhs.m10 * rhs.m02 + lhs.m11 * rhs.m12 + lhs.m12 * rhs.m22 + lhs.m13 * rhs.m32;
                res.m13 = lhs.m10 * rhs.m03 + lhs.m11 * rhs.m13 + lhs.m12 * rhs.m23 + lhs.m13 * rhs.m33;

                res.m20 = lhs.m20 * rhs.m00 + lhs.m21 * rhs.m10 + lhs.m22 * rhs.m20 + lhs.m23 * rhs.m30;
                res.m21 = lhs.m20 * rhs.m01 + lhs.m21 * rhs.m11 + lhs.m22 * rhs.m21 + lhs.m23 * rhs.m31;
                res.m22 = lhs.m20 * rhs.m02 + lhs.m21 * rhs.m12 + lhs.m22 * rhs.m22 + lhs.m23 * rhs.m32;
                res.m23 = lhs.m20 * rhs.m03 + lhs.m21 * rhs.m13 + lhs.m22 * rhs.m23 + lhs.m23 * rhs.m33;

                res.m30 = lhs.m30 * rhs.m00 + lhs.m31 * rhs.m10 + lhs.m32 * rhs.m20 + lhs.m33 * rhs.m30;
                res.m31 = lhs.m30 * rhs.m01 + lhs.m31 * rhs.m11 + lhs.m32 * rhs.m21 + lhs.m33 * rhs.m31;
                res.m32 = lhs.m30 * rhs.m02 + lhs.m31 * rhs.m12 + lhs.m32 * rhs.m22 + lhs.m33 * rhs.m32;
                res.m33 = lhs.m30 * rhs.m03 + lhs.m31 * rhs.m13 + lhs.m32 * rhs.m23 + lhs.m33 * rhs.m33;
                return res;
            }

            public static bool operator ==(Mat4x4 lhs, Mat4x4 rhs)
            { 
                return lhs.Equals(rhs);
            }
            public static bool operator !=(Mat4x4 lhs, Mat4x4 rhs)
            { 
                return !lhs.Equals(rhs);
            }

            public static implicit operator Matrix4x4(Mat4x4 m)
            {
                Matrix4x4 unityMat = new Matrix4x4();
                unityMat.M11 = m.m00; unityMat.M21 = m.m10; unityMat.M31 = m.m20; unityMat.M41 = m.m30;
                unityMat.M12 = m.m01; unityMat.M22 = m.m11; unityMat.M32 = m.m21; unityMat.M42 = m.m31;
                unityMat.M13 = m.m02; unityMat.M23 = m.m12; unityMat.M33 = m.m22; unityMat.M43 = m.m32;
                unityMat.M14 = m.m03; unityMat.M24 = m.m13; unityMat.M34 = m.m23; unityMat.M44 = m.m33;
                return unityMat;
            }

            public static implicit operator Mat4x4(Matrix4x4 m) 
            { 
                return new Mat4x4(m);
            }
        #endregion


        #region StaticMethods
        public static float Determinant(Mat4x4 m) 
        {
            float s0 = m.m00 * m.m11 - m.m10 * m.m01;
            float s1 = m.m00 * m.m21 - m.m20 * m.m01;
            float s2 = m.m00 * m.m31 - m.m30 * m.m01;
            float s3 = m.m10 * m.m21 - m.m20 * m.m11;
            float s4 = m.m10 * m.m31 - m.m30 * m.m11;
            float s5 = m.m20 * m.m31 - m.m30 * m.m21;

            float c0 = m.m02 * m.m13 - m.m12 * m.m03;
            float c1 = m.m02 * m.m23 - m.m22 * m.m03;
            float c2 = m.m02 * m.m33 - m.m32 * m.m03;
            float c3 = m.m12 * m.m23 - m.m22 * m.m13;
            float c4 = m.m12 * m.m33 - m.m32 * m.m13;
            float c5 = m.m22 * m.m33 - m.m32 * m.m23;

            return s0 * c5 - s1 * c4 + s2 * c3 + s3 * c2 - s4 * c1 + s5 * c0;
        }
        public static Mat4x4 Inverse(Mat4x4 m) 
        {
            float s0 = m.m00 * m.m11 - m.m10 * m.m01;
            float s1 = m.m00 * m.m21 - m.m20 * m.m01;
            float s2 = m.m00 * m.m31 - m.m30 * m.m01;
            float s3 = m.m10 * m.m21 - m.m20 * m.m11;
            float s4 = m.m10 * m.m31 - m.m30 * m.m11;
            float s5 = m.m20 * m.m31 - m.m30 * m.m21;

            float c0 = m.m02 * m.m13 - m.m12 * m.m03;
            float c1 = m.m02 * m.m23 - m.m22 * m.m03;
            float c2 = m.m02 * m.m33 - m.m32 * m.m03;
            float c3 = m.m12 * m.m23 - m.m22 * m.m13;
            float c4 = m.m12 * m.m33 - m.m32 * m.m13;
            float c5 = m.m22 * m.m33 - m.m32 * m.m23;

            float det = s0 * c5 - s1 * c4 + s2 * c3 + s3 * c2 - s4 * c1 + s5 * c0;

            float invDet = 1.0f / det;
            Mat4x4 res;

            res.m00 = (m.m11 * c5 - m.m21 * c4 + m.m31 * c3) * invDet;
            res.m01 = (-m.m01 * c5 + m.m21 * c2 - m.m31 * c1) * invDet;
            res.m02 = (m.m01 * c4 - m.m11 * c2 + m.m31 * c0) * invDet;
            res.m03 = (-m.m01 * c3 + m.m11 * c1 - m.m21 * c0) * invDet;

            res.m10 = (-m.m10 * c5 + m.m20 * c4 - m.m30 * c3) * invDet;
            res.m11 = (m.m00 * c5 - m.m20 * c2 + m.m30 * c1) * invDet;
            res.m12 = (-m.m00 * c4 + m.m10 * c2 - m.m30 * c0) * invDet;
            res.m13 = (m.m00 * c3 - m.m10 * c1 + m.m20 * c0) * invDet;

            res.m20 = (m.m13 * s5 - m.m23 * s4 + m.m33 * s3) * invDet;
            res.m21 = (-m.m03 * s5 + m.m23 * s2 - m.m33 * s1) * invDet;
            res.m22 = (m.m03 * s4 - m.m13 * s2 + m.m33 * s0) * invDet;
            res.m23 = (-m.m03 * s3 + m.m13 * s1 - m.m23 * s0) * invDet;

            res.m30 = (-m.m12 * s5 + m.m22 * s4 - m.m32 * s3) * invDet;
            res.m31 = (m.m02 * s5 - m.m22 * s2 + m.m32 * s1) * invDet;
            res.m32 = (-m.m02 * s4 + m.m12 * s2 - m.m32 * s0) * invDet;
            res.m33 = (m.m02 * s3 - m.m12 * s1 + m.m22 * s0) * invDet;

            return res;
        }

        public static Mat4x4 Transpose(Mat4x4 m)
        {
            Mat4x4 res = Zero;
            res.m00 = m.m00; res.m01 = m.m10; res.m02 = m.m20; res.m03 = m.m30;
            res.m10 = m.m01; res.m11 = m.m11; res.m12 = m.m21; res.m13 = m.m31;
            res.m20 = m.m02; res.m21 = m.m12; res.m22 = m.m22; res.m23 = m.m32;
            res.m30 = m.m03; res.m31 = m.m13; res.m32 = m.m23; res.m33 = m.m33;
            return res;
        }

        public static Mat4x4 LookAt(Vec3 from, Vec3 to, Vec3 up)
        {
            Vec3 zAxis = from-to;
            zAxis.Normalize();
            Vec3 xAxis = Vec3.Cross(up, zAxis);
            xAxis.Normalize();
            Vec3 yAxis = Vec3.Cross(zAxis, xAxis);

            Mat4x4 res = Identity;
            res.m00 = xAxis.x; res.m01 = xAxis.y; res.m02 = xAxis.z; res.m03 = -Vec3.Dot(xAxis, from);
            res.m10 = yAxis.x; res.m11 = yAxis.y; res.m12 = yAxis.z; res.m13 = -Vec3.Dot(yAxis, from);
            res.m20 = zAxis.x; res.m21 = zAxis.y; res.m22 = zAxis.z; res.m23 = -Vec3.Dot(zAxis, from);
            return res;
        }

        public static Mat4x4 Rotate(Quat q) 
        {
            Mat4x4 res = Identity;
            float x2 = q.x + q.x; float y2 = q.y + q.y; float z2 = q.z + q.z;
            float xx = q.x * x2;  float xy = q.x * y2;  float xz = q.x * z2;
            float yy = q.y * y2;  float yz = q.y * z2;  float zz = q.z * z2;
            float wx = q.w * x2;  float wy = q.w * y2;  float wz = q.w * z2;

            res.m00 = 1.0f - (yy + zz); res.m01 = xy - wz;          res.m02 = xz + wy;
            res.m10 = xy + wz;          res.m11 = 1.0f - (xx + zz); res.m12 = yz - wx;
            res.m20 = xz - wy;          res.m21 = yz + wx;          res.m22 = 1.0f - (xx + yy);
            return res;
        }

        public static Mat4x4 Scale(Vec3 vector) 
        {
            Mat4x4 res = Identity;
            res.m00 = vector.x;
            res.m11 = vector.y;
            res.m22 = vector.z;
            return res;
        }

        public static Mat4x4 Translate(Vec3 vector)
        {
            Mat4x4 res = Identity;
            res.m03 = vector.x;
            res.m13 = vector.y;
            res.m23 = vector.z;
            return res;
        }

        public static Mat4x4 TRS(Vec3 pos, Quat q, Vec3 s) 
        {
            Mat4x4 r = Rotate(q);
            Mat4x4 res;

            res.m00 = r.m00 * s.x; res.m01 = r.m01 * s.y; res.m02 = r.m02 * s.z; res.m03 = pos.x;
            res.m10 = r.m10 * s.x; res.m11 = r.m11 * s.y; res.m12 = r.m12 * s.z; res.m13 = pos.y;
            res.m20 = r.m20 * s.x; res.m21 = r.m21 * s.y; res.m22 = r.m22 * s.z; res.m23 = pos.z;
            res.m30 = 0.0f;        res.m31 = 0.0f;        res.m32 = 0.0f;        res.m33 = 1.0f;
            return res;
        }

        #endregion

        #region InstanceMethods
        public Vec3 GetPosition() 
        {
            return new Vec3(m03, m13, m23);
        }
        public Vector4 GetRow(int index)
        {
            switch (index)
            {
                case 0: return new Vector4(m00, m01, m02, m03);
                case 1: return new Vector4(m10, m11, m12, m13);
                case 2: return new Vector4(m20, m21, m22, m23);
                case 3: return new Vector4(m30, m31, m32, m33);
                default: return Vector4.Zero;
            }
        }

        public Vec3 MultiplyPoint(Vec3 point)
        {
            Vec3 res;
            float w;
            res.x = m00 * point.x + m01 * point.y + m02 * point.z + m03;
            res.y = m10 * point.x + m11 * point.y + m12 * point.z + m13;
            res.z = m20 * point.x + m21 * point.y + m22 * point.z + m23;
            w     = m30 * point.x + m31 * point.y + m32 * point.z + m33;
        
            w = 1.0f / w;
            res.x *= w;
            res.y *= w;
            res.z *= w;
            return res;
        }

        public Vec3 MultiplyPoint3x4(Vec3 point)
        {
            return new Vec3(
                m00 * point.x + m01 * point.y + m02 * point.z + m03,
                m10 * point.x + m11 * point.y + m12 * point.z + m13,
                m20 * point.x + m21 * point.y + m22 * point.z + m23
            );
        }

        public Vec3 MultiplyVector(Vec3 vector)
        {
            return new Vec3(
                m00 * vector.x + m01 * vector.y + m02 * vector.z,
                m10 * vector.x + m11 * vector.y + m12 * vector.z,
                m20 * vector.x + m21 * vector.y + m22 * vector.z
            );
        }

        public void SetColumn(int index, Vector4 column)
        {
            switch (index)
            {
                case 0: m00 = column.X; m10 = column.Y; m20 = column.Z; m30 = column.W; break;
                case 1: m01 = column.X; m11 = column.Y; m21 = column.Z; m31 = column.W; break;
                case 2: m02 = column.X; m12 = column.Y; m22 = column.Z; m32 = column.W; break;
                case 3: m03 = column.X; m13 = column.Y; m23 = column.Z; m33 = column.W; break;
            }
        }

        public void SetRow(int index, Vector4 row)
        {
            switch (index)
            {
                case 0: m00 = row.X; m01 = row.Y; m02 = row.Z; m03 = row.W; break;
                case 1: m10 = row.X; m11 = row.Y; m12 = row.Z; m13 = row.W; break;
                case 2: m20 = row.X; m21 = row.Y; m22 = row.Z; m23 = row.W; break;
                case 3: m30 = row.X; m31 = row.Y; m32 = row.Z; m33 = row.W; break;
            }
        }

        public void SetTRS(Vec3 pos, Quat q, Vec3 s)
        {
            this = TRS(pos, q, s);
        }

        public bool ValidTRS()
        {
            return UnityEngine.Mathf.Abs(determinant) > 1e-5f;
        }
        #endregion


            public bool Equals(Mat4x4 other)
            {
                return m00 == other.m00 && m10 == other.m10 && m20 == other.m20 && m30 == other.m30 &&
                       m01 == other.m01 && m11 == other.m11 && m21 == other.m21 && m31 == other.m31 &&
                       m02 == other.m02 && m12 == other.m12 && m22 == other.m22 && m32 == other.m32 &&
                       m03 == other.m03 && m13 == other.m13 && m23 == other.m23 && m33 == other.m33;
            }

            public override bool Equals(object obj) => obj is Mat4x4 other && Equals(other);

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 17;
                    hash = hash * 23 + m00.GetHashCode(); hash = hash * 23 + m01.GetHashCode();
                    hash = hash * 23 + m02.GetHashCode(); hash = hash * 23 + m03.GetHashCode();
                    return ((Matrix4x4)this).GetHashCode();
                }
            }

            public override string ToString()
            {
                return "hola";
            }
    }
}