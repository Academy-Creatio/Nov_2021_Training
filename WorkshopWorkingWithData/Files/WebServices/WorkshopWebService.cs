using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using Terrasoft.Core;
using Terrasoft.Web.Common;
using WorkshopWorkingWithData.Files.DataOperations;

namespace WorkshopWorkingWithData
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class WorkshopWebService : BaseService
    {
        private ReadingData ReadingData { get; set; }
        private UpdatingData UpdatingData { get; set; }
        private InsertingData InsertingData { get; set; }
        private DeletingData DeletingData { get; set; }
        private Stopwatch Timer { get; set;}
        public WorkshopWebService()
        {
            ReadingData = new ReadingData(UserConnection ?? SystemUserConnection);
            UpdatingData = new UpdatingData(UserConnection ?? SystemUserConnection);
            InsertingData = new InsertingData(UserConnection ?? SystemUserConnection);
            DeletingData = new DeletingData(UserConnection ?? SystemUserConnection);
            Timer = new Stopwatch();
        }

        #region Properties
        private SystemUserConnection _systemUserConnection;
        private SystemUserConnection SystemUserConnection
        {
            get
            {
                return _systemUserConnection ??= (SystemUserConnection)AppConnection.SystemUserConnection;
            }
        }
        #endregion

        #region Methods : SELECT
        [OperationContract]
        [WebInvoke(Method = "GET", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Xml)]
        public Stream GetAllContacts(QuryType qt)
        {
            //http://k_krylov:7020/0/rest/WorkshopWebService/GetAllContacts?qt=0
            Timer.Start();
            Tuple<DataTable, string> result;
            switch (qt)
            {
                case QuryType.SELECT:
                    result = ReadingData.GetAllContactsSelect();
                    break;
                case QuryType.ESQ:
                    result = ReadingData.GetAllContactsEsq();
                    break;
                case QuryType.CustomQuery:
                    result = ReadingData.GetAllContactsCustomQuery();
                    break;
                default:
                    result = ReadingData.GetAllContactsSelect();
                    break;
            }
            Timer.Stop();

            string htmlPage = result.Item1.GetHtmlPage(Timer.ElapsedTicks, result.Item2, qt.ToString());
            return GetMemoryStream(htmlPage);
        }

        [OperationContract]
        [WebInvoke(Method = "GET", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Xml)]
        public Stream GetFilteredContacts(QuryType qt, string email)
        {
            //http://k_krylov:7020/0/rest/WorkshopWebService/GetFilteredContacts?qt=0&email=kirill.krylov@gmail.com	
            Timer.Start();
            Tuple<DataTable, string> result;
            switch (qt)
            {
                case QuryType.SELECT:
                   result = ReadingData.GetFilteredContactsSelect(email);
                    break;
                case QuryType.ESQ:
                    result = ReadingData.GetFilteredContactsEsq(email);
                    break;
                case QuryType.CustomQuery:
                    result = ReadingData.GetFilteredContactsCustomQuery(email);
                    break;
                default:
                    result = ReadingData.GetFilteredContactsSelect(email);
                    break;
            }
            Timer.Stop();

            string htmlPage = result.Item1.GetHtmlPage(Timer.ElapsedTicks, result.Item2, qt.ToString());
            return GetMemoryStream(htmlPage);
        }

        [OperationContract]
        [WebInvoke(Method = "GET", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Xml)]
        public Stream GetAllDataReverseJoin(QuryType qt)
        {
        // This is simple example of EntitySchemaQuery and use of reverse join expressions
        // Example to run: 
        // http://k_krylov_n:7020/0/rest/WorkshopWebService/GetAllDataReverseJoin?qt=1
            Timer.Start();
            Tuple<DataTable, string> result;
            switch (qt)
            {
                case QuryType.SELECT:
                    result = ReadingData.GetAllDataReverseJoinSelect();
                    break;
                case QuryType.ESQ:
                    result = ReadingData.GetAllDataReverseJoinEsq();
                    break;
                case QuryType.CustomQuery:
                    result = ReadingData.GetAllContactsCustomQuery();
                    break;
                default:
                    result = ReadingData.GetAllDataReverseJoinEsq();
                    break;
            }
            Timer.Stop();

            string htmlPage = result.Item1.GetHtmlPage(Timer.ElapsedTicks, result.Item2, qt.ToString());
            return GetMemoryStream(htmlPage);
        }

        [OperationContract]
        [WebInvoke(Method = "GET", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Xml)]
        public Stream GetContactsWithMinutes(QuryType qt, Guid ContactId)
        {
            // http://k_krylov_n:7020/0/rest/WorkshopWebService/GetContactsWithMinutes?qt=1&ContactId=410006e1-ca4e-4502-a9ec-e54d922d2c00
            Timer.Start();
            Tuple<DataTable, string> result;
            switch (qt)
            {
                case QuryType.SELECT:
                    result = ReadingData.GetContactsWithMinutesSelect(ContactId);
                    break;
                case QuryType.ESQ:
                    result = ReadingData.GetContactsWithMinutesEsq(ContactId);
                    break;
                case QuryType.CustomQuery:
                    result = ReadingData.GetAllContactsCustomQuery();
                    break;
                default:
                    result = ReadingData.GetAllDataReverseJoinEsq();
                    break;
            }
            Timer.Stop();

            string htmlPage = result.Item1.GetHtmlPage(Timer.ElapsedTicks, result.Item2, qt.ToString());
            return GetMemoryStream(htmlPage);
        }

        #endregion

        #region Methods : UPDATE
        [OperationContract]
        [WebInvoke(Method = "GET", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public string UpdateContactName(QuryType qt, Guid ContactId, string NewName)
        {
            // http://k_krylov_n:7020/0/rest/WorkshopWebService/UpdateContactName?qt=1&ContactId=410006e1-ca4e-4502-a9ec-e54d922d2c00&NewName=Kirill
            Timer.Start();
            string result = string.Empty;
            switch (qt)
            {
                case QuryType.UPDATE:
                    result = UpdatingData.UpdateContactUpdate(ContactId, NewName);
                    break;
                case QuryType.ESQ:
                    result = UpdatingData.UpdateContactEsq(ContactId, NewName);
                    break;
                default:
                    result = UpdatingData.UpdateContactEsq(ContactId, NewName);
                    break;
            }
            Timer.Stop();

            return result;
        }
        #endregion

        #region Methods: INSERT
        [OperationContract]
        [WebInvoke(Method = "GET", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public string InsertSeveralContacts(QuryType qt)
        {
            // http://k_krylov:7020/0/rest/WorkshopWebService/InsertSeveralContacts?qt=1
            Timer.Start();
            string result = string.Empty;
            switch (qt)
            {
                case QuryType.INSERT:
                    result = InsertingData.InsertSeveralContacts();
                    break;
                case QuryType.ESQ:
                    result = InsertingData.InsertSeveralContactsEsq();
                    break;
                default:
                    result = InsertingData.InsertSeveralContacts();
                    break;
            }
            Timer.Stop();

            return result;
        }
        #endregion

        #region Methods: DELETE
        [OperationContract]
        [WebInvoke(Method = "GET", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public string DeleteContact(QuryType qt, Guid ContactId)
        {
            // http://k_krylov:7020/0/rest/WorkshopWebService/DeleteContact?qt=1&ContactId=060b93d0-0b94-4967-af32-030ac0844ec9
            Timer.Start();
            string result = string.Empty;
            switch (qt)
            {
                case QuryType.INSERT:
                    result = DeletingData.DeleteContact(ContactId);
                    break;
                case QuryType.ESQ:
                    result = DeletingData.DeleteContactEsq(ContactId);
                    break;
                default:
                    result = DeletingData.DeleteContactEsq(ContactId);
                    break;
            }
            Timer.Stop();

            return result;
        }

        #endregion

        #region Methods : Private
        private MemoryStream GetMemoryStream(string htmlPage)
        {
            byte[] data = Encoding.UTF8.GetBytes(htmlPage);
            string contentType = "text/html; charset=utf-8";
            WebOperationContext.Current.OutgoingResponse.ContentType = contentType;
            WebOperationContext.Current.OutgoingResponse.ContentLength = data.Length;
            return new MemoryStream(data);
        }
        #endregion
    }
}
