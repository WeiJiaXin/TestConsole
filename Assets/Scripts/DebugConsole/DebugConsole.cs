using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lowy.DebugConsole
{
    public partial class DebugConsole : MonoBehaviour
    {
        private static DebugConsole _ins;
        private static DebugConsole Ins
        {
            get
            {
                if (_ins == null)
                    _ins = Instantiate(Resources.Load<GameObject>("DebugConsole/DebugConsole")).GetComponent<DebugConsole>();
                return _ins;
            }
        }

        #region UnityField
        [SerializeField] private Transform _commandContent;
        [SerializeField] private CommandMenu _menu;
        [SerializeField] private DebugOutput _output;
        [SerializeField] private Transform commandTitleBtn;
        #endregion

        private ushort _nextID;
        private Dictionary<int, Command> _commands;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            _commands = new Dictionary<int, Command>();
        }

        private void InitContent()
        {
            ushort i = 0;
            ushort index = 0;
            for (; i < _commandContent.childCount; i++,index++)
            {
                var child = _commandContent.GetChild(i).GetComponent<CommandMsa>();
                if (child == null)
                {
                    _commandContent.GetChild(i).gameObject.SetActive(false);
                    index--;
                    continue;
                }
                if (index < _commands.Count)
                {
                    child.gameObject.SetActive(true);
                    child.id = index;
                    if (_commands[index].args == null)
                        child.GetComponentInChildren<Text>().text = "-- " + _commands[index].title;
                    else
                        child.GetComponentInChildren<Text>().text = "↘ " + _commands[index].title;
                }
                else
                    child.gameObject.SetActive(false);
            }
            //因为总会有一个菜单被跳过
            i--;
            if (_commandContent.childCount-1 < _commands.Count)
            {
                for (; i < _commands.Count; i++)
                {
                    var child = Instantiate(commandTitleBtn, _commandContent).gameObject.AddComponent<CommandMsa>();
                    child.GetComponent<Button>().onClick.AddListener(() => Dispatcher(child));
                    child.id = i;
                    if (_commands[i].args == null)
                        child.GetComponentInChildren<Text>().text = "-- " + _commands[i].title;
                    else
                        child.GetComponentInChildren<Text>().text = "↘ " + _commands[i].title;
                }
            }
        }
        public static void AddCommand(Command command)
        {
            if(Ins._commands.Count==0)
            {
                Ins._commands.Add(Ins._nextID, new Command() {title = "Help", onClick = () => Print("help")});
                Ins._nextID++;
            }
            Ins._commands.Add(Ins._nextID,command);
            Ins._nextID++;
            Ins.InitContent();
        }

        public static void Print(string content)
        {
            Ins._output.Output(content);
        }

        public static void Warning(string content)
        {
            Ins._output.Output($"<color=#faff00>{content}</color>");
        }
        public static void Error(string content)
        {
            Ins._output.Output($"<color=#fa0000>{content}</color>");
        }

        private static void Dispatcher(CommandMsa msa)
        {
            var command = Ins._commands[msa.id];
            if (command.args == null)
                Ins._commands[msa.id].onClick?.Invoke();
            else
            {
                Ins._menu.Init(command);
                if (Ins._menu.transform.GetSiblingIndex() < msa.transform.GetSiblingIndex())
                    Ins._menu.transform.SetSiblingIndex(msa.transform.GetSiblingIndex());
                else
                    Ins._menu.transform.SetSiblingIndex(msa.transform.GetSiblingIndex() + 1);
                Ins._menu.gameObject.SetActive(true);
            }
        }
    }

}
