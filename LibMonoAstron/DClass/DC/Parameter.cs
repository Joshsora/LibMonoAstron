using System;

namespace Astron
{
	namespace DClass
	{
		public class Parameter
		{
			string name;
			string typeAlias;
			DistributedType type;
			Method method;

			bool hasDefaultValue;
			string defaultValue;

			// Constructor
			public Parameter(DistributedType _type, string _name)
			{
				name = _name;
				type = _type;
				method = (Method)null;
				hasDefaultValue = false;

				bool implicit_value;
				defaultValue = DefaultValue.CreateDefaultValue(type, out implicit_value);
				hasDefaultValue = !implicit_value;

				if (type == (DistributedType)null)
				{
					type = new DistributedType();
				}
			}

			// Name
			public string GetName()
			{
				return name;
			}

			public bool SetName(string _name)
			{
				if ((method != (Method)null) && (method.GetParameterByName(_name) != (Parameter)null))
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

			public bool SetDType(DistributedType _type)
			{
				if (_type == (DistributedType)null)
				{
					return false;
				}

				// No method types
				if (type.GetDType() == DType.T_METHOD)
				{
					return false;
				}

				// No class types
				if ((type.GetDType() == DType.T_STRUCT) && (type.AsStruct().AsClass() != (Class)null))
				{
					return false;
				}

				type = _type;
				hasDefaultValue = false;
				defaultValue = DefaultValue.CreateDefaultValue(type);

				return true;
			}

			// Method
			public Method GetMethod()
			{
				return method;
			}

			public void SetMethod(Method _method)
			{
				method = _method;
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

			// Hash
			public void GenerateHash(HashGenerator hashgen)
			{
				type.GenerateHash(hashgen);
			}
		}
	}
}
