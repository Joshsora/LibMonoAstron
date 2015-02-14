using System;
using System.Collections.Generic;

namespace Astron
{
	namespace DClass
	{
		public class Method : DistributedType
		{
			List<Parameter> parameters;
			Dictionary<string, Parameter> parametersByName;

			// Constructor
			public Method()
			{
				type = DType.T_METHOD;

				parameters = new List<Parameter>();
				parametersByName = new Dictionary<string, Parameter>();
			}

			// Parameters
			public uint GetParameterCount()
			{
				return (uint)parameters.Count;
			}

			public Parameter GetParameter(int n)
			{
				return parameters [n];
			}

			public Parameter GetParameterByName(string n)
			{
				if (parametersByName.ContainsKey(n))
				{
					return parametersByName [n];
				}

				return (Parameter)null;
			}

			public bool AddParamater(Parameter param)
			{
				if (param == (Parameter)null)
				{
					return false;
				}

				if (param.GetName() != "")
				{
					if (parametersByName.ContainsKey(param.GetName()))
					{
						return false;
					}

					parametersByName.Add(param.GetName(), param);
				}

				param.SetMethod(this);
				parameters.Add(param);

				if ((HasFixedSize()) || (parameters.Count == 1))
				{
					if (param.GetDType().HasFixedSize())
					{
						size += param.GetDType().GetSize();
					} else
					{
						size = 0;
					}
				}

				return true;
			}

			// Method
			public override Method AsMethod()
			{
				return this;
			}

			// Hash
			public override void GenerateHash(HashGenerator hashgen)
			{
				base.GenerateHash(hashgen);

				hashgen.AddInt(parameters.Count);
				for (int i = 0; i < parameters.Count; i++)
				{
					parameters [i].GenerateHash(hashgen);
				}
			}
		}
	}
}