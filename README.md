# BusTransit
Distributed Application Framework for .NET Core

Readme In progress...

Send request to bus and wait to response very easy

RPC Transmitter:

    public interface ITestClass
    {
        Task<string> Test(string a);
        Task Test2();
        string Test3();
    }

    public class TestClass : ITestClass
    {
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

    public class TemplateInstaller : IIoCInstaller
    {
        public void Install(WindsorContainer container, IMapper mapper)
        {
            var appSetting = IoC.Resolve<IAppSetting>();
            var test = RPCTransmitter<ITestClass>.Register(appSetting.GetSetting("RPCTestClass"));
            container.Register(Component.For<ITestClass>().Instance(test).LifestyleSingleton());        
        }
    }
    
    public class HomeController : Controller
    {
        private readonly ITestClass _testClass;

        public HomeController(ITestClass testClass) 
        {
            _testClass = IoC.Resolve<ITestClass>();
        }

        public async Task<string> Index()
        {
            var a = await _testClass.Test("test: ");
            await _testClass.Test2();
            var b = _testClass.Test3();
            return a;
        }
    }            
