using System;

namespace Astron
{
	namespace DClass
	{
		public class Field : KeywordList
		{
			protected DistributedType type;
			protected Struct owner;
			protected File file;
			protected string name;
			protected int id;

			protected bool hasDefaultValue;
			protected string defaultValue;

			// Constructor
			public Field(DistributedType _type, string _name)
			{
				owner = (Struct)null;
				id = 0;
				name = _name;
				type = _type;
				bool implicitValue;
				defaultValue = DefaultValue.CreateDefaultValue(type, out implicitValue);
				hasDefaultValue = !implicitValue;

				if (type == (DistributedType)null)
				{
					type = new DistributedType();
				}
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

			public bool SetName(string _name)
			{
				if ((owner != (Struct)null) && (owner.GetFieldByName(_name) != (Field)null))
				{
					return false;
				}
				
				name = _name;
				return true;
			}

			// Type
			public DistributedType GetDType()
			{
				return type;
			}

			public void SetDType(DistributedType _type)
			{
				type = _type;
				hasDefaultValue = false;
				defaultValue = DefaultValue.CreateDefaultValue(type);
			}

			// Struct
			public Struct GetStruct()
			{
				return owner;
			}

			public void SetStruct(Struct _owner)
			{
				owner = _owner;
			}

			// Default Value
			public bool HasDefaultValue()
			{
				return hasDefaultValue;
			}

			public string GetDefaultValue()
			{
				return defaultValue;
			}

			public bool SetDefaultValue(string _defaultValue)
			{
				hasDefaultValue = true;
				defaultValue = _defaultValue;
				return true;
			}

			// Molecular
			public virtual MolecularField AsMolecular()
			{
				return (MolecularField)null;
			}

			// Hash
			public override void GenerateHash(HashGenerator hashgen)
			{
				base.GenerateHash(hashgen);
				hashgen.AddInt(id);
				hashgen.AddString(name);
				type.GenerateHash(hashgen);
			}
		}
	}
}
