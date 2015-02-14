using System;
using System.Collections.Generic;

namespace Astron
{
	namespace DClass
	{
		public class MolecularField : Field
		{
			// Constructor
			public MolecularField(DistributedType type, string name)
				: base(type, name)
			{

			}

			// Molecular
			public override MolecularField AsMolecular()
			{
				return this;
			}
		}
	}
}
