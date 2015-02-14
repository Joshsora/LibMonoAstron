using System;

namespace Astron
{
	namespace DClass
	{
		static class DefaultValue
		{
			public static string CreateDefaultValue(DistributedType dtype)
			{
				bool discard;
				return CreateDefaultValue(dtype, out discard);
			}

			public static string CreateDefaultValue(DistributedType dtype, out bool isImplicit)
			{
				if (dtype == (DistributedType)null)
				{
					isImplicit = true;
					return "";
				}

				isImplicit = true;
				switch (dtype.GetDType())
				{
					case (DType.T_INT8):
					case (DType.T_INT16):
					case (DType.T_INT32):
					case (DType.T_INT64):
					case (DType.T_CHAR):
					case (DType.T_UINT8):
					case (DType.T_UINT16):			
					case (DType.T_UINT32):
					case (DType.T_UINT64):
					case (DType.T_FLOAT32):
					case (DType.T_FLOAT64):
						{
							byte v = 0;
							return Convert.ToChar(v).ToString();
						}

					case (DType.T_ARRAY):
					case (DType.T_BLOB):
					case (DType.T_STRING):
						{
							ArrayType array = dtype.AsArray();
							ulong minArrayElements = array.GetRange().min.uinteger;
							return new string('\0', (int)minArrayElements);
						}

					case (DType.T_VARARRAY):
					case (DType.T_VARBLOB):
					case (DType.T_VARSTRING):
						{
							ArrayType array = dtype.AsArray();
							uint len = (uint)array.GetRange().min.uinteger;
							return Convert.ToChar(len).ToString() + new string('\0', (int)len);
						}

					case (DType.T_STRUCT):
						{
							Struct dstruct = dtype.AsStruct();
							int numFields = dstruct.GetFieldCount();

							string val = "";
							for (int i = 0; i < numFields; i++)
							{
								Field field = dstruct.GetField(i);
								if (field.HasDefaultValue())
								{
									isImplicit = false;
								}
								val += field.GetDefaultValue();
							}
							return val;
						}

					case (DType.T_METHOD):
						{
							Method dmethod = dtype.AsMethod();
							uint numParams = dmethod.GetParameterCount();
				
							string val = "";
							for (int i = 0; i < numParams; i++)
							{
								Parameter param = dmethod.GetParameter(i);
								if (param.HasDefaultValue())
								{
									isImplicit = false;
								}
								val += param.GetDefaultValue();
							}
							return val;
						}
					default:
						return "";
				}
			}
		}
	}
}
