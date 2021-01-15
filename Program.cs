using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame
{

	
	
	class NaturalNumber
	{
		int Num;
		int MaxNum;

		public void Add(int num)
		{
			Num = (num + Num).Clamp(0, MaxNum);
		}
		public bool IsMax() => Num == MaxNum;
		public bool IsZero() => Num == 0;

		public NaturalNumber(int num, int maxNum)
		{
			Num = num;
			MaxNum = maxNum;
		}
		public NaturalNumber(int num) : this(num, num)
		{ }

		public static implicit operator int(NaturalNumber n)
		{
			return n.Num;
		}
	}




	public static partial class ExtensionMethods
	{
		public static int Clamp(this int value, int min, int max)
		{
			return (value < min) ? min : (value > max) ? max : value;
		}
	}

	class Program
	{
		

		static void Main(string[] args)
		{
			Console.WriteLine()
		}
	}
}
