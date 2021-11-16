using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrasoft.Core;
using Terrasoft.Core.Factories;
using Training.Interfaces;

namespace Training
{

	[DefaultBinding(typeof(ICalculator))]
	public class Calculator : ICalculator
	{
		private readonly UserConnection _userConnction;

		public Calculator(UserConnection userConnction)
		{
			_userConnction = userConnction;
		}

		public int Add(int a, int b)
		{
			return a + b;
		}

		public int Multiply(int a, int b)
		{
			throw new NotImplementedException();
		}

		public int Sub(int a, int b)
		{
			return a - b;
		}

	}
}
