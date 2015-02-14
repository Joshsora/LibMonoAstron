using System;

namespace Astron
{
	namespace DClass
	{
		// A NumericType can represent any of the basic number types (ie. integers, floats, etc).
		// A NumericType may also have a range and/or modulus to limit its possible values,
		// and/or a divisor representing a fixed-point numeric convention.
		// A divisor scales up any range or modulus to constrain up to (constraint * divisor).
		public class NumericType : DistributedType
		{
			uint divisor;
			double origModulus; 
			NumericRange origRange;
			Number modulus;
			NumericRange range;

			// Constructor
			public NumericType(DType _type)
			{
				divisor = 1;
				origModulus = 0.0;
				origRange = new NumericRange();
				type = _type;

				switch (type)
				{
					case (DType.T_CHAR):
					case (DType.T_INT8):
					case (DType.T_UINT8):
						size = sizeof(byte);
						break;
					case (DType.T_INT16):
					case (DType.T_UINT16):
						size = sizeof(short);
						break;
					case (DType.T_INT32):
					case (DType.T_UINT32):
						size = sizeof(int);
						break;
					case (DType.T_INT64):
					case (DType.T_UINT64):
						size = sizeof(long);
						break;
					case (DType.T_FLOAT32):
						size = sizeof(float);
						break;
					case (DType.T_FLOAT64):
						size = sizeof(double);
						break;
					default:
						type = DType.T_INVALID;
						break;
				}
			}

			// Divisor
			public bool SetDivisor(uint _divisor)
			{
				if (_divisor == 0)
				{
					return false;
				}

				divisor = _divisor;

				if (HasRange())
				{
					SetRange(origRange);
				}

				if (HasModulus())
				{
					SetModulus(origModulus);
				}

				return true;
			}

			public uint GetDivisor()
			{
				return divisor;
			}

			// Modulus
			public bool HasModulus()
			{
				return origModulus != 0.0;
			}

			public double GetModulus()
			{
				return origModulus;
			}

			public bool SetModulus(double _modulus)
			{
				if (_modulus <= 0.0)
				{
					return false;
				}

				double float_modulus = _modulus * divisor;
				ulong uint_modulus = (ulong)Math.Floor(_modulus * divisor + 0.5);

				switch (type)
				{
					case (DType.T_CHAR):
					case (DType.T_INT8):
					case (DType.T_UINT8):
						if ((uint_modulus < 1) || ((ushort)Byte.MaxValue + 1 < uint_modulus))
						{
							return false;
						}
						modulus = new Number(uint_modulus);
						break;
					case (DType.T_INT16):
					case (DType.T_UINT16):
						if ((uint_modulus < 1) || ((uint)UInt16.MaxValue + 1 < uint_modulus))
						{
							return false;
						}
						modulus = new Number(uint_modulus);
						break;
					case (DType.T_INT32):
					case (DType.T_UINT32):
						if ((uint_modulus < 1) || ((ulong)UInt32.MaxValue + 1L < uint_modulus))
						{
							return false;
						}
						modulus = new Number(uint_modulus);
						break;
					case (DType.T_INT64):
					case (DType.T_UINT64):
						if (uint_modulus < 1)
						{
							return false;
						}
						modulus = new Number(uint_modulus);
						break;
					case (DType.T_FLOAT32):
					case (DType.T_FLOAT64):
						modulus = new Number(uint_modulus);
						break;
					default:
						return false;
				}

				origModulus = _modulus;
				return true;
			}
	
			// Range
			public bool HasRange()
			{
				return !origRange.IsEmpty();
			}

			public NumericRange GetRange()
			{
				return origRange;
			}

			public bool SetRange(NumericRange _range)
			{
				if (range.type != NumberType.FLOAT)
				{
					return false;
				}

				origRange = _range;
				switch (type)
				{
					case (DType.T_INT8):
					case (DType.T_INT16):
					case (DType.T_INT32):
					case (DType.T_INT64):
						long min = (long)Math.Floor(range.min.floating * divisor + 0.5);
						long max = (long)Math.Floor(range.max.floating * divisor + 0.5);
						range = new NumericRange(min, max);
						break;
					case (DType.T_CHAR):
					case (DType.T_UINT8):
					case (DType.T_UINT16):
					case (DType.T_UINT32):
					case (DType.T_UINT64):
						ulong _min = (ulong)Math.Floor(range.min.floating * divisor + 0.5);
						ulong _max = (ulong)Math.Floor(range.max.floating * divisor + 0.5);
						range = new NumericRange(_min, _max);
						break;
					case (DType.T_FLOAT32):
					case (DType.T_FLOAT64):
						double __min = range.min.floating * divisor;
						double __max = range.max.floating * divisor;
						range = new NumericRange(__min, __max);
						break;
					default:
						return false;
				}

				return true;
			}

			// Numeric
			public override NumericType AsNumeric()
			{
				return this;
			}

			// Hash
			public override void GenerateHash(HashGenerator hashgen)
			{
				base.GenerateHash(hashgen);

				hashgen.AddInt((int)divisor);
				if (HasModulus())
				{
					hashgen.AddInt((int)modulus.integer);
				}

				if (HasRange())
				{
					hashgen.AddInt((int)range.min.integer);
					hashgen.AddInt((int)range.max.integer);
				}
			}
		}
	}
}
