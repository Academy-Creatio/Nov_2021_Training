using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using Terrasoft.Core;
using Terrasoft.Web.Common;
using Training.Interfaces;

namespace Training
{
	[ServiceContract]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class CreatioWsTemplate : BaseService
	{
		#region Properties
		private SystemUserConnection _systemUserConnection;
		private SystemUserConnection SystemUserConnection
		{
			get
			{
				return _systemUserConnection ?? (_systemUserConnection = (SystemUserConnection)AppConnection.SystemUserConnection);
			}
		}
		#endregion

		#region Methods : REST
		// http://k_krylov_n:7020/0/rest/CreatioWsTemplate/GetMethodname?a=10&b=20
		[OperationContract]
		[WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
		public int GetMethodname(int a, int b)
		{

			var logger = Terrasoft.Core.Factories.ClassFactory.Get<IMyLogging>();
			logger.LogInfo("First log from WS");
			return 0;
		}

		#endregion

		#region Methods : Private

		#endregion
	}
}



