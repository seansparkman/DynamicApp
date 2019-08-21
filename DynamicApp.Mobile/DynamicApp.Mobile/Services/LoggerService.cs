using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DynamicApp.Mobile.Services
{
    public class LoggerService
    {
        public async Task Error(string message, Exception ex)
        {
            await Error($"{message}\n{ex}");
        }

        public async Task Error(string message)
        {
            await Console.Error.WriteLineAsync(message);
        }

        public async Task Info(string message)
        {
#if (DEBUG || QA)
            await Console.Out.WriteLineAsync(message);
#endif
        }

        public async Task Trace(string message)
        {
#if (DEBUG || QA)
            await Console.Out.WriteLineAsync(message);
#endif
        }

        public async Task Warn(string message)
        {
            await Console.Out.WriteLineAsync(message);
        }
    }
}
