using System;
using System.Net;
using System.Net.Sockets;
using System.IO;

using Astron.DClass;

namespace Astron
{
	namespace Net
	{
		public class Connection
		{
			protected Astron.DClass.File dcFile;

			protected TcpClient socket;
			protected NetworkStream stream;
			protected BinaryWriter writer;

			protected DatagramWriter dg;
			protected DatagramReader dgIterator;
			protected AstronStream astream;

			protected string host;
			protected int port;
			protected string version;

			protected bool connected = false;

			public Connection(string _host, int _port, string _version, string[] dcFiles)
			{
				host = _host;
				port = _port;
				version = _version;
				ReadDCFiles(dcFiles);
			}

			protected void ReadDCFiles(string[] dcFiles)
			{
				// Parse the network descriptions
				Parser dcParser = new Parser();
				dcParser.Parse(dcFiles, dcFile);
			}

			protected bool Connect()
			{
				if (connected)
				{
					// Close our connection?
					connected = false;
					socket.Close();
				}
				socket = new TcpClient();

				try
				{
					socket.Connect(host, port);
				} catch (SocketException e)
				{
					OnConnectionFailed();
					return false;
				}

				connected = true;

				// Open a stream
				stream = socket.GetStream();
				writer = new BinaryWriter(stream);
				astream = new AstronStream();

				// Writing, reading
				dg = new DatagramWriter(astream);
				dgIterator = new DatagramReader(stream);

				OnConnectionSuccessful();
				return true;
			}

			public void Send()
			{
				astream.Flush(writer);
			}

			protected virtual void OnConnectionSuccessful()
			{
			}

			protected virtual void OnConnectionFailed()
			{
			}

			protected virtual void OnConnectionEjected(int error, string reason)
			{
				connected = false;
				stream.Close();
				socket.Close();
			}

			protected virtual void OnConnectionLost()
			{
				OnConnectionLost();
			}
		}
	}
}
