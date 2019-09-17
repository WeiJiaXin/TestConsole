using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Lowy.DebugConsole;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Abc<DebugConsole>(null);
//        var arg =new CommandArg(1, "zxcbtn", CommandArgType.SendButton);
//        arg.SendButtonCallBack += (s) => { DebugConsole.Print(s[0]); };
//        DebugConsole.AddCommand(
//            new Command() {
//                title = "zxc",
//                args=new List<CommandArg>()
//                {
//                    new CommandArg(0,"zxc",CommandArgType.InputField),
//                    arg
//                }
//            });
//        arg =new CommandArg(1, "qwebtn", CommandArgType.SendButton);
//        arg.SendButtonCallBack += (s) => { DebugConsole.Warning(s[0]); };
//        DebugConsole.AddCommand(
//            new Command() {
//                title = "qwe",
//                args=new List<CommandArg>()
//                {
//                    new CommandArg(0,"qwe",CommandArgType.Choice),
//                    arg
//                }
//            });
//        DebugConsole.AddCommand(new Command() { title = "asd", onClick = () => DebugConsole.Error("asd") });
    }

    public void Abc<T>([CanBeNull]T t)
    {
        print(typeof(T));
        Asd(t);
    }

    public void Asd(object o)
    {
        print(o);
    }
}
