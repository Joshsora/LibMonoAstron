using System;

namespace Astron
{
	namespace DClass
	{
		// Generates the hash that would have been generated with the old version of the
		// Disney DC parser. ("Legacy" hash) Allows a CLIENT_HELLO to be sent to a regular
		// Astron daemon with no manual_dc_hash.
		public static class LegacyHashGenerator
		{
			static HashGenerator hashgen;

			// Old DType
			enum LegacyType
			{
				L_INT8,
				L_INT16,
				L_INT32,
				L_INT64,
				
				L_UINT8,
				L_UINT16,
				L_UINT32,
				L_UINT64,
				
				L_FLOAT64,
				
				L_STRING,
				L_BLOB,
				
				L_CHAR = 19,
				
				L_INVALID = 20
			}

			// Keyword
			struct LegacyKeyword
			{
				public string keyword;
				public int flag;

				public LegacyKeyword(string _keyword, int _flag)
				{
					keyword = _keyword;
					flag = _flag;
				}
			}

			static LegacyKeyword[] LegacyKeywords = new LegacyKeyword[] {
				new LegacyKeyword("required", 0x0001),
				new LegacyKeyword("broadcast", 0x0002),
				new LegacyKeyword("ownrecv", 0x0004),
				new LegacyKeyword("ram", 0x0008),
				new LegacyKeyword("db", 0x0010),
				new LegacyKeyword("clsend", 0x0020),
				new LegacyKeyword("clrecv", 0x0040),
				new LegacyKeyword("ownsend", 0x0080),
				new LegacyKeyword("airecv", 0x0100),
				new LegacyKeyword("", 0)
			};

			// File
			static void HashFile(File file)
			{
				hashgen.AddInt(1); // (dc_virtual_inheritance && dc_sort_inheritance_by_file)
				hashgen.AddInt(file.GetStructCount() + file.GetClassCount());

				int typeCount = file.GetTypeCount();
				for (int i = 0; i < typeCount; i++)
				{
					DistributedType type = file.GetTypeByID(i);
					if (type.AsStruct() == (Struct)null)
					{
						throw new Exception("Cannot generate legacy hash for this file.");
					}

					Struct dstruct = type.AsStruct();
					if (dstruct.AsClass() != (Class)null)
					{
						HashClass(dstruct.AsClass());
					} else
					{
						HashStruct(dstruct);
					}
				}
			}

			// Hash
			static void HashClass(Class dclass)
			{
				hashgen.AddString(dclass.GetName());

				int numParents = dclass.GetParentCount();
				hashgen.AddInt(numParents);
				for (int i = 0; i < numParents; i++)
				{
					hashgen.AddInt(dclass.GetParent(i).GetID());
				}

				if (dclass.HasConstructor())
				{
					HashField(dclass.GetConstructor());
				}

				int numFields = dclass.GetBaseFieldCount();
				hashgen.AddInt(numFields);
				for (int i = 0; i < numFields; i++)
				{
					HashField(dclass.GetBaseField(i));
				}
			}

			// Struct
			static void HashStruct(Struct dstruct)
			{
				hashgen.AddString(dstruct.GetName());
				hashgen.AddInt(1);
				hashgen.AddInt(0);

				int numFields = dstruct.GetFieldCount();
				hashgen.AddInt(numFields);
				for (int i = 0; i < numFields; i++)
				{
					HashField(dstruct.GetField(i));
				}
			}

			// Field
			static void HashField(Field field)
			{
				// Handle molecular
				if (field.AsMolecular() != (MolecularField)null)
				{
					hashgen.AddString(field.GetName());
					hashgen.AddInt(field.GetID());
					/*
					MolecularField mol = field.AsMolecular();
					int numFields = mol.GetFieldCount();

					hashgen.AddInt(numFields);
					for (int i = 0; i < numFields; i++)
					{
						HashField(mol.GetField(i));
					}
					*/

					return;
				}

				// Handle atomic
				if (field.GetDType().GetDType() == DType.T_METHOD)
				{
					hashgen.AddString(field.GetName());
					hashgen.AddInt(field.GetID());

					Method method = field.GetDType().AsMethod();
					uint numParams = method.GetParameterCount();

					hashgen.AddInt((int)numParams);
					for (int i = 0; i < numParams; i++)
					{
						HashParameter(method.GetParameter(i));
					}

					HashKeywords(field);
					return;
				}

				if (field.GetKeywordCount() != 0)
				{
					HashKeywords(field);
				}

				HashType(field.GetDType());
			}

