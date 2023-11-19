﻿#region License and information
/* * * * *
 * 
 * Extension file for the SimpleJSON framework for better support of some common
 * .NET types. It does only work together with the SimpleJSON.cs
 * It provides direct conversion support for types like decimal, char, byte,
 * sbyte, short, ushort, uint, DateTime, TimeSpan and Guid. In addition there
 * are conversion helpers for converting an array of number values into a byte[]
 * or a List<byte> as well as converting an array of string values into a string[]
 * or List<string>.
 * Finally there are some additional type conversion operators for some nullable
 * types like short?, int?, float?, double?, long? and bool?. They will actually
 * assign a JSONNull value when it's null or a JSONNumber when it's not.
 * 
 * The MIT License (MIT)
 * 
 * Copyright (c) 2020 Markus Göbel (Bunny83)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 * 
 * * * * */

#endregion License and information

namespace SimpleJSON
{
    using System.Collections.Generic;
    using System.Globalization;
    public partial class JSONNode
    {
        #region Decimal
        public virtual decimal AsDecimal
        {
            get
            {
                if (!decimal.TryParse(Value, out var result))
                    result = 0;
                return result;
            }
            set => Value = value.ToString();
        }

        public static implicit operator JSONNode(decimal aDecimal) => new JSONString(aDecimal.ToString());

        public static implicit operator decimal(JSONNode aNode) => aNode.AsDecimal;
        #endregion Decimal

        #region Char
        public virtual char AsChar
        {
            get
            {
                if (IsString && Value.Length > 0)
                    return Value[0];
                if (IsNumber)
                    return (char)AsInt;
                return '\0';
            }
            set
            {
                if (IsString)
                    Value = value.ToString();
                else if (IsNumber)
                    AsInt = value;
            }
        }

        public static implicit operator JSONNode(char aChar) => new JSONString(aChar.ToString());

        public static implicit operator char(JSONNode aNode) => aNode.AsChar;
        #endregion Decimal

        #region UInt
        public virtual uint AsUInt
        {
            get => (uint)AsDouble;
            set => AsDouble = value;
        }

        public static implicit operator JSONNode(uint aUInt) => new JSONNumber(aUInt);

        public static implicit operator uint(JSONNode aNode) => aNode.AsUInt;
        #endregion UInt

        #region Byte
        public virtual byte AsByte
        {
            get => (byte)AsInt;
            set => AsInt = value;
        }

        public static implicit operator JSONNode(byte aByte) => new JSONNumber(aByte);

        public static implicit operator byte(JSONNode aNode) => aNode.AsByte;
        #endregion Byte
        #region SByte
        public virtual sbyte AsSByte
        {
            get => (sbyte)AsInt;
            set => AsInt = value;
        }

        public static implicit operator JSONNode(sbyte aSByte) => new JSONNumber(aSByte);

        public static implicit operator sbyte(JSONNode aNode) => aNode.AsSByte;
        #endregion SByte

        #region Short
        public virtual short AsShort
        {
            get => (short)AsInt;
            set => AsInt = value;
        }

        public static implicit operator JSONNode(short aShort) => new JSONNumber(aShort);

        public static implicit operator short(JSONNode aNode) => aNode.AsShort;
        #endregion Short
        #region UShort
        public virtual ushort AsUShort
        {
            get => (ushort)AsInt;
            set => AsInt = value;
        }

        public static implicit operator JSONNode(ushort aUShort) => new JSONNumber(aUShort);

        public static implicit operator ushort(JSONNode aNode) => aNode.AsUShort;
        #endregion UShort

