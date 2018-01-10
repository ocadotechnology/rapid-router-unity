using NUnit.Framework;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]

namespace Tests
{
    public class CodeExecutorTest
    {

        [Test]
        public void CanExecuteMoreForwardCode()
        {
            string protobufCodeString = @"{""methods"":[{""name"":""start""}]}";
            Listener listener = new Listener();
            VehicleMover vehicleMover = new VehicleMover();
            CodeExecutor codeExecutor = new CodeExecutor(vehicleMover);
            listener.codeExecutor = codeExecutor;
            listener.Listen(protobufCodeString);
        }
    }
}
