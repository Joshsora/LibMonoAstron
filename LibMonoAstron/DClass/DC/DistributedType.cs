/*
 * Unity DC Object - Written by Joshsora (Joshua Scott)

 * What does it do?
 * Represents a single object. (Struct or DClass)
 */

using System;
using System.Collections.Generic;

namespace Astron
{
	namespace DClass
	{
		public enum DType
		{
			/* Numeric Types */
			T_INT8,
			T_INT16,
			T_INT32,
			T_INT64,
			T_UINT8,
			T_UINT16,
			T_UINT32,
			T_UINT64,
			T_CHAR, // equivalent to uint8, except that it should be printed as a string
			T_FLOAT32,
			T_FLOAT64,
	
			/* Array Types */
			T_STRING,      // a human-printable string with fixed length
			T_VARSTRING,   // a human-printable string with variable length
			T_BLOB,        // any binary data stored as a string, fixed length
			T_VARBLOB,     // any binary data stored as a varstring, variable length
			T_ARRAY,       // any array with fixed byte-length (fixed array-size and element-length)
			T_VARARRAY,    // any array with variable array-size or variable length elements
	
			/* Complex Types */
			T_STRUCT,
			T_METHOD,
	
			// New additions should be added at the end to prevent the file hash from changing.
			T_INVALID
		}

		public class DistributedType
		{
			// Object properties
			protected DType type = DType.T_INVALID;
			protected uint size = 0;
			protected string alias = "";

			// DC Type
			public void SetDType(DType _type)
			{
				type = _type;
			}
	
			public DType GetDType()
			{
				return type;
			}

			// Size
			public bool HasFixedSize()
			{
				return size != 0;
			}

			public uint GetSize()
			{
				return size;
			}

			// Alias
			public bool HasAlias()
			{
				return (alias.Length > 0);
			}

			public void SetAlias(string _alias)
			{
				alias = _alias;
			}

			public string GetAlias()
			{
				return alias;
			}

			// Numeric
			public virtual NumericType AsNumeric()
			{
				return (NumericType)null;
			}

			// Array
			public virtual ArrayType AsArray()
			{
				return (ArrayType)null;
			}

			// Struct
			public virtual Struct AsStruct()
			{
				return (Struct)null;
			}

			// Method
			public virtual Method AsMethod()
			{
				return (Method)null;
			}

			// Clone
			public DistributedType Clone()
			{
				return (DistributedType)MemberwiseClone();
			}

			// Hash
			public virtual void GenerateHash(HashGenerator hashgen)
			{
				hashgen.AddInt((int)type);
				if (HasAlias())
				{
					hashgen.AddString(alias);
				}
			}
		}
	}
}
