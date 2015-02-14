using System;
using System.Collections.Generic;

namespace Astron
{
	namespace DClass
	{
		public class Class : Struct
		{
			Field constructor;
			List<Field> baseFields;
			Dictionary<string, Field> baseFieldsByName;
			List<Class> parents;
			List<Class> children;

			// Constructor
			public Class(File file, string name)
				: base(file, name)
			{
				constructor = (Field)null;
				baseFields = new List<Field>();
				baseFieldsByName = new Dictionary<string, Field>();
				parents = new List<Class>();
				children = new List<Class>();
			}

			// Parents
			public int GetParentCount()
			{
				return parents.Count;
			}

			public void AddParent(Class parent)
			{
				parent.AddChild(this);
				parents.Add(parent);

				for (int i = 0; i < parent.GetFieldCount(); i++)
				{
					AddInheritedField(parent, parent.GetField(i));
				}
			}

			public Class GetParent(int n)
			{
				return parents [n];
			}

			// Children
			public int GetChildCount()
			{
				return children.Count;
			}

			void AddChild(Class child)
			{
				children.Add(child);
			}

			public Class GetChild(int n)
			{
				return children [n];
			}

			// Constructor
			public bool HasConstructor()
			{
				return (constructor != (Field)null);
			}

			public Field GetConstructor()
			{
				return constructor;
			}

			// Fields
			public int GetBaseFieldCount()
			{
				return baseFields.Count;
			}

			public Field GetBaseField(int n)
			{
				return baseFields [n];
			}

			public new bool AddField(Field field)
			{
				if (field == (Field)null)
				{
					return false;
				}

				if ((field.GetStruct() != (Struct)null) && (field.GetStruct() != this))
				{
					return false;
				}

				if (field.GetName() == "")
				{
					return false;
				}

				if (field.GetName() == name)
				{
					if (constructor != (Field)null)
					{
						return false;
					}

					if (field.AsMolecular() != (MolecularField)null)
					{
						return false;
					}

					if (baseFields.Count > 0)
					{
						return false;
					}

					field.SetStruct(this);
					constructor = field;

					file.AddField(field);
					fieldsByID.Add(field.GetID(), field);
					fieldsByName.Add(field.GetName(), field);
					return true;
				}

				if (baseFieldsByName.ContainsKey(field.GetName()))
				{
					return false;
				}

				baseFieldsByName.Add(field.GetName(), field);
				baseFields.Add(field);

				if (fieldsByName.ContainsKey(field.GetName()))
				{
					ShadowField(field);
				}

				field.SetStruct(this);
				fields.Add(field);

				file.AddField(field);
				fieldsByID.Add(field.GetID(), field);
				fieldsByName.Add(field.GetName(), field);

				if ((field.AsMolecular() == (MolecularField)null) &&
					(HasFixedSize() || fields.Count == 1))
				{
					if (field.GetDType().HasFixedSize())
					{
						size += field.GetDType().GetSize();
					} else
					{
						size = 0;
					}
				}

				for (int i = 0; i < children.Count; i++)
				{
					children [i].AddInheritedField(this, field);
				}

				return true;
			}

			void ShadowField(Field field)
			{
				if (HasFixedSize())
				{
					size -= field.GetDType().GetSize();
				}

				fieldsByID.Remove(field.GetID());
				fieldsByName.Remove(field.GetName());
				fields.Remove(field);

				for (int i = 0; i < children.Count; i++)
				{
					Class child = children [i];
					if (child.GetFieldByID(field.GetID()) == field)
					{
						child.ShadowField(field);
					}
				}
			}

			// Inherited Fields
			void AddInheritedField(Class parent, Field field)
			{
				if (baseFieldsByName.ContainsKey(field.GetName()))
				{
					return;
				}

				if (fieldsByName.ContainsKey(field.GetName()))
				{
					Field prevField = fieldsByName [field.GetName()];
					Struct parentB = prevField.GetStruct();
					for (int i = 0; i < parents.Count; i++)
					{
						if (parents [i] == parentB)
						{
							return;
						} else if (parents [i] == parent)
						{
							ShadowField(prevField);
						}
					}
				}

				fieldsByID.Add(field.GetID(), field);
				fieldsByName.Add(field.GetName(), field);

				if (fields.Count == 0)
				{
					fields.Add(field);
				} else
				{
					for (int i = fields.Count; i --> 0;)
					{
						if (fields [i].GetID() < field.GetID())
						{
							fields.Insert(i, field);
							break;
						}
					}
				}

				if (HasFixedSize() || fields.Count == 1)
				{
					if (field.GetDType().HasFixedSize())
					{
						size += field.GetDType().GetSize();
					} else
					{
						size = 0;
					}
				}

				for (int i = 0; i < children.Count; i++)
				{
					children [i].AddInheritedField(this, field);
				}
			}

			// Class
			public override Class AsClass()
			{
				return this;
			}

			// Hash
			public override void GenerateHash(HashGenerator hashgen)
			{
				hashgen.AddInt((int)type);
				if (HasAlias())
				{
					hashgen.AddString(alias);
				}

				hashgen.AddInt(parents.Count);
				for (int i = 0; i < parents.Count; i++)
				{
					hashgen.AddInt(parents [i].GetID());
				}

				if (HasConstructor())
				{
					constructor.GenerateHash(hashgen);
				}

				hashgen.AddInt(baseFields.Count);
				for (int i = 0; i < fields.Count; i++)
				{
					fields [i].GenerateHash(hashgen);
				}
			}
		}
	}
}