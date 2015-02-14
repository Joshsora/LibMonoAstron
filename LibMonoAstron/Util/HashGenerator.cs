using System;
using System.Collections.Generic;

namespace Astron
{
	public static class PrimeNumberGenerator
	{
		private static List<int> primes = new List<int>() { 2 };

		public static uint Get(int n)
		{
			if (n < 0)
			{
				throw new ArgumentException("Invalid prime number index: " + n);
			}

			int candidate = primes [primes.Count - 1] + 1;
			while (primes.Count <= n)
			{
				bool maybePrime = true;
				int j = 0;
				while ((maybePrime) && (primes[j] * primes[j] <= candidate))
				{
					if ((primes [j] * (candidate / primes [j])) == candidate)
					{
						maybePrime = false;
					}
					j++;
				}

				if (maybePrime)
				{
					primes.Add(candidate);
				}
				candidate++;
			}

			return (uint)primes [n];
		}
	}

	public class HashGenerator
	{
		private uint hash;
		private int index;

		private const int MAX_PRIME_NUMBERS = 10000;

		public HashGenerator()
		{
			hash = 0;
			index = 0;
		}

		public void AddInt(int num)
		{
			hash += PrimeNumberGenerator.Get(index) * (uint)num;
			index = (index + 1) % MAX_PRIME_NUMBERS;
		}

		public void AddString(string text)
		{
			AddInt(text.Length);
			for (int i = 0; i < text.Length; i++)
			{
				AddInt((int)text [i]);
			}
		}

		public uint GetHash()
		{
			return hash & 0xffffffff;
		}
	}
}