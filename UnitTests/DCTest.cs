using NUnit.Framework;
using System;

// We're testing DC
using Astron.DClass;

namespace UnitTests
{

	[TestFixture()]
	public class DCTest
	{
		File dcFile = new File();
	
		[Test()]
		public void TestParser()
		{
			// Parse (raise errors if something's wrong)
			Parser parser = new Parser();
			parser.Parse(new string[] { "../../resources/unit_test.dc" }, dcFile);
		
			// We'll assume things aren't correct if the hash is wrong.
		}
	
		[Test()]
		public void TestHash()
		{
			// Expected hash
			uint expectedHash = 0x00;
			uint hash = dcFile.GetHash();
		
			// Check equal
			Assert.AreEqual(expectedHash, hash, "#P03");
		}
	
		[Test()]
		public void TestLegacyHash()
		{
			// Expected hash
			uint expectedHash = 0x02;
			uint hash = LegacyHashGenerator.GenerateHash(dcFile);
		
			// Check equal
			Assert.AreEqual(expectedHash, hash, "#P04");
		}
	}
}
