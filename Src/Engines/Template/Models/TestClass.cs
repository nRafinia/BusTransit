using System;
using System.Threading.Tasks;
using Common.Attributes;

namespace Template.Models
{
    public class TestClass : ITestClass
    {
        [CacheOutput]
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