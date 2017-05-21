using Localization.Enums;
using Localization.Localize;
using System;
using System.Collections.Generic;
using System.Text;

namespace Vaerator.Helpers
{
    public class MessageSource
    {
        MessageType messageType;
        public MessageType MessageType { get { return messageType; } set { messageType = value; } }
        int messageIndex;
        List<string> messageKeys;
        string messageResourceName;
        static Random randOrder = new Random();

        public MessageSource(MessageType messageType, string messageResourceName, List<string> messageKeys)
        {
            this.messageType = messageType;
            this.messageResourceName = messageResourceName;
            messageIndex = 0;
            this.messageKeys = messageKeys;
            Scramble();
        }

        public string GetNext()
        {
            if (messageIndex >= messageKeys.Count) messageIndex = 0;
            return ResourceContainer.Instance.GetString(messageKeys[messageIndex++], messageResourceName);
        }

        public void SetMessageKeys(List<string> messageKeys)
        {
            this.messageKeys = messageKeys;
            Scramble();
        }

        public void AddMessageKey(string messageKey)
        {
            messageKeys.Add(messageKey);
            messageKeys.Insert(randOrder.Next(0, messageKey.Length), messageKey);
        }

        public void RemoveMessageKey(string messageKey)
        {
            messageKeys.Remove(messageKey);
        }

        public void ForceScramble()
        {
            Scramble();
        }

        void Scramble()
        {
            string temp;
            int length = messageKeys.Count;
            int randIndex;

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    randIndex = randOrder.Next(0, length);
                    temp = messageKeys[j];
                    messageKeys[j] = messageKeys[randIndex];
                    messageKeys[randIndex] = temp;
                }
            }
        }
    }
}
