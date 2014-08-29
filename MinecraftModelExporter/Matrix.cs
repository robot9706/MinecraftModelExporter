using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftModelExporter
{
    public struct Matrix
    {
        internal static Matrix InternalIdentity = new Matrix(1, 0, 0, 0, 1, 0, 0, 0, 1);

        public Matrix(float m11, float m12, float m13, float m14, float m21, float m22, float m23, float m24, float m31,
                      float m32, float m33, float m34, float m41, float m42, float m43, float m44)
        {
            this.M11 = m11;
            this.M12 = m12;
            this.M13 = m13;
            this.M14 = m14;
            this.M21 = m21;
            this.M22 = m22;
            this.M23 = m23;
            this.M24 = m24;
            this.M31 = m31;
            this.M32 = m32;
            this.M33 = m33;
            this.M34 = m34;
            this.M41 = m41;
            this.M42 = m42;
            this.M43 = m43;
            this.M44 = m44;
        }

        public Matrix(float m11, float m12, float m13, float m21, float m22, float m23, float m31, float m32, float m33)
        {
            this.M11 = m11;
            this.M12 = m12;
            this.M13 = m13;
            this.M14 = 0;
            this.M21 = m21;
            this.M22 = m22;
            this.M23 = m23;
            this.M24 = 0;
            this.M31 = m31;
            this.M32 = m32;
            this.M33 = m33;
            this.M34 = 0;
            this.M41 = 0;
            this.M42 = 0;
            this.M43 = 0;
            this.M44 = 0;
        }

        public float M11;
        public float M12;
        public float M13;
        public float M14;
        public float M21;
        public float M22;
        public float M23;
        public float M24;
        public float M31;
        public float M32;
        public float M33;
        public float M34;
        public float M41;
        public float M42;
        public float M43;
        public float M44;

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return M11;
                    case 1: return M12;
                    case 2: return M13;
                    case 3: return M14;
                    case 4: return M21;
                    case 5: return M22;
                    case 6: return M23;
                    case 7: return M24;
                    case 8: return M31;
                    case 9: return M32;
                    case 10: return M33;
                    case 11: return M34;
                    case 12: return M41;
                    case 13: return M42;
                    case 14: return M43;
                    case 15: return M44;
                }
                throw new ArgumentOutOfRangeException();
            }

            set
            {
                switch (index)
                {
                    case 0: M11 = value; break;
                    case 1: M12 = value; break;
                    case 2: M13 = value; break;
                    case 3: M14 = value; break;
                    case 4: M21 = value; break;
                    case 5: M22 = value; break;
                    case 6: M23 = value; break;
                    case 7: M24 = value; break;
                    case 8: M31 = value; break;
                    case 9: M32 = value; break;
                    case 10: M33 = value; break;
                    case 11: M34 = value; break;
                    case 12: M41 = value; break;
                    case 13: M42 = value; break;
                    case 14: M43 = value; break;
                    case 15: M44 = value; break;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        public float this[int row, int column]
        {
            get
            {
                return this[(row * 4) + column];
            }

            set
            {
                this[(row * 4) + column] = value;
            }
        }

        private static Matrix identity = new Matrix(1f, 0f, 0f, 0f,
                                                    0f, 1f, 0f, 0f,
                                                    0f, 0f, 1f, 0f,
                                                    0f, 0f, 0f, 1f);

        public static Matrix Zero = new Matrix(0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f);
        public Vector3 Backward
        {
            get
            {
                return new Vector3(this.M31, this.M32, this.M33);
            }
            set
            {
                this.M31 = value.X;
                this.M32 = value.Y;
                this.M33 = value.Z;
            }
        }


        public Vector3 Down
        {
            get
            {
                return new Vector3(-this.M21, -this.M22, -this.M23);
            }
            set
            {
                this.M21 = -value.X;
                this.M22 = -value.Y;
                this.M23 = -value.Z;
            }
        }


        public Vector3 Forward
        {
            get
            {
                return new Vector3(-this.M31, -this.M32, -this.M33);
            }
            set
            {
                this.M31 = -value.X;
                this.M32 = -value.Y;
                this.M33 = -value.Z;
            }
        }


        public static Matrix Identity
        {
            get { return identity; }
        }

        public Vector3 Left
        {
            get
            {
                return new Vector3(-this.M11, -this.M12, -this.M13);
            }
            set
            {
                this.M11 = -value.X;
                this.M12 = -value.Y;
                this.M13 = -value.Z;
            }
        }


        public Vector3 Right
        {
            get
            {
                return new Vector3(this.M11, this.M12, this.M13);
            }
            set
            {
                this.M11 = value.X;
                this.M12 = value.Y;
                this.M13 = value.Z;
            }
        }


        public Vector3 Translation
        {
            get
            {
                return new Vector3(this.M41, this.M42, this.M43);
            }
            set
            {
                this.M41 = value.X;
                this.M42 = value.Y;
                this.M43 = value.Z;
            }
        }


        public Vector3 Up
        {
            get
            {
                return new Vector3(this.M21, this.M22, this.M23);
            }
            set
            {
                this.M21 = value.X;
                this.M22 = value.Y;
                this.M23 = value.Z;
            }
        }

        public float[] ToArray()
        {
            return new float[] { M11, M12, M13, M14, M21, M22, M23, M24, M31, M32, M33, M34, M41, M42, M43, M44 };
        }

        public float Trace()
        {
            return this.M11 + this.M22 + this.M33;
        }

        public static void Inverse(ref Matrix matrix, out Matrix res)
        {
            Matrix result = Matrix.identity;

            float det = matrix.M11 * matrix.M22 * matrix.M33 -
                matrix.M11 * matrix.M23 * matrix.M32 -
                matrix.M12 * matrix.M21 * matrix.M33 +
                matrix.M12 * matrix.M23 * matrix.M31 +
                matrix.M13 * matrix.M21 * matrix.M32 -
                matrix.M13 * matrix.M22 * matrix.M31;

            float num11 = matrix.M22 * matrix.M33 - matrix.M23 * matrix.M32;
            float num12 = matrix.M13 * matrix.M32 - matrix.M12 * matrix.M33;
            float num13 = matrix.M12 * matrix.M23 - matrix.M22 * matrix.M13;

            float num21 = matrix.M23 * matrix.M31 - matrix.M33 * matrix.M21;
            float num22 = matrix.M11 * matrix.M33 - matrix.M31 * matrix.M13;
            float num23 = matrix.M13 * matrix.M21 - matrix.M23 * matrix.M11;

            float num31 = matrix.M21 * matrix.M32 - matrix.M31 * matrix.M22;
            float num32 = matrix.M12 * matrix.M31 - matrix.M32 * matrix.M11;
            float num33 = matrix.M11 * matrix.M22 - matrix.M21 * matrix.M12;

            result.M11 = num11 / det;
            result.M12 = num12 / det;
            result.M13 = num13 / det;
            result.M21 = num21 / det;
            result.M22 = num22 / det;
            result.M23 = num23 / det;
            result.M31 = num31 / det;
            result.M32 = num32 / det;
            result.M33 = num33 / det;

            res = result;
        }

        public static Matrix Add(Matrix matrix1, Matrix matrix2)
        {
            matrix1.M11 += matrix2.M11;
            matrix1.M12 += matrix2.M12;
            matrix1.M13 += matrix2.M13;
            matrix1.M14 += matrix2.M14;
            matrix1.M21 += matrix2.M21;
            matrix1.M22 += matrix2.M22;
            matrix1.M23 += matrix2.M23;
            matrix1.M24 += matrix2.M24;
            matrix1.M31 += matrix2.M31;
            matrix1.M32 += matrix2.M32;
            matrix1.M33 += matrix2.M33;
            matrix1.M34 += matrix2.M34;
            matrix1.M41 += matrix2.M41;
            matrix1.M42 += matrix2.M42;
            matrix1.M43 += matrix2.M43;
            matrix1.M44 += matrix2.M44;
            return matrix1;
        }


        public static void Add(ref Matrix matrix1, ref Matrix matrix2, out Matrix res)
        {
            Matrix result = Matrix.identity;

            result.M11 = matrix1.M11 + matrix2.M11;
            result.M12 = matrix1.M12 + matrix2.M12;
            result.M13 = matrix1.M13 + matrix2.M13;
            result.M14 = matrix1.M14 + matrix2.M14;
            result.M21 = matrix1.M21 + matrix2.M21;
            result.M22 = matrix1.M22 + matrix2.M22;
            result.M23 = matrix1.M23 + matrix2.M23;
            result.M24 = matrix1.M24 + matrix2.M24;
            result.M31 = matrix1.M31 + matrix2.M31;
            result.M32 = matrix1.M32 + matrix2.M32;
            result.M33 = matrix1.M33 + matrix2.M33;
            result.M34 = matrix1.M34 + matrix2.M34;
            result.M41 = matrix1.M41 + matrix2.M41;
            result.M42 = matrix1.M42 + matrix2.M42;
            result.M43 = matrix1.M43 + matrix2.M43;
            result.M44 = matrix1.M44 + matrix2.M44;

            res = result;
        }

        public static Matrix CreateRotation(Vector3 rotation)
        {
            Matrix res;
            CreateRotation(rotation, out res);

            return res;
        }

        public static void CreateRotation(Vector3 rotation, out Matrix res)
        {
            res = Matrix.CreateRotationX(rotation.X) * Matrix.CreateRotationY(rotation.Y) * Matrix.CreateRotationZ(rotation.Z);
        }

        public static Matrix CreateRotationX(float radians)
        {
            Matrix result;
            CreateRotationX(radians, out result);
            return result;
        }


        public static void CreateRotationX(float radians, out Matrix res)
        {
            Matrix result = Matrix.identity;

            var val1 = (float)Math.Cos(radians);
            var val2 = (float)Math.Sin(radians);

            result.M22 = val1;
            result.M23 = val2;
            result.M32 = -val2;
            result.M33 = val1;

            res = result;
        }

        public static Matrix CreateRotationY(float radians)
        {
            Matrix result;
            CreateRotationY(radians, out result);
            return result;
        }


        public static void CreateRotationY(float radians, out Matrix res)
        {
            Matrix result = Matrix.identity;

            var val1 = (float)Math.Cos(radians);
            var val2 = (float)Math.Sin(radians);

            result.M11 = val1;
            result.M13 = -val2;
            result.M31 = val2;
            result.M33 = val1;

            res = result;
        }


        public static Matrix CreateRotationZ(float radians)
        {
            Matrix result;
            CreateRotationZ(radians, out result);
            return result;
        }


        public static void CreateRotationZ(float radians, out Matrix res)
        {
            Matrix result = Matrix.identity;

            var val1 = (float)Math.Cos(radians);
            var val2 = (float)Math.Sin(radians);

            result.M11 = val1;
            result.M12 = val2;
            result.M21 = -val2;
            result.M22 = val1;

            res = result;
        }


        public static Matrix CreateScale(float scale)
        {
            Matrix result;
            CreateScale(scale, scale, scale, out result);
            return result;
        }


        public static void CreateScale(float scale, out Matrix result)
        {
            CreateScale(scale, scale, scale, out result);
        }


        public static Matrix CreateScale(float xScale, float yScale, float zScale)
        {
            Matrix result;
            CreateScale(xScale, yScale, zScale, out result);
            return result;
        }


        public static void CreateScale(float xScale, float yScale, float zScale, out Matrix res)
        {
            Matrix result = Matrix.identity;

            result.M11 = xScale;
            result.M12 = 0;
            result.M13 = 0;
            result.M14 = 0;
            result.M21 = 0;
            result.M22 = yScale;
            result.M23 = 0;
            result.M24 = 0;
            result.M31 = 0;
            result.M32 = 0;
            result.M33 = zScale;
            result.M34 = 0;
            result.M41 = 0;
            result.M42 = 0;
            result.M43 = 0;
            result.M44 = 1;

            res = result;
        }


        public static Matrix CreateScale(Vector3 scales)
        {
            Matrix result;
            CreateScale(ref scales, out result);
            return result;
        }


        public static void CreateScale(ref Vector3 scales, out Matrix res)
        {
            Matrix result = Matrix.identity;

            result.M11 = scales.X;
            result.M12 = 0;
            result.M13 = 0;
            result.M14 = 0;
            result.M21 = 0;
            result.M22 = scales.Y;
            result.M23 = 0;
            result.M24 = 0;
            result.M31 = 0;
            result.M32 = 0;
            result.M33 = scales.Z;
            result.M34 = 0;
            result.M41 = 0;
            result.M42 = 0;
            result.M43 = 0;
            result.M44 = 1;

            res = result;
        }

        public static Matrix CreateTranslation(float xPosition, float yPosition, float zPosition)
        {
            Matrix result;
            CreateTranslation(xPosition, yPosition, zPosition, out result);
            return result;
        }


        public static void CreateTranslation(ref Vector3 position, out Matrix res)
        {
            Matrix result = Matrix.identity;

            result.M11 = 1;
            result.M12 = 0;
            result.M13 = 0;
            result.M14 = 0;
            result.M21 = 0;
            result.M22 = 1;
            result.M23 = 0;
            result.M24 = 0;
            result.M31 = 0;
            result.M32 = 0;
            result.M33 = 1;
            result.M34 = 0;
            result.M41 = position.X;
            result.M42 = position.Y;
            result.M43 = position.Z;
            result.M44 = 1;

            res = result;
        }


        public static Matrix CreateTranslation(Vector3 position)
        {
            Matrix result;
            CreateTranslation(ref position, out result);
            return result;
        }


        public static void CreateTranslation(float xPosition, float yPosition, float zPosition, out Matrix res)
        {
            Matrix result = Matrix.identity;

            result.M11 = 1;
            result.M12 = 0;
            result.M13 = 0;
            result.M14 = 0;
            result.M21 = 0;
            result.M22 = 1;
            result.M23 = 0;
            result.M24 = 0;
            result.M31 = 0;
            result.M32 = 0;
            result.M33 = 1;
            result.M34 = 0;
            result.M41 = xPosition;
            result.M42 = yPosition;
            result.M43 = zPosition;
            result.M44 = 1;

            res = result;
        }
       
        public float Determinant()
        {
            float num22 = this.M11;
            float num21 = this.M12;
            float num20 = this.M13;
            float num19 = this.M14;
            float num12 = this.M21;
            float num11 = this.M22;
            float num10 = this.M23;
            float num9 = this.M24;
            float num8 = this.M31;
            float num7 = this.M32;
            float num6 = this.M33;
            float num5 = this.M34;
            float num4 = this.M41;
            float num3 = this.M42;
            float num2 = this.M43;
            float num = this.M44;
            float num18 = (num6 * num) - (num5 * num2);
            float num17 = (num7 * num) - (num5 * num3);
            float num16 = (num7 * num2) - (num6 * num3);
            float num15 = (num8 * num) - (num5 * num4);
            float num14 = (num8 * num2) - (num6 * num4);
            float num13 = (num8 * num3) - (num7 * num4);
            return ((((num22 * (((num11 * num18) - (num10 * num17)) + (num9 * num16))) - (num21 * (((num12 * num18) - (num10 * num15)) + (num9 * num14)))) + (num20 * (((num12 * num17) - (num11 * num15)) + (num9 * num13)))) - (num19 * (((num12 * num16) - (num11 * num14)) + (num10 * num13))));
        }


        public static Matrix Divide(Matrix matrix1, Matrix matrix2)
        {
            matrix1.M11 = matrix1.M11 / matrix2.M11;
            matrix1.M12 = matrix1.M12 / matrix2.M12;
            matrix1.M13 = matrix1.M13 / matrix2.M13;
            matrix1.M14 = matrix1.M14 / matrix2.M14;
            matrix1.M21 = matrix1.M21 / matrix2.M21;
            matrix1.M22 = matrix1.M22 / matrix2.M22;
            matrix1.M23 = matrix1.M23 / matrix2.M23;
            matrix1.M24 = matrix1.M24 / matrix2.M24;
            matrix1.M31 = matrix1.M31 / matrix2.M31;
            matrix1.M32 = matrix1.M32 / matrix2.M32;
            matrix1.M33 = matrix1.M33 / matrix2.M33;
            matrix1.M34 = matrix1.M34 / matrix2.M34;
            matrix1.M41 = matrix1.M41 / matrix2.M41;
            matrix1.M42 = matrix1.M42 / matrix2.M42;
            matrix1.M43 = matrix1.M43 / matrix2.M43;
            matrix1.M44 = matrix1.M44 / matrix2.M44;
            return matrix1;
        }


        public static void Divide(ref Matrix matrix1, ref Matrix matrix2, out Matrix res)
        {
            Matrix result = Matrix.identity;

            result.M11 = matrix1.M11 / matrix2.M11;
            result.M12 = matrix1.M12 / matrix2.M12;
            result.M13 = matrix1.M13 / matrix2.M13;
            result.M14 = matrix1.M14 / matrix2.M14;
            result.M21 = matrix1.M21 / matrix2.M21;
            result.M22 = matrix1.M22 / matrix2.M22;
            result.M23 = matrix1.M23 / matrix2.M23;
            result.M24 = matrix1.M24 / matrix2.M24;
            result.M31 = matrix1.M31 / matrix2.M31;
            result.M32 = matrix1.M32 / matrix2.M32;
            result.M33 = matrix1.M33 / matrix2.M33;
            result.M34 = matrix1.M34 / matrix2.M34;
            result.M41 = matrix1.M41 / matrix2.M41;
            result.M42 = matrix1.M42 / matrix2.M42;
            result.M43 = matrix1.M43 / matrix2.M43;
            result.M44 = matrix1.M44 / matrix2.M44;

            res = result;
        }


        public static Matrix Divide(Matrix matrix1, float divider)
        {
            float num = 1f / divider;
            matrix1.M11 = matrix1.M11 * num;
            matrix1.M12 = matrix1.M12 * num;
            matrix1.M13 = matrix1.M13 * num;
            matrix1.M14 = matrix1.M14 * num;
            matrix1.M21 = matrix1.M21 * num;
            matrix1.M22 = matrix1.M22 * num;
            matrix1.M23 = matrix1.M23 * num;
            matrix1.M24 = matrix1.M24 * num;
            matrix1.M31 = matrix1.M31 * num;
            matrix1.M32 = matrix1.M32 * num;
            matrix1.M33 = matrix1.M33 * num;
            matrix1.M34 = matrix1.M34 * num;
            matrix1.M41 = matrix1.M41 * num;
            matrix1.M42 = matrix1.M42 * num;
            matrix1.M43 = matrix1.M43 * num;
            matrix1.M44 = matrix1.M44 * num;
            return matrix1;
        }


        public static void Divide(ref Matrix matrix1, float divider, out Matrix res)
        {
            Matrix result = Matrix.identity;

            float num = 1f / divider;
            result.M11 = matrix1.M11 * num;
            result.M12 = matrix1.M12 * num;
            result.M13 = matrix1.M13 * num;
            result.M14 = matrix1.M14 * num;
            result.M21 = matrix1.M21 * num;
            result.M22 = matrix1.M22 * num;
            result.M23 = matrix1.M23 * num;
            result.M24 = matrix1.M24 * num;
            result.M31 = matrix1.M31 * num;
            result.M32 = matrix1.M32 * num;
            result.M33 = matrix1.M33 * num;
            result.M34 = matrix1.M34 * num;
            result.M41 = matrix1.M41 * num;
            result.M42 = matrix1.M42 * num;
            result.M43 = matrix1.M43 * num;
            result.M44 = matrix1.M44 * num;

            res = result;
        }


        public bool Equals(Matrix other)
        {
            return ((((((this.M11 == other.M11) && (this.M22 == other.M22)) && ((this.M33 == other.M33) && (this.M44 == other.M44))) && (((this.M12 == other.M12) && (this.M13 == other.M13)) && ((this.M14 == other.M14) && (this.M21 == other.M21)))) && ((((this.M23 == other.M23) && (this.M24 == other.M24)) && ((this.M31 == other.M31) && (this.M32 == other.M32))) && (((this.M34 == other.M34) && (this.M41 == other.M41)) && (this.M42 == other.M42)))) && (this.M43 == other.M43));
        }


        public override bool Equals(object obj)
        {
            bool flag = false;
            if (obj is Matrix)
            {
                flag = this.Equals((Matrix)obj);
            }
            return flag;
        }


        public override int GetHashCode()
        {
            return (((((((((((((((this.M11.GetHashCode() + this.M12.GetHashCode()) + this.M13.GetHashCode()) + this.M14.GetHashCode()) + this.M21.GetHashCode()) + this.M22.GetHashCode()) + this.M23.GetHashCode()) + this.M24.GetHashCode()) + this.M31.GetHashCode()) + this.M32.GetHashCode()) + this.M33.GetHashCode()) + this.M34.GetHashCode()) + this.M41.GetHashCode()) + this.M42.GetHashCode()) + this.M43.GetHashCode()) + this.M44.GetHashCode());
        }


        public static Matrix Invert(Matrix matrix)
        {
            Invert(ref matrix, out matrix);
            return matrix;
        }


        public static void Invert(ref Matrix matrix, out Matrix res)
        {
            Matrix result = Matrix.identity;

            float num1 = matrix.M11;
            float num2 = matrix.M12;
            float num3 = matrix.M13;
            float num4 = matrix.M14;
            float num5 = matrix.M21;
            float num6 = matrix.M22;
            float num7 = matrix.M23;
            float num8 = matrix.M24;
            float num9 = matrix.M31;
            float num10 = matrix.M32;
            float num11 = matrix.M33;
            float num12 = matrix.M34;
            float num13 = matrix.M41;
            float num14 = matrix.M42;
            float num15 = matrix.M43;
            float num16 = matrix.M44;
            float num17 = (float)((float)num11 * (float)num16 - (float)num12 * (float)num15);
            float num18 = (float)((float)num10 * (float)num16 - (float)num12 * (float)num14);
            float num19 = (float)((float)num10 * (float)num15 - (float)num11 * (float)num14);
            float num20 = (float)((float)num9 * (float)num16 - (float)num12 * (float)num13);
            float num21 = (float)((float)num9 * (float)num15 - (float)num11 * (float)num13);
            float num22 = (float)((float)num9 * (float)num14 - (float)num10 * (float)num13);
            float num23 = (float)((float)num6 * (float)num17 - (float)num7 * (float)num18 + (float)num8 * (float)num19);
            float num24 = (float)-((float)num5 * (float)num17 - (float)num7 * (float)num20 + (float)num8 * (float)num21);
            float num25 = (float)((float)num5 * (float)num18 - (float)num6 * (float)num20 + (float)num8 * (float)num22);
            float num26 = (float)-((float)num5 * (float)num19 - (float)num6 * (float)num21 + (float)num7 * (float)num22);
            float num27 = (float)(1.0 / ((float)num1 * (float)num23 + (float)num2 * (float)num24 + (float)num3 * (float)num25 + (float)num4 * (float)num26));

            result.M11 = num23 * num27;
            result.M21 = num24 * num27;
            result.M31 = num25 * num27;
            result.M41 = num26 * num27;
            result.M12 = (float)-((float)num2 * (float)num17 - (float)num3 * (float)num18 + (float)num4 * (float)num19) * num27;
            result.M22 = (float)((float)num1 * (float)num17 - (float)num3 * (float)num20 + (float)num4 * (float)num21) * num27;
            result.M32 = (float)-((float)num1 * (float)num18 - (float)num2 * (float)num20 + (float)num4 * (float)num22) * num27;
            result.M42 = (float)((float)num1 * (float)num19 - (float)num2 * (float)num21 + (float)num3 * (float)num22) * num27;
            float num28 = (float)((float)num7 * (float)num16 - (float)num8 * (float)num15);
            float num29 = (float)((float)num6 * (float)num16 - (float)num8 * (float)num14);
            float num30 = (float)((float)num6 * (float)num15 - (float)num7 * (float)num14);
            float num31 = (float)((float)num5 * (float)num16 - (float)num8 * (float)num13);
            float num32 = (float)((float)num5 * (float)num15 - (float)num7 * (float)num13);
            float num33 = (float)((float)num5 * (float)num14 - (float)num6 * (float)num13);
            result.M13 = (float)((float)num2 * (float)num28 - (float)num3 * (float)num29 + (float)num4 * (float)num30) * num27;
            result.M23 = (float)-((float)num1 * (float)num28 - (float)num3 * (float)num31 + (float)num4 * (float)num32) * num27;
            result.M33 = (float)((float)num1 * (float)num29 - (float)num2 * (float)num31 + (float)num4 * (float)num33) * num27;
            result.M43 = (float)-((float)num1 * (float)num30 - (float)num2 * (float)num32 + (float)num3 * (float)num33) * num27;
            float num34 = (float)((float)num7 * (float)num12 - (float)num8 * (float)num11);
            float num35 = (float)((float)num6 * (float)num12 - (float)num8 * (float)num10);
            float num36 = (float)((float)num6 * (float)num11 - (float)num7 * (float)num10);
            float num37 = (float)((float)num5 * (float)num12 - (float)num8 * (float)num9);
            float num38 = (float)((float)num5 * (float)num11 - (float)num7 * (float)num9);
            float num39 = (float)((float)num5 * (float)num10 - (float)num6 * (float)num9);
            result.M14 = (float)-((float)num2 * (float)num34 - (float)num3 * (float)num35 + (float)num4 * (float)num36) * num27;
            result.M24 = (float)((float)num1 * (float)num34 - (float)num3 * (float)num37 + (float)num4 * (float)num38) * num27;
            result.M34 = (float)-((float)num1 * (float)num35 - (float)num2 * (float)num37 + (float)num4 * (float)num39) * num27;
            result.M44 = (float)((float)num1 * (float)num36 - (float)num2 * (float)num38 + (float)num3 * (float)num39) * num27;

            res = result;
        }


        public static Matrix Lerp(Matrix matrix1, Matrix matrix2, float amount)
        {
            matrix1.M11 = matrix1.M11 + ((matrix2.M11 - matrix1.M11) * amount);
            matrix1.M12 = matrix1.M12 + ((matrix2.M12 - matrix1.M12) * amount);
            matrix1.M13 = matrix1.M13 + ((matrix2.M13 - matrix1.M13) * amount);
            matrix1.M14 = matrix1.M14 + ((matrix2.M14 - matrix1.M14) * amount);
            matrix1.M21 = matrix1.M21 + ((matrix2.M21 - matrix1.M21) * amount);
            matrix1.M22 = matrix1.M22 + ((matrix2.M22 - matrix1.M22) * amount);
            matrix1.M23 = matrix1.M23 + ((matrix2.M23 - matrix1.M23) * amount);
            matrix1.M24 = matrix1.M24 + ((matrix2.M24 - matrix1.M24) * amount);
            matrix1.M31 = matrix1.M31 + ((matrix2.M31 - matrix1.M31) * amount);
            matrix1.M32 = matrix1.M32 + ((matrix2.M32 - matrix1.M32) * amount);
            matrix1.M33 = matrix1.M33 + ((matrix2.M33 - matrix1.M33) * amount);
            matrix1.M34 = matrix1.M34 + ((matrix2.M34 - matrix1.M34) * amount);
            matrix1.M41 = matrix1.M41 + ((matrix2.M41 - matrix1.M41) * amount);
            matrix1.M42 = matrix1.M42 + ((matrix2.M42 - matrix1.M42) * amount);
            matrix1.M43 = matrix1.M43 + ((matrix2.M43 - matrix1.M43) * amount);
            matrix1.M44 = matrix1.M44 + ((matrix2.M44 - matrix1.M44) * amount);
            return matrix1;
        }


        public static void Lerp(ref Matrix matrix1, ref Matrix matrix2, float amount, out Matrix res)
        {
            Matrix result = Matrix.identity;

            result.M11 = matrix1.M11 + ((matrix2.M11 - matrix1.M11) * amount);
            result.M12 = matrix1.M12 + ((matrix2.M12 - matrix1.M12) * amount);
            result.M13 = matrix1.M13 + ((matrix2.M13 - matrix1.M13) * amount);
            result.M14 = matrix1.M14 + ((matrix2.M14 - matrix1.M14) * amount);
            result.M21 = matrix1.M21 + ((matrix2.M21 - matrix1.M21) * amount);
            result.M22 = matrix1.M22 + ((matrix2.M22 - matrix1.M22) * amount);
            result.M23 = matrix1.M23 + ((matrix2.M23 - matrix1.M23) * amount);
            result.M24 = matrix1.M24 + ((matrix2.M24 - matrix1.M24) * amount);
            result.M31 = matrix1.M31 + ((matrix2.M31 - matrix1.M31) * amount);
            result.M32 = matrix1.M32 + ((matrix2.M32 - matrix1.M32) * amount);
            result.M33 = matrix1.M33 + ((matrix2.M33 - matrix1.M33) * amount);
            result.M34 = matrix1.M34 + ((matrix2.M34 - matrix1.M34) * amount);
            result.M41 = matrix1.M41 + ((matrix2.M41 - matrix1.M41) * amount);
            result.M42 = matrix1.M42 + ((matrix2.M42 - matrix1.M42) * amount);
            result.M43 = matrix1.M43 + ((matrix2.M43 - matrix1.M43) * amount);
            result.M44 = matrix1.M44 + ((matrix2.M44 - matrix1.M44) * amount);

            res = result;
        }

        public static Matrix Multiply(Matrix matrix1, Matrix matrix2)
        {
            var m11 = (((matrix1.M11 * matrix2.M11) + (matrix1.M12 * matrix2.M21)) + (matrix1.M13 * matrix2.M31)) + (matrix1.M14 * matrix2.M41);
            var m12 = (((matrix1.M11 * matrix2.M12) + (matrix1.M12 * matrix2.M22)) + (matrix1.M13 * matrix2.M32)) + (matrix1.M14 * matrix2.M42);
            var m13 = (((matrix1.M11 * matrix2.M13) + (matrix1.M12 * matrix2.M23)) + (matrix1.M13 * matrix2.M33)) + (matrix1.M14 * matrix2.M43);
            var m14 = (((matrix1.M11 * matrix2.M14) + (matrix1.M12 * matrix2.M24)) + (matrix1.M13 * matrix2.M34)) + (matrix1.M14 * matrix2.M44);
            var m21 = (((matrix1.M21 * matrix2.M11) + (matrix1.M22 * matrix2.M21)) + (matrix1.M23 * matrix2.M31)) + (matrix1.M24 * matrix2.M41);
            var m22 = (((matrix1.M21 * matrix2.M12) + (matrix1.M22 * matrix2.M22)) + (matrix1.M23 * matrix2.M32)) + (matrix1.M24 * matrix2.M42);
            var m23 = (((matrix1.M21 * matrix2.M13) + (matrix1.M22 * matrix2.M23)) + (matrix1.M23 * matrix2.M33)) + (matrix1.M24 * matrix2.M43);
            var m24 = (((matrix1.M21 * matrix2.M14) + (matrix1.M22 * matrix2.M24)) + (matrix1.M23 * matrix2.M34)) + (matrix1.M24 * matrix2.M44);
            var m31 = (((matrix1.M31 * matrix2.M11) + (matrix1.M32 * matrix2.M21)) + (matrix1.M33 * matrix2.M31)) + (matrix1.M34 * matrix2.M41);
            var m32 = (((matrix1.M31 * matrix2.M12) + (matrix1.M32 * matrix2.M22)) + (matrix1.M33 * matrix2.M32)) + (matrix1.M34 * matrix2.M42);
            var m33 = (((matrix1.M31 * matrix2.M13) + (matrix1.M32 * matrix2.M23)) + (matrix1.M33 * matrix2.M33)) + (matrix1.M34 * matrix2.M43);
            var m34 = (((matrix1.M31 * matrix2.M14) + (matrix1.M32 * matrix2.M24)) + (matrix1.M33 * matrix2.M34)) + (matrix1.M34 * matrix2.M44);
            var m41 = (((matrix1.M41 * matrix2.M11) + (matrix1.M42 * matrix2.M21)) + (matrix1.M43 * matrix2.M31)) + (matrix1.M44 * matrix2.M41);
            var m42 = (((matrix1.M41 * matrix2.M12) + (matrix1.M42 * matrix2.M22)) + (matrix1.M43 * matrix2.M32)) + (matrix1.M44 * matrix2.M42);
            var m43 = (((matrix1.M41 * matrix2.M13) + (matrix1.M42 * matrix2.M23)) + (matrix1.M43 * matrix2.M33)) + (matrix1.M44 * matrix2.M43);
            var m44 = (((matrix1.M41 * matrix2.M14) + (matrix1.M42 * matrix2.M24)) + (matrix1.M43 * matrix2.M34)) + (matrix1.M44 * matrix2.M44);
            matrix1.M11 = m11;
            matrix1.M12 = m12;
            matrix1.M13 = m13;
            matrix1.M14 = m14;
            matrix1.M21 = m21;
            matrix1.M22 = m22;
            matrix1.M23 = m23;
            matrix1.M24 = m24;
            matrix1.M31 = m31;
            matrix1.M32 = m32;
            matrix1.M33 = m33;
            matrix1.M34 = m34;
            matrix1.M41 = m41;
            matrix1.M42 = m42;
            matrix1.M43 = m43;
            matrix1.M44 = m44;
            return matrix1;
        }


        public static void Multiply(ref Matrix matrix1, ref Matrix matrix2, out Matrix res)
        {
            Matrix result = Matrix.identity;

            var m11 = (((matrix1.M11 * matrix2.M11) + (matrix1.M12 * matrix2.M21)) + (matrix1.M13 * matrix2.M31)) + (matrix1.M14 * matrix2.M41);
            var m12 = (((matrix1.M11 * matrix2.M12) + (matrix1.M12 * matrix2.M22)) + (matrix1.M13 * matrix2.M32)) + (matrix1.M14 * matrix2.M42);
            var m13 = (((matrix1.M11 * matrix2.M13) + (matrix1.M12 * matrix2.M23)) + (matrix1.M13 * matrix2.M33)) + (matrix1.M14 * matrix2.M43);
            var m14 = (((matrix1.M11 * matrix2.M14) + (matrix1.M12 * matrix2.M24)) + (matrix1.M13 * matrix2.M34)) + (matrix1.M14 * matrix2.M44);
            var m21 = (((matrix1.M21 * matrix2.M11) + (matrix1.M22 * matrix2.M21)) + (matrix1.M23 * matrix2.M31)) + (matrix1.M24 * matrix2.M41);
            var m22 = (((matrix1.M21 * matrix2.M12) + (matrix1.M22 * matrix2.M22)) + (matrix1.M23 * matrix2.M32)) + (matrix1.M24 * matrix2.M42);
            var m23 = (((matrix1.M21 * matrix2.M13) + (matrix1.M22 * matrix2.M23)) + (matrix1.M23 * matrix2.M33)) + (matrix1.M24 * matrix2.M43);
            var m24 = (((matrix1.M21 * matrix2.M14) + (matrix1.M22 * matrix2.M24)) + (matrix1.M23 * matrix2.M34)) + (matrix1.M24 * matrix2.M44);
            var m31 = (((matrix1.M31 * matrix2.M11) + (matrix1.M32 * matrix2.M21)) + (matrix1.M33 * matrix2.M31)) + (matrix1.M34 * matrix2.M41);
            var m32 = (((matrix1.M31 * matrix2.M12) + (matrix1.M32 * matrix2.M22)) + (matrix1.M33 * matrix2.M32)) + (matrix1.M34 * matrix2.M42);
            var m33 = (((matrix1.M31 * matrix2.M13) + (matrix1.M32 * matrix2.M23)) + (matrix1.M33 * matrix2.M33)) + (matrix1.M34 * matrix2.M43);
            var m34 = (((matrix1.M31 * matrix2.M14) + (matrix1.M32 * matrix2.M24)) + (matrix1.M33 * matrix2.M34)) + (matrix1.M34 * matrix2.M44);
            var m41 = (((matrix1.M41 * matrix2.M11) + (matrix1.M42 * matrix2.M21)) + (matrix1.M43 * matrix2.M31)) + (matrix1.M44 * matrix2.M41);
            var m42 = (((matrix1.M41 * matrix2.M12) + (matrix1.M42 * matrix2.M22)) + (matrix1.M43 * matrix2.M32)) + (matrix1.M44 * matrix2.M42);
            var m43 = (((matrix1.M41 * matrix2.M13) + (matrix1.M42 * matrix2.M23)) + (matrix1.M43 * matrix2.M33)) + (matrix1.M44 * matrix2.M43);
            var m44 = (((matrix1.M41 * matrix2.M14) + (matrix1.M42 * matrix2.M24)) + (matrix1.M43 * matrix2.M34)) + (matrix1.M44 * matrix2.M44);
            result.M11 = m11;
            result.M12 = m12;
            result.M13 = m13;
            result.M14 = m14;
            result.M21 = m21;
            result.M22 = m22;
            result.M23 = m23;
            result.M24 = m24;
            result.M31 = m31;
            result.M32 = m32;
            result.M33 = m33;
            result.M34 = m34;
            result.M41 = m41;
            result.M42 = m42;
            result.M43 = m43;
            result.M44 = m44;

            res = result;
        }

        public static Matrix Multiply(Matrix matrix1, float factor)
        {
            matrix1.M11 *= factor;
            matrix1.M12 *= factor;
            matrix1.M13 *= factor;
            matrix1.M14 *= factor;
            matrix1.M21 *= factor;
            matrix1.M22 *= factor;
            matrix1.M23 *= factor;
            matrix1.M24 *= factor;
            matrix1.M31 *= factor;
            matrix1.M32 *= factor;
            matrix1.M33 *= factor;
            matrix1.M34 *= factor;
            matrix1.M41 *= factor;
            matrix1.M42 *= factor;
            matrix1.M43 *= factor;
            matrix1.M44 *= factor;
            return matrix1;
        }


        public static void Multiply(ref Matrix matrix1, float factor, out Matrix res)
        {
            Matrix result = Matrix.identity; ;

            result.M11 = matrix1.M11 * factor;
            result.M12 = matrix1.M12 * factor;
            result.M13 = matrix1.M13 * factor;
            result.M14 = matrix1.M14 * factor;
            result.M21 = matrix1.M21 * factor;
            result.M22 = matrix1.M22 * factor;
            result.M23 = matrix1.M23 * factor;
            result.M24 = matrix1.M24 * factor;
            result.M31 = matrix1.M31 * factor;
            result.M32 = matrix1.M32 * factor;
            result.M33 = matrix1.M33 * factor;
            result.M34 = matrix1.M34 * factor;
            result.M41 = matrix1.M41 * factor;
            result.M42 = matrix1.M42 * factor;
            result.M43 = matrix1.M43 * factor;
            result.M44 = matrix1.M44 * factor;

            res = result;
        }


        public static Matrix Negate(Matrix matrix)
        {
            matrix.M11 = -matrix.M11;
            matrix.M12 = -matrix.M12;
            matrix.M13 = -matrix.M13;
            matrix.M14 = -matrix.M14;
            matrix.M21 = -matrix.M21;
            matrix.M22 = -matrix.M22;
            matrix.M23 = -matrix.M23;
            matrix.M24 = -matrix.M24;
            matrix.M31 = -matrix.M31;
            matrix.M32 = -matrix.M32;
            matrix.M33 = -matrix.M33;
            matrix.M34 = -matrix.M34;
            matrix.M41 = -matrix.M41;
            matrix.M42 = -matrix.M42;
            matrix.M43 = -matrix.M43;
            matrix.M44 = -matrix.M44;
            return matrix;
        }


        public static void Negate(ref Matrix matrix, out Matrix res)
        {
            Matrix result = Matrix.identity;

            result.M11 = -matrix.M11;
            result.M12 = -matrix.M12;
            result.M13 = -matrix.M13;
            result.M14 = -matrix.M14;
            result.M21 = -matrix.M21;
            result.M22 = -matrix.M22;
            result.M23 = -matrix.M23;
            result.M24 = -matrix.M24;
            result.M31 = -matrix.M31;
            result.M32 = -matrix.M32;
            result.M33 = -matrix.M33;
            result.M34 = -matrix.M34;
            result.M41 = -matrix.M41;
            result.M42 = -matrix.M42;
            result.M43 = -matrix.M43;
            result.M44 = -matrix.M44;

            res = result;
        }


        public static Matrix operator +(Matrix matrix1, Matrix matrix2)
        {
            Matrix.Add(ref matrix1, ref matrix2, out matrix1);
            return matrix1;
        }


        public static Matrix operator /(Matrix matrix1, Matrix matrix2)
        {
            matrix1.M11 = matrix1.M11 / matrix2.M11;
            matrix1.M12 = matrix1.M12 / matrix2.M12;
            matrix1.M13 = matrix1.M13 / matrix2.M13;
            matrix1.M14 = matrix1.M14 / matrix2.M14;
            matrix1.M21 = matrix1.M21 / matrix2.M21;
            matrix1.M22 = matrix1.M22 / matrix2.M22;
            matrix1.M23 = matrix1.M23 / matrix2.M23;
            matrix1.M24 = matrix1.M24 / matrix2.M24;
            matrix1.M31 = matrix1.M31 / matrix2.M31;
            matrix1.M32 = matrix1.M32 / matrix2.M32;
            matrix1.M33 = matrix1.M33 / matrix2.M33;
            matrix1.M34 = matrix1.M34 / matrix2.M34;
            matrix1.M41 = matrix1.M41 / matrix2.M41;
            matrix1.M42 = matrix1.M42 / matrix2.M42;
            matrix1.M43 = matrix1.M43 / matrix2.M43;
            matrix1.M44 = matrix1.M44 / matrix2.M44;
            return matrix1;
        }


        public static Matrix operator /(Matrix matrix, float divider)
        {
            float num = 1f / divider;
            matrix.M11 = matrix.M11 * num;
            matrix.M12 = matrix.M12 * num;
            matrix.M13 = matrix.M13 * num;
            matrix.M14 = matrix.M14 * num;
            matrix.M21 = matrix.M21 * num;
            matrix.M22 = matrix.M22 * num;
            matrix.M23 = matrix.M23 * num;
            matrix.M24 = matrix.M24 * num;
            matrix.M31 = matrix.M31 * num;
            matrix.M32 = matrix.M32 * num;
            matrix.M33 = matrix.M33 * num;
            matrix.M34 = matrix.M34 * num;
            matrix.M41 = matrix.M41 * num;
            matrix.M42 = matrix.M42 * num;
            matrix.M43 = matrix.M43 * num;
            matrix.M44 = matrix.M44 * num;
            return matrix;
        }


        public static bool operator ==(Matrix matrix1, Matrix matrix2)
        {
            return (
                matrix1.M11 == matrix2.M11 &&
                matrix1.M12 == matrix2.M12 &&
                matrix1.M13 == matrix2.M13 &&
                matrix1.M14 == matrix2.M14 &&
                matrix1.M21 == matrix2.M21 &&
                matrix1.M22 == matrix2.M22 &&
                matrix1.M23 == matrix2.M23 &&
                matrix1.M24 == matrix2.M24 &&
                matrix1.M31 == matrix2.M31 &&
                matrix1.M32 == matrix2.M32 &&
                matrix1.M33 == matrix2.M33 &&
                matrix1.M34 == matrix2.M34 &&
                matrix1.M41 == matrix2.M41 &&
                matrix1.M42 == matrix2.M42 &&
                matrix1.M43 == matrix2.M43 &&
                matrix1.M44 == matrix2.M44
                );
        }


        public static bool operator !=(Matrix matrix1, Matrix matrix2)
        {
            return (
                matrix1.M11 != matrix2.M11 ||
                matrix1.M12 != matrix2.M12 ||
                matrix1.M13 != matrix2.M13 ||
                matrix1.M14 != matrix2.M14 ||
                matrix1.M21 != matrix2.M21 ||
                matrix1.M22 != matrix2.M22 ||
                matrix1.M23 != matrix2.M23 ||
                matrix1.M24 != matrix2.M24 ||
                matrix1.M31 != matrix2.M31 ||
                matrix1.M32 != matrix2.M32 ||
                matrix1.M33 != matrix2.M33 ||
                matrix1.M34 != matrix2.M34 ||
                matrix1.M41 != matrix2.M41 ||
                matrix1.M42 != matrix2.M42 ||
                matrix1.M43 != matrix2.M43 ||
                matrix1.M44 != matrix2.M44
                );
        }


        public static Matrix operator *(Matrix matrix1, Matrix matrix2)
        {
            var m11 = (((matrix1.M11 * matrix2.M11) + (matrix1.M12 * matrix2.M21)) + (matrix1.M13 * matrix2.M31)) + (matrix1.M14 * matrix2.M41);
            var m12 = (((matrix1.M11 * matrix2.M12) + (matrix1.M12 * matrix2.M22)) + (matrix1.M13 * matrix2.M32)) + (matrix1.M14 * matrix2.M42);
            var m13 = (((matrix1.M11 * matrix2.M13) + (matrix1.M12 * matrix2.M23)) + (matrix1.M13 * matrix2.M33)) + (matrix1.M14 * matrix2.M43);
            var m14 = (((matrix1.M11 * matrix2.M14) + (matrix1.M12 * matrix2.M24)) + (matrix1.M13 * matrix2.M34)) + (matrix1.M14 * matrix2.M44);
            var m21 = (((matrix1.M21 * matrix2.M11) + (matrix1.M22 * matrix2.M21)) + (matrix1.M23 * matrix2.M31)) + (matrix1.M24 * matrix2.M41);
            var m22 = (((matrix1.M21 * matrix2.M12) + (matrix1.M22 * matrix2.M22)) + (matrix1.M23 * matrix2.M32)) + (matrix1.M24 * matrix2.M42);
            var m23 = (((matrix1.M21 * matrix2.M13) + (matrix1.M22 * matrix2.M23)) + (matrix1.M23 * matrix2.M33)) + (matrix1.M24 * matrix2.M43);
            var m24 = (((matrix1.M21 * matrix2.M14) + (matrix1.M22 * matrix2.M24)) + (matrix1.M23 * matrix2.M34)) + (matrix1.M24 * matrix2.M44);
            var m31 = (((matrix1.M31 * matrix2.M11) + (matrix1.M32 * matrix2.M21)) + (matrix1.M33 * matrix2.M31)) + (matrix1.M34 * matrix2.M41);
            var m32 = (((matrix1.M31 * matrix2.M12) + (matrix1.M32 * matrix2.M22)) + (matrix1.M33 * matrix2.M32)) + (matrix1.M34 * matrix2.M42);
            var m33 = (((matrix1.M31 * matrix2.M13) + (matrix1.M32 * matrix2.M23)) + (matrix1.M33 * matrix2.M33)) + (matrix1.M34 * matrix2.M43);
            var m34 = (((matrix1.M31 * matrix2.M14) + (matrix1.M32 * matrix2.M24)) + (matrix1.M33 * matrix2.M34)) + (matrix1.M34 * matrix2.M44);
            var m41 = (((matrix1.M41 * matrix2.M11) + (matrix1.M42 * matrix2.M21)) + (matrix1.M43 * matrix2.M31)) + (matrix1.M44 * matrix2.M41);
            var m42 = (((matrix1.M41 * matrix2.M12) + (matrix1.M42 * matrix2.M22)) + (matrix1.M43 * matrix2.M32)) + (matrix1.M44 * matrix2.M42);
            var m43 = (((matrix1.M41 * matrix2.M13) + (matrix1.M42 * matrix2.M23)) + (matrix1.M43 * matrix2.M33)) + (matrix1.M44 * matrix2.M43);
            var m44 = (((matrix1.M41 * matrix2.M14) + (matrix1.M42 * matrix2.M24)) + (matrix1.M43 * matrix2.M34)) + (matrix1.M44 * matrix2.M44);
            matrix1.M11 = m11;
            matrix1.M12 = m12;
            matrix1.M13 = m13;
            matrix1.M14 = m14;
            matrix1.M21 = m21;
            matrix1.M22 = m22;
            matrix1.M23 = m23;
            matrix1.M24 = m24;
            matrix1.M31 = m31;
            matrix1.M32 = m32;
            matrix1.M33 = m33;
            matrix1.M34 = m34;
            matrix1.M41 = m41;
            matrix1.M42 = m42;
            matrix1.M43 = m43;
            matrix1.M44 = m44;
            return matrix1;
        }


        public static Matrix operator *(Matrix matrix, float scaleFactor)
        {
            matrix.M11 = matrix.M11 * scaleFactor;
            matrix.M12 = matrix.M12 * scaleFactor;
            matrix.M13 = matrix.M13 * scaleFactor;
            matrix.M14 = matrix.M14 * scaleFactor;
            matrix.M21 = matrix.M21 * scaleFactor;
            matrix.M22 = matrix.M22 * scaleFactor;
            matrix.M23 = matrix.M23 * scaleFactor;
            matrix.M24 = matrix.M24 * scaleFactor;
            matrix.M31 = matrix.M31 * scaleFactor;
            matrix.M32 = matrix.M32 * scaleFactor;
            matrix.M33 = matrix.M33 * scaleFactor;
            matrix.M34 = matrix.M34 * scaleFactor;
            matrix.M41 = matrix.M41 * scaleFactor;
            matrix.M42 = matrix.M42 * scaleFactor;
            matrix.M43 = matrix.M43 * scaleFactor;
            matrix.M44 = matrix.M44 * scaleFactor;
            return matrix;
        }


        public static Matrix operator -(Matrix matrix1, Matrix matrix2)
        {
            matrix1.M11 = matrix1.M11 - matrix2.M11;
            matrix1.M12 = matrix1.M12 - matrix2.M12;
            matrix1.M13 = matrix1.M13 - matrix2.M13;
            matrix1.M14 = matrix1.M14 - matrix2.M14;
            matrix1.M21 = matrix1.M21 - matrix2.M21;
            matrix1.M22 = matrix1.M22 - matrix2.M22;
            matrix1.M23 = matrix1.M23 - matrix2.M23;
            matrix1.M24 = matrix1.M24 - matrix2.M24;
            matrix1.M31 = matrix1.M31 - matrix2.M31;
            matrix1.M32 = matrix1.M32 - matrix2.M32;
            matrix1.M33 = matrix1.M33 - matrix2.M33;
            matrix1.M34 = matrix1.M34 - matrix2.M34;
            matrix1.M41 = matrix1.M41 - matrix2.M41;
            matrix1.M42 = matrix1.M42 - matrix2.M42;
            matrix1.M43 = matrix1.M43 - matrix2.M43;
            matrix1.M44 = matrix1.M44 - matrix2.M44;
            return matrix1;
        }


        public static Matrix operator -(Matrix matrix)
        {
            matrix.M11 = -matrix.M11;
            matrix.M12 = -matrix.M12;
            matrix.M13 = -matrix.M13;
            matrix.M14 = -matrix.M14;
            matrix.M21 = -matrix.M21;
            matrix.M22 = -matrix.M22;
            matrix.M23 = -matrix.M23;
            matrix.M24 = -matrix.M24;
            matrix.M31 = -matrix.M31;
            matrix.M32 = -matrix.M32;
            matrix.M33 = -matrix.M33;
            matrix.M34 = -matrix.M34;
            matrix.M41 = -matrix.M41;
            matrix.M42 = -matrix.M42;
            matrix.M43 = -matrix.M43;
            matrix.M44 = -matrix.M44;
            return matrix;
        }


        public static Matrix Subtract(Matrix matrix1, Matrix matrix2)
        {
            matrix1.M11 = matrix1.M11 - matrix2.M11;
            matrix1.M12 = matrix1.M12 - matrix2.M12;
            matrix1.M13 = matrix1.M13 - matrix2.M13;
            matrix1.M14 = matrix1.M14 - matrix2.M14;
            matrix1.M21 = matrix1.M21 - matrix2.M21;
            matrix1.M22 = matrix1.M22 - matrix2.M22;
            matrix1.M23 = matrix1.M23 - matrix2.M23;
            matrix1.M24 = matrix1.M24 - matrix2.M24;
            matrix1.M31 = matrix1.M31 - matrix2.M31;
            matrix1.M32 = matrix1.M32 - matrix2.M32;
            matrix1.M33 = matrix1.M33 - matrix2.M33;
            matrix1.M34 = matrix1.M34 - matrix2.M34;
            matrix1.M41 = matrix1.M41 - matrix2.M41;
            matrix1.M42 = matrix1.M42 - matrix2.M42;
            matrix1.M43 = matrix1.M43 - matrix2.M43;
            matrix1.M44 = matrix1.M44 - matrix2.M44;
            return matrix1;
        }


        public static void Subtract(ref Matrix matrix1, ref Matrix matrix2, out Matrix res)
        {
            Matrix result = Matrix.identity;

            result.M11 = matrix1.M11 - matrix2.M11;
            result.M12 = matrix1.M12 - matrix2.M12;
            result.M13 = matrix1.M13 - matrix2.M13;
            result.M14 = matrix1.M14 - matrix2.M14;
            result.M21 = matrix1.M21 - matrix2.M21;
            result.M22 = matrix1.M22 - matrix2.M22;
            result.M23 = matrix1.M23 - matrix2.M23;
            result.M24 = matrix1.M24 - matrix2.M24;
            result.M31 = matrix1.M31 - matrix2.M31;
            result.M32 = matrix1.M32 - matrix2.M32;
            result.M33 = matrix1.M33 - matrix2.M33;
            result.M34 = matrix1.M34 - matrix2.M34;
            result.M41 = matrix1.M41 - matrix2.M41;
            result.M42 = matrix1.M42 - matrix2.M42;
            result.M43 = matrix1.M43 - matrix2.M43;
            result.M44 = matrix1.M44 - matrix2.M44;

            res = result;
        }


        public override string ToString()
        {
            return "{" + String.Format("M11:{0} M12:{1} M13:{2} M14:{3}", M11, M12, M13, M14) + "}"
                + " {" + String.Format("M21:{0} M22:{1} M23:{2} M24:{3}", M21, M22, M23, M24) + "}"
                + " {" + String.Format("M31:{0} M32:{1} M33:{2} M34:{3}", M31, M32, M33, M34) + "}"
                + " {" + String.Format("M41:{0} M42:{1} M43:{2} M44:{3}", M41, M42, M43, M44) + "}";
        }


        public static Matrix Transpose(Matrix matrix)
        {
            Matrix ret;
            Transpose(ref matrix, out ret);
            return ret;
        }


        public static void Transpose(ref Matrix matrix, out Matrix res)
        {
            Matrix ret = Matrix.identity;

            ret.M11 = matrix.M11;
            ret.M12 = matrix.M21;
            ret.M13 = matrix.M31;
            ret.M14 = matrix.M41;

            ret.M21 = matrix.M12;
            ret.M22 = matrix.M22;
            ret.M23 = matrix.M32;
            ret.M24 = matrix.M42;

            ret.M31 = matrix.M13;
            ret.M32 = matrix.M23;
            ret.M33 = matrix.M33;
            ret.M34 = matrix.M43;

            ret.M41 = matrix.M14;
            ret.M42 = matrix.M24;
            ret.M43 = matrix.M34;
            ret.M44 = matrix.M44;

            res = ret;
        }

        private static void FindDeterminants(ref Matrix matrix, out float major,
                                             out float minor1, out float minor2, out float minor3, out float minor4, out float minor5, out float minor6,
                                             out float minor7, out float minor8, out float minor9, out float minor10, out float minor11, out float minor12)
        {
            float det1 = (float)matrix.M11 * (float)matrix.M22 - (float)matrix.M12 * (float)matrix.M21;
            float det2 = (float)matrix.M11 * (float)matrix.M23 - (float)matrix.M13 * (float)matrix.M21;
            float det3 = (float)matrix.M11 * (float)matrix.M24 - (float)matrix.M14 * (float)matrix.M21;
            float det4 = (float)matrix.M12 * (float)matrix.M23 - (float)matrix.M13 * (float)matrix.M22;
            float det5 = (float)matrix.M12 * (float)matrix.M24 - (float)matrix.M14 * (float)matrix.M22;
            float det6 = (float)matrix.M13 * (float)matrix.M24 - (float)matrix.M14 * (float)matrix.M23;
            float det7 = (float)matrix.M31 * (float)matrix.M42 - (float)matrix.M32 * (float)matrix.M41;
            float det8 = (float)matrix.M31 * (float)matrix.M43 - (float)matrix.M33 * (float)matrix.M41;
            float det9 = (float)matrix.M31 * (float)matrix.M44 - (float)matrix.M34 * (float)matrix.M41;
            float det10 = (float)matrix.M32 * (float)matrix.M43 - (float)matrix.M33 * (float)matrix.M42;
            float det11 = (float)matrix.M32 * (float)matrix.M44 - (float)matrix.M34 * (float)matrix.M42;
            float det12 = (float)matrix.M33 * (float)matrix.M44 - (float)matrix.M34 * (float)matrix.M43;

            major = (float)(det1 * det12 - det2 * det11 + det3 * det10 + det4 * det9 - det5 * det8 + det6 * det7);
            minor1 = (float)det1;
            minor2 = (float)det2;
            minor3 = (float)det3;
            minor4 = (float)det4;
            minor5 = (float)det5;
            minor6 = (float)det6;
            minor7 = (float)det7;
            minor8 = (float)det8;
            minor9 = (float)det9;
            minor10 = (float)det10;
            minor11 = (float)det11;
            minor12 = (float)det12;
        }

        public float[] GetData()
        {
            return new float[] { M11, M12, M13, M14, M21, M22, M23, M24, M31, M32, M33, M34, M41, M42, M43, M44 };
        }

        public int GetSizeOfData()
        {
            return sizeof(float) * 16;
        }
    }
}
