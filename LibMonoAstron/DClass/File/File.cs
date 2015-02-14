using System;
using System.Collections.Generic;

namespace Astron
{
	namespace DClass
	{
		public class Import
		{
			string module;
			List<string> symbols;
			
			public Import(string moduleName)
			{
				module = moduleName;
				symbols = new List<string>();
			}

			public void AddSymbol(string symbol)
			{
				symbols.Add(symbol);
			}
		}

		public class File
		{
			List<Struct> structs;
			List<Class> classes;
			List<Import> imports;
			List<string> keywords;

			List<Field> fieldsByID;
			List<DistributedType> typesByID;
			Dictionary<string, DistributedType> typesByName;
			uint hash;

			// Constructor
			public File()
			{
				structs = new List<Struct>();
				classes = new List<Class>();
				imports = new List<Import>();
				keywords = new List<string>();

				fieldsByID = new List<Field>();
				typesByID = new List<DistributedType>();
				typesByName = new Dictionary<string, DistributedType>();
				hash = 0;
			}

			// Struct
			public int GetStructCount()
			{
				return structs.Count;
			}

			public Struct GetStruct(int n)
			{
				return structs [n];
			}

			public bool AddStruct(Struct dstruct)
			{
				if (dstruct.GetName() == "")
				{
					return false;
				}
				
				if (typesByName.ContainsKey(dstruct.GetName()))
				{
					return false;
				}

				dstruct.SetID(typesByID.Count);

				typesByName.Add(dstruct.GetName(), dstruct);
				typesByID.Add(dstruct);

				structs.Add(dstruct);
				return true;
			}

			// Class
			public int GetClassCount()
			{
				return classes.Count;
			}

			public Class GetClass(int n)
			{
				return classes [n];
			}

			public Class GetClassById(int id)
			{
				DistributedType dt = GetTypeByID(id);
				if (dt == (DistributedType)null)
				{
					return (Class)null;
				}
				if (dt.AsStruct() == (Struct)null)
				{
					return (Class)null;
				}

				return dt.AsStruct().AsClass();
			}

			public Class GetClassByName(string name)
			{
				DistributedType dt = GetTypeByName(name);
				if (dt == (DistributedType)null)
				{
					return (Class)null;
				}
				if (dt.AsStruct() == (Struct)null)
				{
					return (Class)null;
				}
				
				return dt.AsStruct().AsClass();
			}

			public bool AddClass(Class dclass)
			{
				if (dclass.GetName() == "")
				{
					return false;
				}

				if (typesByName.ContainsKey(dclass.GetName()))
				{
					return false;
				}

				dclass.SetID(typesByID.Count);

				typesByName.Add(dclass.GetName(), dclass);
				typesByID.Add(dclass);

				classes.Add(dclass);
				return true;
			}

			// Type
			public int GetTypeCount()
			{
				return typesByID.Count;
			}

			public DistributedType GetTypeByID(int id)
			{
				if (id < typesByID.Count)
				{
					return typesByID [id];
				}

				return (DistributedType)null;
			}

			public DistributedType GetTypeByName(string name)
			{
				if (typesByName.ContainsKey(name))
				{
					return typesByName [name];
				}

				return (DistributedType)null;
			}

			public bool AddTypedef(string name, DistributedType type)
			{
				if (name == "")
				{
					return false;
				}

				if (typesByName.ContainsKey(name))
				{
					return false;
				}

				typesByName.Add(name, type);
				typesByID.Add(type);
				return true;
			}

			// Field
			public Field GetFieldByID(int id)
			{
				if (id < fieldsByID.Count)
				{
					return fieldsByID [id];
				}

				return (Field)null;
			}

			public void AddField(Field field)
			{
				field.SetID(fieldsByID.Count);
				fieldsByID.Add(field);
			}

			// Imports
			public int GetImportCount()
			{
				return imports.Count;
			}

			public Import GetImport(int n)
			{
				return imports [n];
			}

			public void AddImport(Import import)
			{
				imports.Add(import);
			}

			// Keywords
			public int GetKeywordCount()
			{
				return keywords.Count;
			}

			public bool HasKeyword(string keyword)
			{
				if (keywords.Contains(keyword))
				{
					return true;
				}

				return false;
			}

			public string GetKeyword(int n)
			{
				return keywords [n];
			}

			public void AddKeyword(string keyword)
			{
				if (!HasKeyword(keyword))
				{
					keywords.Add(keyword);
				}
			}

			// Hash
			public uint GetLegacyHash()
			{
				hash = LegacyHashGenerator.GenerateHash(this);
				return hash;
			}

			public uint GetHash()
			{
				GenerateHash();
				return hash;
			}

			public void GenerateHash()
			{
				HashGenerator hashgen = new HashGenerator();

				hashgen.AddInt(classes.Count);
				for (int i = 0; i < classes.Count; i++)
				{
					classes [i].GenerateHash(hashgen);
				}

				hashgen.AddInt(structs.Count);
				for (int i = 0; i < structs.Count; i++)
				{
					structs [i].GenerateHash(hashgen);
				}

				hashgen.AddInt(keywords.Count);
				for (int i = 0; i < keywords.Count; i++)
				{
					hashgen.AddString(keywords [i]);
				}

				hash = hashgen.GetHash();
			}
		}
	}
}
