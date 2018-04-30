# Command line parser for Windows

## Parsing

|Input|"C:\Program Files\test.exe" -m="Test message"|
|:-|:-|
|args[0]|C:\Program Files\test.exe|
|args[1]|-m=Test message|

```csharp
var cmd = "\"C:\\Program Files\\test.exe\" -m=\"Test message\"";
var cl = CommandLine.Parse(cmd);
if (cl != null) {
    var psi = new ProcessStartInfo(cl.Exe);
    psi.Arguments = CommandLine.ToString(cl.Args);
    Process.Start(psi);
}
```

## Encoding

|Output|test.exe -dir "D:\Test Dir\\\\"|
|:-|:-|
|args[0]|test.exe|
|args[1]|-dir|
|args[2]|D:\Test Dir\\ |

```csharp
var args = new string[] {
    "test.exe",
    "-dir",
    "D:\\Test Dir\\",
};
var cmd = CommandLine.ToString(args);
```
