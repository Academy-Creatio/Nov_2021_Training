using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using Terrasoft.Core;
using Terrasoft.Core.Tasks;
using Terrasoft.Web.Common;
using Training.Interfaces;

namespace Training
{
	[ServiceContract]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class CreatioWsTemplate : BaseService, IReadOnlySessionState
	{
		#region Methods : REST
		// http://k_krylov_n:7020/0/rest/CreatioWsTemplate/GetMethodname?a=10&b=20
		[OperationContract]
		[WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json,
			BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
		public PersonModel GetMethodname(PersonModel person)
		{
			var myHeader = HttpContextAccessor.GetInstance().Request.Headers["X-MyHeader"];
			var context = HttpContextAccessor.GetInstance();
			//context.Response.StatusCode = 503;

			context.Response.AddHeader("X-MyHeader-2", "Value for the header");
			WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Created;

			person.Name = person.Name + $" {myHeader}";
			return person;
		}

		#endregion

		[OperationContract]
		[WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare,
			ResponseFormat = WebMessageFormat.Json)]
		public Stream ReceiveStream(Stream stream)
		{
			return ParseQueryParameters(stream);
		}


		[OperationContract]
		[WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped,
			ResponseFormat = WebMessageFormat.Json)]
		public PersonModel Test2(PersonModel person)
		{
			return person;
		}



		[OperationContract]
		[WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare,
			ResponseFormat = WebMessageFormat.Json)]
		public string ProcessDataCommand(PersonModel person)
		{

			//Only relevant for annonymous ws
			//SessionHelper.SpecifyWebOperationIdentity(HttpContextAccessor.GetInstance(), UserConnection.CurrentUser);
			Task.StartNewWithUserConnection<LongRunningTask, PersonModel> (person);
			return "Done";
		}

		private Stream ParseQueryParameters(Stream stream)
		{
			PersonModel _person = new PersonModel();
			using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8))
			{
				FormReader form = new FormReader(streamReader.ReadToEnd());
				Dictionary<string, Microsoft.Extensions.Primitives.StringValues> kvp = form.ReadForm();
				foreach (KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues> item in kvp)
				{
					if (!string.IsNullOrEmpty(item.Value))
					{
						_person.GetType().GetProperty(item.Key).SetValue(_person, item.Value.ToString());
					}
				}
			}
			return GetResponseStream("Ok plain text");
		}

		private MemoryStream GetResponseStream(string message)
		{
			

			byte[] data = Encoding.UTF8.GetBytes(message);
			string contentType = "plain/text; charset=utf-8;";

			var HttpContext = HttpContextAccessor.GetInstance();

			HttpContext.Response.ContentType = contentType;
			HttpContext.Response.StatusCode = 200;
			HttpContext.Response.AddHeader("Content-Length", data.Length.ToString());
			return new MemoryStream(data);
		}
	}


	public class LongRunningTask : IBackgroundTask<PersonModel>, IUserConnectionRequired
	{

		private UserConnection _userConnection;

		public void Run(PersonModel parameters)
		{

			IProcessEngine processEngine = _userConnection.ProcessEngine;
			var processExecutor = processEngine.ProcessExecutor;

			Dictionary<string, string> inParamas = new Dictionary<string, string>();
			inParamas.Add("Name", "Kirill from WS2");
			inParamas.Add("Age", "40");
			inParamas.Add("Email", "k.krylov@creatio.com.us");

			var x = processExecutor.Execute("Process_StartFromWs", inParamas, new[] {"contactId"} );
			var y = "";
		}

		public void SetUserConnection(UserConnection userConnection)
		{
			_userConnection = userConnection;
		}
	}




	[DataContract]
	public class PersonModel
	{
		[DataMember(Name = "name", IsRequired = true, Order = 1)]
		public string Name { get; set; }

		[DataMember(Name = "age", IsRequired = true, Order = 2)]
		public string Age { get; set; }

		[DataMember(Name = "email", IsRequired = true, Order = 3)]
		public string Email { get; set; }
	}
}



