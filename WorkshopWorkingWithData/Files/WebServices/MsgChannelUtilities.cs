using System;
using Terrasoft.Core;
using Terrasoft.Messaging.Common;
//using global::Common.Logging;


namespace WorkshopWorkingWithData.Files.WebServices
{
    public static class MsgChannelUtilities
    {

        #region Constants
        public const string ProcessEngineSenderName = "ProcessEngine";

        [Obsolete("7.15.1 | Constant is not in use and will be removed in upcoming releases")]
        public const string ProcessEngineBackHistoryStateSenderName = "ProcessEngineBackHistoryState";
        #endregion

        #region Fields
        //private static readonly ILog _log = LogManager.GetLogger("WebSocket");
        #endregion

        #region Methods : Private
        private static bool CheckCanPostMessage(string userName, string senderName)
        {
            if (!MsgChannelManager.IsRunning)
            {
                //_log.WarnFormat("Can't post message to {0} from {1} while MsgChannelManager is not running", userName ?? "All", senderName);
                return false;
            }
            return true;
        }

        private static void PostMessageInternal(IMsgChannel channel, string sender, string msg)
        {
            IMsg simpleMessage = new SimpleMessage()
            {
                Id = Guid.NewGuid(),
                Body = msg
            };
            simpleMessage.Header.Sender = sender;
            //_log.Debug($"Channel {channel.Name} post for {sender} msg: {msg}");
            channel.PostMessage(simpleMessage);
        }
        #endregion

        #region Methods : Public
        /// <summary>
        /// Sends message to a specific user
        /// </summary>
        /// <param name="userConnection">userConnection</param>
        /// <param name="senderName">Sender Name</param>
        /// <param name="messageText">Message body</param>
        public static void PostMessage(UserConnection userConnection, string senderName, string messageText)
        {
            if (!CheckCanPostMessage(userConnection.CurrentUser.Name, senderName))
            {
                return;
            }
            MsgChannelManager channelManager = MsgChannelManager.Instance;
            IMsgChannel userChannel = channelManager.FindItemByUId(userConnection.CurrentUser.Id);
            if (userChannel != null)
            {
                PostMessageInternal(userChannel, senderName, messageText);
            }
            else
            {
                //_log.Info($"Channel not found for user {userConnection.CurrentUser.Name} from {senderName}");
            }
        }

        /// <summary>
        /// Sends message to all users
        /// </summary>
        /// <param name="senderName">Sender Name</param>
        /// <param name="messageText">Message Body</param>
        public static void PostMessageToAll(string senderName, string messageText)
        {
            if (!CheckCanPostMessage(null, senderName))
            {
                return;
            }
            MsgChannelManager channelManager = MsgChannelManager.Instance;
            foreach (IMsgChannel userChannel in channelManager.Channels.Values)
            {
                PostMessageInternal(userChannel, senderName, messageText);
            }
        }
        #endregion
    }
}
