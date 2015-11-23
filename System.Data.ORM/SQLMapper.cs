using System;
using System.Collections.Generic;
using System.Text;

namespace System.Data.ORM
{
    public class SqlMapper
    {
        public static DbType TypeConvert(object obj)
        {
            if (obj is string)
            {
                return DbType.String;
            }
            else if (obj is char)
            {
                return DbType.StringFixedLength;
            }
            else if (obj is byte)
            {
                return DbType.Byte;
            }
            else if (obj is sbyte)
            {
                return DbType.SByte;
            }
            else if (obj is int)
            {
                return DbType.Int32;
            }
            else if (obj is uint)
            {
                return DbType.UInt32;
            }
            else if (obj is short)
            {
                return DbType.Int16;
            }
            else if (obj is ushort)
            {
                return DbType.UInt16;
            }
            else if (obj is long)
            {
                return DbType.Int64;
            }
            else if (obj is ulong)
            {
                return DbType.UInt64;
            }
            else if (obj is double)
            {
                return DbType.Double;
            }
            else if (obj is float)
            {
                return DbType.Single;
            }
            else if (obj is decimal)
            {
                return DbType.Decimal;
            }
            else if (obj is bool)
            {
                return DbType.Boolean;
            }
            else if (obj is DateTime)
            {
                return DbType.DateTime;
            }
           
            return DbType.String; 
        }
    }
}
