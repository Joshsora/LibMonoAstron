using System;
using System.Collections.Generic;

namespace Astron
{
	namespace DClass
	{
		public class Struct : DistributedType
		{
			protected File file;
			protected string name;
			protected int id;

			protected List<Field> fields;
			protected Dictionary<string, Field> fieldsByName;
			protected Dictionary<int, Field> fieldsByID;

			// Constructors
			public Struct(File _file, string _name)
			{
				file = _file;
				name = _name;
				id = 0;
		
				type = DType.T_STRUCT;
				fields = new List<Field>();
				fieldsByName = new Dictionary<string, Field>();
				fieldsByID = new Dictionary<int, Field>();
			}

			protected Struct(File _file)
			{
				file = _file;
				name = "";
				id = 0;

				type = DType.T_STRUCT;
				fields = new List<Field>();
				fieldsByName = new Dictionary<string, Field>();
				fieldsByID = new Dictionary<int, Field>();
			}

			// ID
			public int GetID()
			{
				return id;
			}

			public void SetID(int _id)
			{
				id = _id;
			}

			// Name
			public string GetName()
			{
				return name;
			}

			// File
			public File GetFile()
			{
				return file;
			}

			// Fields
			public int GetFieldCount()
			{
				return fields.Count;
			}

			public Field GetField(int n)
			{
				return fields [n];
			}

			public Field GetFieldByID(int id)
			{
				if (fieldsByID.ContainsKey(id))
				{
					return fieldsByID [id];
				} else
				{
					return (Field)null;
				}
			}

			public Field GetFieldByName(string name)
			{
				if (fieldsByName.ContainsKey(name))
				{
					return fieldsByName [name];
				} else
				{
					return (Field)null;
				}
			}

			public bool AddField(Field field)
			{
				if (field == (Field)null)
				{
					return false;
				}

				if ((field.GetStruct() != null) && (field.GetStruct() != this))
				{
					return false;
				}

				if (field.AsMolecular() != (MolecularField)null)
				{
					return false;
				}

				if (field.GetDType().AsMethod() != (Method)null)
				{
					return false;
				}

				if (field.GetName() != "")
				{
					if (field.GetName() == name)
					{
						return false;
					}

					fieldsByName.Add(field.GetName(), field);
				}

				file.AddField(field);
				fieldsByID.Add(field.GetID(), field);

				fields.Add(field);
				if ((HasFixedSize()) || (fields.Count == 1))
				{
					if (field.GetDType().HasFixedSize())
					{
						size += field.GetDType().GetSize();
					} else
					{
						size = 0;
					}
				}

				return true;
			}

			// Struct0
			public override Struct AsStruct()
			{
				return this;
			}

			// Class
			public virtual Class AsClass()
			{
				return (Class)null;
			}

			// Hash
			public override void GenerateHash(HashGenerator hashgen)
			{
				base.GenerateHash(hashgen);

				hashgen.AddString(name);
				hashgen.AddInt(fields.Count);
				for (int i = 0; i < fields.Count; i++)
				{
					fields [i].GenerateHash(hashgen);
				}
			}
		}
	}
}
