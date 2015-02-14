using System;
using System.Runtime.InteropServices;

namespace Astron
{
	namespace DClass
	{
		public enum NumberType
		{
			NONE,
			INT,
			UINT,
			FLOAT,
		};

		// Represents ANY number (any data-type)
		[StructLayout(LayoutKind.Explicit)]
		public struct Number
		{
			[FieldOffset(0)]
			public NumberType
				type;

			[FieldOffset(1)]
			public long
				integer;
			[FieldOffset(1)]
			public ulong
				uinteger;
			[FieldOffset(1)]
			public double
				floating;

			public Number(int n)
			{
				type = NumberType.INT;

				integer = (long)n;
				uinteger = 0;
				floating = 0.0;
			}

			public Number(long n)
			{
				type = NumberType.INT;
				integer = n;
				uinteger = 0;
				floating = 0.0;
			}

			public Number(uint n)
			{
				type = NumberType.UINT;
				uinteger = (ulong)n;
				integer = 0;
				floating = 0.0;
			}

			public Number(ulong n)
			{
				type = NumberType.UINT;
				uinteger = n;
				integer = 0;
				floating = 0.0;
			}

			public Number(float n)
			{
				type = NumberType.FLOAT;
				floating = (double)n;
				uinteger = 0;
				integer = 0;
			}

			public Number(double n)
			{
				type = NumberType.FLOAT;
				floating = n;
				uinteger = 0;
				integer = 0;
			}

			public override bool Equals(Object obj)
			{
				if (obj == (Object)null)
				{
					return false;
				}

				if (obj.GetType() != typeof(Number))
				{
					return false;
				}

				Number rhs = (Number)obj;
				return ((rhs.type == type) && (rhs.uinteger == uinteger));
			}

			public static bool operator==(Number lhs, Number rhs)
			{
				return ((lhs.type == rhs.type) && (lhs.uinteger == rhs.uinteger));
			}

			public static bool operator!=(Number lhs, Number rhs)
			{
				return !((lhs.type == rhs.type) && (lhs.uinteger == rhs.uinteger));
			}
		}

// A NumericRange represents a range of integer or floating-point values.
// This is used to limit numeric types; or array, string, or blob sizes.
		public struct NumericRange
		{
			public NumberType type;
			public Number min;
			public Number max;
	
			// Constructors
			public NumericRange(int _min, int _max)
			{
				type = NumberType.INT;
				min = new Number(_min);
				max = new Number(_max);
			}

			public NumericRange(long _min, long _max)
			{
				type = NumberType.INT;
				min = new Number(_min);
				max = new Number(_max);
			}

			public NumericRange(uint _min, uint _max)
			{
				type = NumberType.UINT;
				min = new Number(_min);
				max = new Number(_max);
			}

			public NumericRange(ulong _min, ulong _max)
			{
				type = NumberType.UINT;
				min = new Number(_min);
				max = new Number(_max);
			}

			public NumericRange(double _min, double _max)
			{
				type = NumberType.FLOAT;
				min = new Number(_min);
				max = new Number(_max);
			}

			// Contains
			public bool Contains(Number num)
			{
				switch (min.type)
				{
					case (NumberType.NONE):
						return true;

					case (NumberType.INT):
						return (min.integer <= num.integer && num.integer <= max.integer);

					case (NumberType.UINT):
						return (min.uinteger <= num.uinteger && num.uinteger <= max.uinteger);

					case (NumberType.FLOAT):
						return (min.floating <= num.floating && num.floating <= max.floating);
				}

				return false;
			}

			// Empty
			public bool IsEmpty()
			{
				return (type == NumberType.NONE);
			}
		};
	}
}