Readline like library written in C#.

Support CJK fullwidth character.
Only use Console API.

## Usage

Here is basic usage.

```
using ReCJKLine;

var recjk = new ReCJKLine.ReCJKLine();
var result = recjk.ReadLine(">");
Console.WriteLine("result is: {0}", result);
```