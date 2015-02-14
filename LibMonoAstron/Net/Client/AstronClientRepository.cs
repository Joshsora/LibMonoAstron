using System;
using System.Linq;
using Astron.DClass;

namespace Astron
{
	namespace Net
	{
		namespace Client
		{
			public class AstronClientRepository : Connection
			{
				public AstronClientRepository(string host, int port, string version, string[] dcFiles)
					: base(host, port, version, dcFiles)
				{
					Connect();
				}

				// Message senders
				void SendClientHello()
				{
				}

				// Default handlers
				protected override void OnConnectionSuccessful()
				{
					base.OnConnectionSuccessful();
					SendClientHello();
				}

				protected override void OnConnectionFailed()
				{
					base.OnConnectionFailed();
				}

				protected override void OnConnectionEjected(int error, string reason)
				{
					base.OnConnectionEjected(error, reason);
				}
			}
		}
	}
}