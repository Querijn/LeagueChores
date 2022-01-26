using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueChores
{
	internal class LockFile
	{
		public string protocol { get; private set; }
		public ushort port { get; private set; }
		public string password { get; private set; }
		public string domain { get; private set; }

		public LockFile(string source)
		{
			// TODO: Validation
			string[] parts = source.Split(':');

			port = ushort.Parse(parts[2]);
			password = parts[3];
			protocol = parts[4];

			domain = String.Format("{0}://127.0.0.1:{1}", protocol, port);
		}
	}
}
