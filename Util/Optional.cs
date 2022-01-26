
using System;
using System.Runtime.Serialization;

namespace LeagueChores
{
	internal struct Optional<T>
	{
		private T valueCache;

		public bool hasValue { get; private set; }

		public T value
		{
			get
			{
				if (hasValue)
					return valueCache;
				else
					throw new InvalidOperationException();
			}
		}

		public T Get(Func<T> a = null)
		{
			if (hasValue)
				return valueCache;
			else if (a != null)
				return a();
			return default(T);
		}

		public Optional(T value)
		{
			this.valueCache = value;
			hasValue = true;
		}

		public static explicit operator T(Optional<T> optional)
		{
			return optional.value;
		}
		public static implicit operator Optional<T>(T value)
		{
			return new Optional<T>(value);
		}

		public override bool Equals(object obj)
		{
			if (obj is Optional<T>)
				return this.Equals((Optional<T>)obj);
			else
				return false;
		}

		public override int GetHashCode()
		{
			if (hasValue)
				return valueCache.GetHashCode();
			return 0;
		}

		public bool Equals(Optional<T> other)
		{
			if (hasValue && other.hasValue)
				return object.Equals(valueCache, other.valueCache);
			else
				return hasValue == other.hasValue;
		}

		public bool Equals(T other)
		{
			if (hasValue)
				return Equals(valueCache, other);
			else
				return false;
		}
	}
}
