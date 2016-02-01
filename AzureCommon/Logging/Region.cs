using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AzureCommon
{
    public class Region: IDisposable
    {
        private readonly Stopwatch stopwatch = new Stopwatch();

        private readonly string message;

        public Region(string message)
        {
            this.message = message;
            Log.Info($"{message} >>");
            stopwatch.Start();
        }

        public void Dispose()
        {
            stopwatch.Stop();
            Log.Info($"{message} << in {stopwatch.Elapsed}");
        }
    }
}