			// Parameter
			static void HashParameter(Parameter parameter)
			{
				HashType(parameter.GetDType());
			}

			// Keywords
			static void HashKeywords(KeywordList list)
			{
				uint numKeywords = list.GetKeywordCount();
				int flags = 0;

				for (int i = 0; i < numKeywords; i++)
				{
					bool setFlag = false;
					string keyword = list.GetKeyword(i);
					for (int j = 0; j < LegacyKeywords.Length; j++)
					{
						if (keyword == LegacyKeywords [j].keyword)
						{
							flags |= LegacyKeywords [j].flag;
							setFlag = true;
							break;
						}
					}

					if (!setFlag)
					{
						flags = ~0;
						break;
					}
				}

				if (flags != ~0)
				{
					hashgen.AddInt(flags);
				} else
				{
					hashgen.AddInt((int)numKeywords);
					for (int i = 0; i < numKeywords; i++)
					{
						hashgen.AddString(list.GetKeyword(i));
					}
				}
			}

			// Type
			static void HashType(DistributedType type)
			{
				switch (type.GetDType())
				{
					case (DType.T_STRUCT):
						Struct dstruct = type.AsStruct();
						if (dstruct.AsClass() != (Class)null)
						{
							HashClass(dstruct.AsClass());
						} else
						{
							HashStruct(dstruct);
						}
						break;
					case (DType.T_BLOB):
					case (DType.T_VARBLOB):
						break;
					case (DType.T_STRING):
					case (DType.T_VARSTRING):
						if (type.GetAlias() == "string")
						{
							hashgen.AddInt((int)LegacyType.L_STRING);
						} else
						{
							hashgen.AddInt((int)LegacyType.L_CHAR);
						}
						hashgen.AddInt(1);

						ArrayType str = type.AsArray();
						if (str.HasRange())
						{
							NumericRange rng = str.GetRange();
							hashgen.AddInt(1);
							hashgen.AddInt((int)rng.min.uinteger);
							hashgen.AddInt((int)rng.max.uinteger);
						}
						break;

					case (DType.T_INT8):
						hashgen.AddInt((int)LegacyType.L_INT8);
						HashNumericType(type.AsNumeric());
						break;
					case (DType.T_INT16):
						hashgen.AddInt((int)LegacyType.L_INT16);
						HashNumericType(type.AsNumeric());
						break;
					case (DType.T_INT32):
						hashgen.AddInt((int)LegacyType.L_INT32);
						HashNumericType(type.AsNumeric());
						break;
					case (DType.T_INT64):
						hashgen.AddInt((int)LegacyType.L_INT64);
						HashNumericType(type.AsNumeric());
						break;

					case (DType.T_UINT8):
						hashgen.AddInt((int)LegacyType.L_UINT8);
						HashNumericType(type.AsNumeric());
						break;
					case (DType.T_UINT16):
						hashgen.AddInt((int)LegacyType.L_UINT16);
						HashNumericType(type.AsNumeric());
						break;
					case (DType.T_UINT32):
						hashgen.AddInt((int)LegacyType.L_UINT32);
						HashNumericType(type.AsNumeric());
						break;
					case (DType.T_UINT64):
						hashgen.AddInt((int)LegacyType.L_UINT64);
						HashNumericType(type.AsNumeric());
						break;

					case (DType.T_CHAR):
						hashgen.AddInt((int)LegacyType.L_CHAR);
						HashNumericType(type.AsNumeric());
						break;
					
					case (DType.T_FLOAT64):
						hashgen.AddInt((int)LegacyType.L_FLOAT64);
						break;

					case (DType.T_FLOAT32):
						// Skip
						break;
					case (DType.T_INVALID):
						throw new Exception("Cannot generate legacy hash for this file.");
						break;
				}
			}

			// Numeric
			static void HashNumericType(NumericType number)
			{
			}

			// Generator
			public static uint GenerateHash(File file)
			{
				hashgen = new HashGenerator();
				HashFile(file);

				return hashgen.GetHash();
			}
		}
	}
}
