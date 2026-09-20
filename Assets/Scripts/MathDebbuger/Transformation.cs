using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Space
{
    World,
    Self
}

namespace CustomMath
{
    public class MyTransform : IEnumerable
    {
        #region Variables
        private Vec3 _localPosition = Vec3.Zero;
        private Quat _localRotation = Quat.identity;
        private Vec3 _localScale = Vec3.One;

        private MyTransform _parent = null;
        private readonly List<MyTransform> _children = new List<MyTransform>();
        #endregion

        #region Properties
        // --- Local Space ---
        public Vec3 localPosition
        {
            get
            {
                return _localPosition;
            }
            set
            {
                _localPosition = value;
                SetDirty();
            }
        }

        public Quat localRotation
        {
            get
            {
                return _localRotation;
            }
            set
            {
                _localRotation = value;
                SetDirty();
            }
        }

        public Vec3 localEulerAngles
        {
            get
            {
                return _localRotation.eulerAngles;
            }
            set
            {
                _localRotation = Quat.Euler(value);
            }
        }

        public Vec3 localScale
        {
            get
            {
                return _localScale;
            }
            set
            {
                _localScale = value;
                SetDirty();
            }
        }

        public Mat4x4 localToWorldMatrix
        {
            get
            {
                Mat4x4 localMatrix = Mat4x4.TRS(_localPosition, _localRotation, _localScale);
                if (_parent != null)
                {
                    return _parent.localToWorldMatrix * localMatrix;
                }
                return localMatrix;
            }
        }

        public Mat4x4 worldToLocalMatrix
        {
            get
            {
                return localToWorldMatrix.inverse;
            }
        }

        // --- World Space ---
        public Vec3 position
        {
            get
            {
                return localToWorldMatrix.MultiplyPoint3x4(Vec3.Zero);
            }
            set
            {
                if (_parent != null)
                {
                    _localPosition = _parent.worldToLocalMatrix.MultiplyPoint3x4(value);
                }
                else
                {
                    _localPosition = value;
                }
            }
        }

        public Quat rotation
        {
            get
            {
                if (_parent != null)
                {
                    return _parent.rotation * _localRotation;
                }
                return _localRotation;
            }
            set
            {
                if (_parent != null)
                {
                    _localRotation = Quat.Inverse(_parent.rotation) * value;
                }
                else
                {
                    _localRotation = value;
                }
            }
        }

        public Vec3 eulerAngles
        {
            get
            {
                return rotation.eulerAngles;
            }
            set
            {
                rotation = Quat.Euler(value);
            }
        }

        public Vec3 lossyScale
        {
            get
            {
                Mat4x4 m = localToWorldMatrix;
                return new Vec3(
                    new Vec3(m.m00, m.m10, m.m20).magnitude,
                    new Vec3(m.m01, m.m11, m.m21).magnitude,
                    new Vec3(m.m02, m.m12, m.m22).magnitude
                );
            }
        }

        // Directores de orientación
        public Vec3 forward
        {
            get
            {
                return rotation * Vec3.Forward;
            }
        }

        public Vec3 back
        {
            get
            {
                return rotation * Vec3.Back;
            }
        }

        public Vec3 up
        {
            get
            {
                return rotation * Vec3.Up;
            }
        }

        public Vec3 down
        {
            get
            {
                return rotation * Vec3.Down;
            }
        }

        public Vec3 right
        {
            get
            {
                return rotation * Vec3.Right;
            }
        }

        public Vec3 left
        {
            get
            {
                return rotation * Vec3.Left;
            }
        }

        // --- Hierarchy ---
        public MyTransform parent
        {
            get
            {
                return _parent;
            }
            set
            {
                SetParent(value, true);
            }
        }

        public MyTransform root
        {
            get
            {
                MyTransform current = this;
                while (current._parent != null)
                {
                    current = current._parent;
                }
                return current;
            }
        }

        public int childCount
        {
            get
            {
                return _children.Count;
            }
        }

        public int hierarchyCapacity { get; set; }

        public int hierarchyCount
        {
            get
            {
                return GetHierarchyCountRecursive(root);
            }
        }

        public bool hasChanged { get; set; }
        public string name { get; set; } = "MyTransform";
        #endregion

        #region Constructors
        public MyTransform() { }
        #endregion

        #region HierarchyMethods
        public void SetParent(MyTransform newParent, bool worldPositionStays = true)
        {
            if (_parent == newParent)
            {
                return;
            }

            Vec3 worldPos = position;
            Quat worldRot = rotation;
            Vec3 worldScale = lossyScale;

            if (_parent != null)
            {
                _parent._children.Remove(this);
            }

            _parent = newParent;

            if (_parent != null)
            {
                _parent._children.Add(this);
            }

            if (worldPositionStays)
            {
                position = worldPos;
                rotation = worldRot;
                if (_parent != null)
                {
                    Vec3 pScale = _parent.lossyScale;
                    _localScale = new Vec3(
                        pScale.x != 0 ? worldScale.x / pScale.x : 0,
                        pScale.y != 0 ? worldScale.y / pScale.y : 0,
                        pScale.z != 0 ? worldScale.z / pScale.z : 0
                    );
                }
                else
                {
                    _localScale = worldScale;
                }
            }

            SetDirty();
        }

        public MyTransform GetChild(int index)
        {
            return _children[index];
        }

        public int GetSiblingIndex()
        {
            return _parent != null ? _parent._children.IndexOf(this) : 0;
        }

