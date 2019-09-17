using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Lowy.DebugConsole
{
    public class ForceAcceptAll : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }
    public partial class DebugConsole : MonoBehaviour
    {
        private static DebugConsole _ins;
        private static DebugConsole Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = Instantiate(Resources.Load<GameObject>("DebugConsole/DebugConsole"))
                        .GetComponent<DebugConsole>();
                    AddCommand(new Command("Help",
                        () => Print("Print Msa or Execution Command、ArgCommand")));
                }
                return _ins;
            }
        }

        #region UnityField
        [SerializeField] private string _configIP = "127.0.0.1";

        [SerializeField] private List<string> _testDiv = new List<string>()
        {
            "cbee5611-8fbc-43a7-a6c3-cdbff158e8b7",
            "1a9d5535-67c5-4f51-91ea-d4a725f39243",
        };
        [SerializeField] private DebugButton _openBtn;
        [SerializeField] private Transform _commandContent;
        [SerializeField] private CommandMenu _menu;
        [SerializeField] private DebugOutput _output;
        [SerializeField] private Transform commandTitleBtn;
        #endregion

        protected List<Command> _commands;

        private List<Command> Commands
        {
            get => _commands;
            set => _commands = value;
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Commands = new List<Command>();

            StartCoroutine(GetConfig());

            IEnumerator GetConfig()
            {
                using (UnityWebRequest www=UnityWebRequest.Get(_configIP + "/WebServer/ShootZombies.txt"))
                {
                    www.certificateHandler = new ForceAcceptAll();
                    yield return www.SendWebRequest();
                    www.certificateHandler?.Dispose();
                    if ((!string.IsNullOrEmpty(www.error))||string.IsNullOrEmpty(www.downloadHandler.text))
                    {
                        var sup = Application.RequestAdvertisingIdentifierAsync(
                            (string advertisingId, bool trackingEnabled, string error) =>
                            {
                                Debug.Log("advertisingId " + advertisingId + " " + trackingEnabled + " " + error);
                                foreach (var id in _testDiv) Debug.Log("advertisingId " + id);
                                if (null != _openBtn && _testDiv.Contains(advertisingId))
                                    _openBtn.gameObject.SetActive(true);
                            }
                        );
                        if (!sup)
                            Debug.Log("不支持广告ID，禁止打开调试");
                    }
                    Dictionary<string, object>
                        dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(www.downloadHandler.text);
                    if (null != _openBtn && null != dic)
                    {
                        _openBtn.gameObject.SetActive((bool) dic["OpenDebugConsole"]);
                    }
                }
            }
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
                if (index < Commands.Count)
                {
                    child.gameObject.SetActive(true);
                    child.id = index;
                    if (Commands[index]._args == null)
                        child.GetComponentInChildren<Text>().text = "-- " + Commands[index]._title;
                    else
                        child.GetComponentInChildren<Text>().text = "↘ " + Commands[index]._title;
                }
                else
                    child.gameObject.SetActive(false);
            }
            //因为总会有一个菜单被跳过
            i--;
            if (_commandContent.childCount-1 < Commands.Count)
            {
                for (; i < Commands.Count; i++)
                {
                    var child = Instantiate(commandTitleBtn, _commandContent).gameObject.AddComponent<CommandMsa>();
                    child.GetComponent<Button>().onClick.AddListener(() => Dispatcher(child));
                    child.id = i;
                    if (Commands[i]._args == null)
                        child.GetComponentInChildren<Text>().text = "-- " + Commands[i]._title;
                    else
                        child.GetComponentInChildren<Text>().text = "↘ " + Commands[i]._title;
                }
            }
        }

        public static void OpenWindow() => Ins._openBtn.OpenWindow();
        public static void CloseWindow() => Ins._openBtn.CloseWindow();

        public static void AddCommand(Command command)
        {
            Ins.Commands.RemoveAll(c => c._title == command._title);
            Ins.Commands.Add(command);
            Ins.InitContent();
        }

        public static void Print(string content)
        {
            Output(content);
        }

        public static void Warning(string content)
        {
            Output($"<color=#faff00>{content}</color>");
        }
        public static void Error(string content)
        {
            Output($"<color=#fa0000>{content}</color>");
        }

        private static void Output(string content)
        {
            Ins._output.Output(content);
            Ins._openBtn.TipsNewMsa();
        }

        private static void Dispatcher(CommandMsa msa)
        {
            var command = Ins.Commands[msa.id];
            if (command._args == null)
                Ins.Commands[msa.id]._onClick?.Invoke();
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
