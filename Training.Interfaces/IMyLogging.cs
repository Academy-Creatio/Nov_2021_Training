using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Training.Interfaces
{
	public interface IMyLogging
	{
		void LogInfo(string value);
		void LogError(string value);
	}
}
