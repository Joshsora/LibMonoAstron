using System;

namespace Astron
{
	namespace DClass
	{
		// An ArrayType represents an array of some other kind of object, meaning
		// this parameter type accepts an arbitrary (or possibly fixed) number of
		// nested fields, all of which are of the same type.
		// Strings and blobs are arrays with char and uint8 elements respectively.
		public class ArrayType : DistributedType
		{
			DistributedType elementType;
			uint arraySize;
			NumericRange arrayRange;

			// Constructor
			public ArrayType(DistributedType _elementType, NumericRange _arrayRange = new NumericRange())
			{
				elementType = _elementType;
				arrayRange = _arrayRange;
				if (_elementType == (DistributedType)null)
				{
					elementType = new DistributedType();
				}

				if (arrayRange.IsEmpty())
				{
					arraySize = 0;
					arrayRange.min.uinteger = UInt64.MinValue;
					arrayRange.max.uinteger = UInt64.MaxValue;
				} else if (arrayRange.min == arrayRange.max)
				{
					arraySize = (uint)arrayRange.min.uinteger;
				} else
				{
					arraySize = 0;
				}

				if (elementType.HasFixedSize())
				{
					type = DType.T_ARRAY;
					size = arraySize * elementType.GetSize();
				} else
				{
					type = DType.T_VARARRAY;
					size = 0;
				}

				if (elementType.GetDType() == DType.T_CHAR)
				{
					if (type == DType.T_ARRAY)
					{
						type = DType.T_STRING;
					} else
					{
						type = DType.T_VARSTRING;
					}
				} else if (elementType.GetDType() == DType.T_UINT8)
				{
					if (type == DType.T_ARRAY)
					{
						type = DType.T_BLOB;
					} else
					{
						type = DType.T_VARBLOB;
					}
				}
			}

			// Element Type
			public DistributedType GetElementType()
			{
				return elementType;
			}

			// Size
			public uint GetArraySize()
			{
				return arraySize;
			}
	
			// Range
			public bool HasRange()
			{
				return !arrayRange.IsEmpty();
			}
	
			public NumericRange GetRange()
			{
				return arrayRange;
			}

			// Array
			public override ArrayType AsArray()
			{
				return this;
			}

			// Hash
			public override void GenerateHash(HashGenerator hashgen)
			{
				base.GenerateHash(hashgen);
				elementType.GenerateHash(hashgen);
				if (HasRange())
				{
					hashgen.AddInt((int)arrayRange.min.integer);
					hashgen.AddInt((int)arrayRange.max.integer);
				} else
				{
					hashgen.AddInt((int)arraySize);
				}
			}
		}
	}
}
