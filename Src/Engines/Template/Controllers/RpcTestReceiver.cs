using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Attributes;
using Common.QMessageModels;
using Common.Receivers;
using Common.Tools;
using Template.Models;

namespace Template.Controllers
{
    public class RpcTestReceiver : RPCReceiver, ITestClass
    {

        public RpcTestReceiver()
        {
        }

        public async Task<string> Test(string a)
        {
            return a + new Random().Next(1000).ToString();
        }

        public async Task Test2()
        {
        }

        public string Test3()
        {
            return new Random().Next(1000).ToString();
        }
    }
}