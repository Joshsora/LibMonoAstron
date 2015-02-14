using System;
using System.IO;

namespace Astron
{
	namespace Net
	{
		public class AstronStream : MemoryStream
		{
			public void Flush(BinaryWriter output)
			{
				output.Write((UInt16)Length);
				
				byte[] Payload = ToArray();
				
				output.Write(Payload);
				Seek(0, SeekOrigin.Begin);
				SetLength(0);
			}
		}
		
		public class DatagramWriter : BinaryWriter
		{
			public DatagramWriter(AstronStream output) : base(output)
			{
			}
			
			public override void Write(string s)
			{
				Write((UInt16)s.Length);
				Write(s.ToCharArray());
			}
		}
		
		public class DatagramReader : BinaryReader
		{
			public DatagramReader(Stream output) : base(output)
			{
			}
			
			public override string ReadString()
			{
				return System.Text.Encoding.Default.GetString(ReadBlob());
			}
			
			public byte[] ReadBlob()
			{
				return ReadBytes(ReadUInt16());
			}
		}

	}
}