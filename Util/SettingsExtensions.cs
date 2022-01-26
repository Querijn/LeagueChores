using System;
using System.Linq;
using System.Reflection;

namespace LeagueChores.Util
{
	internal class SettingsExtensions
	{
		public object this[string name]
		{
			get
			{
				var properties = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

				foreach (var property in properties)
					if (property.Name == name)
						return property.GetValue(this);

				throw new ArgumentException("Can't find property");
			}
			set
			{
				var properties = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

				foreach (var property in properties)
				{
					if (property.Name != name)
						continue;
					property.SetValue(this, value);
					return;
				}

				throw new ArgumentException("Can't find property");
			}
		}
	}
}
