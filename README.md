# Command line text parser/encoder for Windows process

- Parse the command line text. (Windows process specs)
- Encode symbolic characters. (" and \\)

## Parse

|Input|string|"C:\Program Files\test.exe" -m="Test message"|
|:-|:-|:-|
|Output|args[0]|C:\Program Files\test.exe|
|Output|args[1]|-m=Test message|

```csharp
var cmd = "\"C:\\Program Files\\test.exe\" -m=\"Test message\"";
var cl = CommandLine.Parse(cmd);
if (cl != null) { // missing end of double quotation, if null returned.
    var psi = new ProcessStartInfo(cl.Exe);
    psi.Arguments = CommandLine.ToString(cl.Args);
    Process.Start(psi);
}
```

## Encode

|Output|string|test.exe -dir "D:\Test Dir\\\\"|
|:-|:-|:-|
|Input|args[0]|test.exe|
|Input|args[1]|-dir|
|Input|args[2]|D:\Test Dir\\ |

```csharp
var args = new string[] {
    "test.exe",
    "-dir",
    "D:\\Test Dir\\",
};
var cmd = CommandLine.ToString(args);
```