        public void SetSiblingIndex(int index)
        {
            if (_parent == null)
            {
                return;
            }
            index = Math.Clamp(index, 0, _parent._children.Count - 1);
            _parent._children.Remove(this);
            _parent._children.Insert(index, this);
        }

        public void SetAsFirstSibling()
        {
            SetSiblingIndex(0);
        }

        public void SetAsLastSibling()
        {
            SetSiblingIndex(_parent != null ? _parent._children.Count - 1 : 0);
        }

        public bool IsChildOf(MyTransform parentTarget)
        {
            MyTransform current = this._parent;
            while (current != null)
            {
                if (current == parentTarget)
                {
                    return true;
                }
                current = current._parent;
            }
            return false;
        }

        public MyTransform Find(string name)
        {
            foreach (MyTransform child in _children)
            {
                if (child.name == name)
                {
                    return child;
                }
            }
            return null;
        }

        public void DetachChildren()
        {
            for (int i = _children.Count - 1; i >= 0; i--)
            {
                _children[i].SetParent(null, true);
            }
        }

        public IEnumerator GetEnumerator()
        {
            return _children.GetEnumerator();
        }
        #endregion

        #region TransformationMethods
        public void Translate(Vec3 translation, Space space = Space.Self)
        {
            if (space == Space.Self)
            {
                position += rotation * translation;
            }
            else
            {
                position += translation;
            }
        }

        public void Translate(float x, float y, float z, Space space = Space.Self)
        {
            Translate(new Vec3(x, y, z), space);
        }

        public void Translate(Vec3 translation, MyTransform relativeTo)
        {
            if (relativeTo != null)
            {
                position += relativeTo.TransformDirection(translation);
            }
            else
            {
                Translate(translation, Space.World);
            }
        }

        public void Translate(float x, float y, float z, MyTransform relativeTo)
        {
            Translate(new Vec3(x, y, z), relativeTo);
        }

        public void Rotate(Vec3 eulers, Space space = Space.Self)
        {
            Quat eulerRot = Quat.Euler(eulers);
            if (space == Space.Self)
            {
                _localRotation *= eulerRot;
            }
            else
            {
                rotation = eulerRot * rotation;
            }
        }

        public void Rotate(float xAngle, float yAngle, float zAngle, Space space = Space.Self)
        {
            Rotate(new Vec3(xAngle, yAngle, zAngle), space);
        }

        public void Rotate(Vec3 axis, float angle, Space space = Space.Self)
        {
            Quat q = Quat.AngleAxis(angle, axis);
            if (space == Space.Self)
            {
                _localRotation *= q;
            }
            else
            {
                rotation = q * rotation;
            }
        }

        public void RotateAround(Vec3 point, Vec3 axis, float angle)
        {
            Vec3 currentPos = position;
            Quat q = Quat.AngleAxis(angle, axis);
            Vec3 dir = currentPos - point;
            dir = q * dir;
            position = point + dir;
            Rotate(axis, angle, Space.World);
        }

        public void LookAt(MyTransform target, Vec3 worldUp)
        {
            if (target != null)
            {
                LookAt(target.position, worldUp);
            }
        }

        public void LookAt(MyTransform target)
        {
            LookAt(target, Vec3.Up);
        }

        public void LookAt(Vec3 worldPosition, Vec3 worldUp)
        {
            Vec3 forwardDir = worldPosition - position;
            if (forwardDir.sqrMagnitude > 0.00001f)
            {
                rotation = Quat.LookRotation(forwardDir, worldUp);
            }
        }

        public void LookAt(Vec3 worldPosition)
        {
            LookAt(worldPosition, Vec3.Up);
        }
        #endregion

        #region SpaceConversionMethods
        public Vec3 TransformPoint(Vec3 position)
        {
            return localToWorldMatrix.MultiplyPoint(position);
        }

        public Vec3 TransformPoint(float x, float y, float z)
        {
            return TransformPoint(new Vec3(x, y, z));
        }

        public Vec3 InverseTransformPoint(Vec3 position)
        {
            return worldToLocalMatrix.MultiplyPoint(position);
        }

        public Vec3 InverseTransformPoint(float x, float y, float z)
        {
            return InverseTransformPoint(new Vec3(x, y, z));
        }

        public Vec3 TransformVector(Vec3 vector)
        {
            return localToWorldMatrix.MultiplyVector(vector);
        }

        public Vec3 TransformVector(float x, float y, float z)
        {
            return TransformVector(new Vec3(x, y, z));
        }

        public Vec3 InverseTransformVector(Vec3 vector)
        {
            return worldToLocalMatrix.MultiplyVector(vector);
        }

        public Vec3 InverseTransformVector(float x, float y, float z)
        {
            return InverseTransformVector(new Vec3(x, y, z));
        }

        public Vec3 TransformDirection(Vec3 direction)
        {
            return rotation * direction;
        }

        public Vec3 TransformDirection(float x, float y, float z)
        {
            return TransformDirection(new Vec3(x, y, z));
        }

        public Vec3 InverseTransformDirection(Vec3 direction)
        {
            return Quat.Inverse(rotation) * direction;
        }

        public Vec3 InverseTransformDirection(float x, float y, float z)
        {
            return InverseTransformDirection(new Vec3(x, y, z));
        }
        #endregion

        #region PrivateUtilityMethods
        private void SetDirty()
        {
            hasChanged = true;
            foreach (MyTransform child in _children)
            {
                child.SetDirty();
            }
        }

        private int GetHierarchyCountRecursive(MyTransform current)
        {
            int count = 1;
            foreach (MyTransform child in current._children)
            {
                count += GetHierarchyCountRecursive(child);
            }
            return count;
        }
        #endregion
    }
}