using ReCJKLine;

var recjk = new ReCJKLine.ReCJKLine();
var result = recjk.ReadLine("test>");
Console.WriteLine("result is: {0}", result);
var result2 = recjk.ReadLine("test2>");
Console.WriteLine("result2 is: {0}", result2);
