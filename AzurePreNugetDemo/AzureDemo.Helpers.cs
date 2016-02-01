using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AzureCommon;
using Hyak.Common;

namespace AzurePreNugetDemo
{
    partial class AzureDemo
    {
        private static T HandleResult<T>(string region, Func<T> action, Func<T, string> toStringFunc)
        {
            using (new Region(region))
            {
                try
                {
                    T result = action();

                    if (result == null)
                    {
                        throw new Exception(Log.Error(region + " result: NULL"));
                    }

                    Log.Info(region + " result: " + (toStringFunc == null ? result.ToString() : toStringFunc(result)));

                    return result;
                }
                catch (CloudException e)
                {
                    var errMsg = region + " error: " + e.Message;
                    if (e.Error == null || string.IsNullOrEmpty(e.Error.Code))
                    {
                        Log.Error(errMsg);
                    }
                    else
                    {
                        Log.Error(errMsg + " - " + e.Error.Code + ": " + e.Error.Message);
                    }
                    throw;
                }
            }
        }

    }
}
