using Core.Interfaces;
using Core.Services;

namespace Tests
{
    public class BaseTest
    {
        protected readonly IScanner scanner;

        public BaseTest()
        {
            scanner = new Scanner();
        }
    }
}