using System;
using Neo.SmartContract;
using System.IO;
using Neo.VM;
using Neo;


namespace DeBugToolkitWithSyscall
{
    class DebugWithSyscall
    {

        static void Main(string[] args)
        {
            string path1 = @"D:\Neo\test2.txt";
            string[] script1 = File.ReadAllLines(path1, System.Text.Encoding.Default);
            byte[] tempScript = new byte[(script1[0].Length * (script1.Length - 1))/2 + (script1[script1.Length - 1].Length)/2];
            for (int i = 0; i < script1.Length; i++)
            {
                byte[] output = ToByte.HexStringToByteArray(script1[i]);
                for (int j = 0; j < output.Length; j++)
                {
                    tempScript[script1[0].Length * i + j] = output[j];
                }
            }
            Script returnScript = tempScript;

            Console.WriteLine("DebugToolkit Started");
            //string path = @"D:\Neo\test1.txt";
            //string[] script = File.ReadAllLines(path, System.Text.Encoding.Default);
            //byte[] tempScript = new byte[script.Length];
            //Script returnScript;
            //for (int i = 0; i < script.Length; i++)
            //{
            //    tempScript[i] = (byte)Enum.Parse(typeof(OpCode), script[i]);
            //}
            //returnScript = tempScript;

            NeoSystem TheNeoSystem;
            TheNeoSystem = new NeoSystem(ProtocolSettings.Default, null, null);
            var snapshot = TheNeoSystem.GetSnapshot().CreateSnapshot();
            ApplicationEngine engine = ApplicationEngine.Run(returnScript,snapshot);
            Console.WriteLine("State:"+engine.State);
            Console.WriteLine("GasConsumed:"+engine.GasConsumed);
            Console.WriteLine("---------------");

            Console.WriteLine("Invocation Stack Count:" + engine.InvocationStack.Count);
            while (engine.InvocationStack.Count > 0)
            {
                try
                {
                    var item = engine.InvocationStack.Pop();
                    while (item.EvaluationStack.Count > 0)
                    {
                        try
                        {
                            var itemEva = item.EvaluationStack.Pop();
                            var itemEvaS = JsonSerializer.Serialize(itemEva);
                            Console.WriteLine(itemEvaS);

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            Console.WriteLine("---------------");
            Console.WriteLine("Result Stack Count" + engine.ResultStack.Count);
            while (engine.ResultStack.Count > 0)
            {
                try
                {
                    var item = engine.ResultStack.Pop();
                    var itemS = JsonSerializer.Serialize(item);
                    Console.WriteLine(itemS);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
