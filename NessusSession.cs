using System;
using System.Collections;
using System.Xml;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

namespace nessussharp
{
	public class NessusSession : IDisposable
	{
		public NessusSession(string host, int port = 8834){
			this.ServerHost = host;
			this.ServerPort = port;
		}

		public NessusSession(string username, string password, string host, int port = 8834) {
			
			this.ServerHost = host;
			this.ServerPort = port;
			JObject response = this.Authenticate (username, password);

			if (response ["error"] != null)

				throw new Exception (response ["error"].Value<string>());

			this.AuthenticationToken = response ["token"].Value<string>();

		}

		public string ServerHost { get; set; }

		public int ServerPort { get; set; }

		public string Username { get; set; }

		public string Password { get; set; }

		public string AuthenticationToken { get; set; }

		public JObject Authenticate (string username, string password) {

			if (username == string.Empty || password == string.Empty)
				throw new Exception ("Username and Password required");

			ServicePointManager.ServerCertificateValidationCallback = (s, cert, chain, ssl) => true;

			HttpWebRequest request = WebRequest.Create ("https://" + this.ServerHost + ":" + this.ServerPort + "/session") as HttpWebRequest;
			request.Method = "POST";
			request.Accept = "application/json";
			request.ContentType = "application/json";
			request.Proxy = new WebProxy ("192.168.1.52", 8080);

			JObject parameters = new JObject ();
			parameters ["username"] = username;
			parameters ["password"] = password;

			byte[] parmBytes = System.Text.Encoding.ASCII.GetBytes (parameters.ToString ());

			request.ContentLength = parmBytes.Length;

			request.GetRequestStream().Write (parmBytes, 0, parmBytes.Length);

			string response = string.Empty;
			try {
				using (StreamReader reader = new StreamReader (request.GetResponse ().GetResponseStream ()))
					response = reader.ReadToEnd ();
			} catch (WebException ex) {
				using (StreamReader reader = new StreamReader (ex.Response.GetResponseStream ()))
					response = reader.ReadToEnd ();
			}

			return JObject.Parse (response);
		}

		public void Dispose()
		{}
	}
}

