using System.Threading.Tasks;
using Common.Attributes;
using Common.Containers;
using Common.Receivers;
using Template.Models;

namespace Template.Controllers
{
    public class WebTestReceiver : WebServiceReceiver
    {
        private readonly ITestClass _testClass;

        public WebTestReceiver() 
        {
            _testClass = IoC.Resolve<ITestClass>();
        }

        public async Task<string> Alaki([QFromHeader] string accept)
        {
            //CurrentRequest

            var a = await _testClass.Test("test: ");

            await _testClass.Test2();

            var b = _testClass.Test3();

            return a;
        }
    }
}