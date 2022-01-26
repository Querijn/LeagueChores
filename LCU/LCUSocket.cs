using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using WebSocketSharp;

namespace LeagueChores
{
	public class LCUMessageEventArgs : EventArgs
	{
		public string eventName { get; private set; }
		public string uri { get; private set; }
		public JToken msg { get; private set; }

		public LCUMessageEventArgs(string eventName, string uri, JToken msg)
		{
			this.eventName = eventName;
			this.uri = uri;
			this.msg = msg;
		}
	}

	internal class LCUSocket
	{
		public delegate void LCUMessageEventHandler(object sender, LCUMessageEventArgs e);
		public event LCUMessageEventHandler onMessage;
		WebSocket m_connection = null;

		public LCUSocket()
		{
			LCU.onValid += (s, e) => SetCredentials();
			LCU.onDisconnect += (s, e) => UnsetCredentials();
		}

		public void SetCredentials()
		{
			m_connection = new WebSocket($"wss://127.0.0.1:{LCU.port}/", "wamp");
			m_connection.SetCredentials("riot", LCU.password, true);
			m_connection.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
			m_connection.SslConfiguration.ServerCertificateValidationCallback = delegate { return true; };
			m_connection.OnMessage += OnMessageReceived;
			m_connection.OnOpen += (s, e) => m_connection.Send("[5,\"OnJsonApiEvent\"]");
			m_connection.ConnectAsync();
		}

		public void UnsetCredentials()
		{
			if (m_connection != null)
			{
				m_connection.Close();
				m_connection = null;
			}
		}

		private void OnMessageReceived(object sender, MessageEventArgs e)
		{
			var msgs = JArray.Parse(e.Data);

			int type = 0;
			if (!int.TryParse(msgs[0].ToString(), out type) || type != 8)
				return;

			var eventData = msgs[2];
			var eventName = eventData["eventType"].ToString();
			var uri = eventData["uri"].ToString();
			// Debug.WriteLine($"Received an event: {eventName} {uri}");
			if (onMessage != null)
				onMessage(this, new LCUMessageEventArgs(eventName, uri, eventData["data"]));
		}
	}
}
