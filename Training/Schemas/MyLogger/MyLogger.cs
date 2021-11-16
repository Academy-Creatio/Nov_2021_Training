using global::Common.Logging;
using Terrasoft.Core.Factories;
using Training.Interfaces;

namespace Training
{
	/// <summary>
	/// HOWTO: Custom Logging with NLog
	/// </summary>
	/// <remarks>
	/// See <seealso href="https://github.com/Academy-Creatio/TrainingProgramm/wiki/Custom-Logging-with-NLog"></seealso>
	/// </remarks>
	/// 
	[DefaultBinding(typeof(IMyLogging))]
	class MyLogger : IMyLogging
	{

		private ILog _log = LogManager.GetLogger("GuidedLearningLogger");


		public void LogError(string value)
		{
			_log.Error(value);
		}

		public void LogInfo(string value)
		{
			_log.Info(value);
		}
	}
}