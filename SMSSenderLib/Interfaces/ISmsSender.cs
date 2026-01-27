using System;
using System.Collections.Generic;
using System.Text;

namespace SMSSenderLib.Interfaces
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string to, string message);
        Task SendSmsAsync(List<string> recipients, string message);
    }
}