        #region DateTime
        public virtual System.DateTime AsDateTime
        {
            get
            {
                if (!System.DateTime.TryParse(Value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                    result = new System.DateTime(0);
                return result;
            }
            set => Value = value.ToString(CultureInfo.InvariantCulture);
        }

        public static implicit operator JSONNode(System.DateTime aDateTime) => new JSONString(aDateTime.ToString(CultureInfo.InvariantCulture));

        public static implicit operator System.DateTime(JSONNode aNode) => aNode.AsDateTime;
        #endregion DateTime
        #region TimeSpan
        public virtual System.TimeSpan AsTimeSpan
        {
            get
            {
                if (!System.TimeSpan.TryParse(Value, CultureInfo.InvariantCulture, out var result))
                    result = new System.TimeSpan(0);
                return result;
            }
            set => Value = value.ToString();
        }

        public static implicit operator JSONNode(System.TimeSpan aTimeSpan) => new JSONString(aTimeSpan.ToString());

        public static implicit operator System.TimeSpan(JSONNode aNode) => aNode.AsTimeSpan;
        #endregion TimeSpan

        #region Guid
        public virtual System.Guid AsGuid
        {
            get
            {
                _ = System.Guid.TryParse(Value, out var result);
                return result;
            }
            set => Value = value.ToString();
        }

        public static implicit operator JSONNode(System.Guid aGuid) => new JSONString(aGuid.ToString());

        public static implicit operator System.Guid(JSONNode aNode) => aNode.AsGuid;
        #endregion Guid

        #region ByteArray
        public virtual byte[] AsByteArray
        {
            get
            {
                if (this.IsNull || !this.IsArray)
                    return null;
                int count = Count;
                byte[] result = new byte[count];
                for (int i = 0; i < count; i++)
                    result[i] = this[i].AsByte;
                return result;
            }
            set
            {
                if (!IsArray || value == null)
                    return;
                Clear();
                for (int i = 0; i < value.Length; i++)
                    Add(value[i]);
            }
        }

        public static implicit operator JSONNode(byte[] aByteArray) => new JSONArray { AsByteArray = aByteArray };

        public static implicit operator byte[](JSONNode aNode) => aNode.AsByteArray;
        #endregion ByteArray
        #region ByteList
        public virtual List<byte> AsByteList
        {
            get
            {
                if (this.IsNull || !this.IsArray)
                    return null;
                int count = Count;
                List<byte> result = new List<byte>(count);
                for (int i = 0; i < count; i++)
                    result.Add(this[i].AsByte);
                return result;
            }
            set
            {
                if (!IsArray || value == null)
                    return;
                Clear();
                for (int i = 0; i < value.Count; i++)
                    Add(value[i]);
            }
        }

        public static implicit operator JSONNode(List<byte> aByteList) => new JSONArray { AsByteList = aByteList };

        public static implicit operator List<byte>(JSONNode aNode) => aNode.AsByteList;
        #endregion ByteList

        #region StringArray
        public virtual string[] AsStringArray
        {
            get
            {
                if (this.IsNull || !this.IsArray)
                    return null;
                int count = Count;
                string[] result = new string[count];
                for (int i = 0; i < count; i++)
                    result[i] = this[i].Value;
                return result;
            }
            set
            {
                if (!IsArray || value == null)
                    return;
                Clear();
                for (int i = 0; i < value.Length; i++)
                    Add(value[i]);
            }
        }

        public static implicit operator JSONNode(string[] aStringArray) => new JSONArray { AsStringArray = aStringArray };

        public static implicit operator string[](JSONNode aNode) => aNode.AsStringArray;
        #endregion StringArray
        #region StringList
        public virtual List<string> AsStringList
        {
            get
            {
                if (this.IsNull || !this.IsArray)
                    return null;
                int count = Count;
                List<string> result = new List<string>(count);
                for (int i = 0; i < count; i++)
                    result.Add(this[i].Value);
                return result;
            }
            set
            {
                if (!IsArray || value == null)
                    return;
                Clear();
                for (int i = 0; i < value.Count; i++)
                    Add(value[i]);
            }
        }

        public static implicit operator JSONNode(List<string> aStringList) => new JSONArray { AsStringList = aStringList };

        public static implicit operator List<string>(JSONNode aNode) => aNode.AsStringList;
        #endregion StringList

        #region NullableTypes
        public static implicit operator JSONNode(int? aValue)
        {
            if (aValue == null)
                return JSONNull.CreateOrGet();
            return new JSONNumber((int)aValue);
        }
        public static implicit operator int?(JSONNode aNode)
        {
            if (aNode == null || aNode.IsNull)
                return null;
            return aNode.AsInt;
        }

        public static implicit operator JSONNode(float? aValue)
        {
            if (aValue == null)
                return JSONNull.CreateOrGet();
            return new JSONNumber((float)aValue);
        }
        public static implicit operator float?(JSONNode aNode)
        {
            if (aNode == null || aNode.IsNull)
                return null;
            return aNode.AsFloat;
        }

        public static implicit operator JSONNode(double? aValue)
        {
            if (aValue == null)
                return JSONNull.CreateOrGet();
            return new JSONNumber((double)aValue);
        }
        public static implicit operator double?(JSONNode aNode)
        {
            if (aNode == null || aNode.IsNull)
                return null;
            return aNode.AsDouble;
        }

        public static implicit operator JSONNode(bool? aValue)
        {
            if (aValue == null)
                return JSONNull.CreateOrGet();
            return new JSONBool((bool)aValue);
        }
        public static implicit operator bool?(JSONNode aNode)
        {
            if (aNode == null || aNode.IsNull)
                return null;
            return aNode.AsBool;
        }

        public static implicit operator JSONNode(long? aValue)
        {
            if (aValue == null)
                return JSONNull.CreateOrGet();
            return new JSONNumber((long)aValue);
        }
        public static implicit operator long?(JSONNode aNode)
        {
            if (aNode == null || aNode.IsNull)
                return null;
            return aNode.AsLong;
        }

        public static implicit operator JSONNode(short? aValue)
        {
            if (aValue == null)
                return JSONNull.CreateOrGet();
            return new JSONNumber((short)aValue);
        }
        public static implicit operator short?(JSONNode aNode)
        {
            if (aNode == null || aNode.IsNull)
                return null;
            return aNode.AsShort;
        }
        #endregion NullableTypes
    }
}
