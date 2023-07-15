using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Reflection;

namespace WS
{
    class AppSetting
    {
        private char Op = '=';
        private string _path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
        private string _appName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
        private string _iniName = null;
        private string _fullPathName = null;

        private  List<string> _keySet = new List<string>();
        public List<string> KeySet { get { return _keySet; } set { _keySet = value; } }
        private  List<string> _valueSet = new List<string>();
        public List<string> ValueSet { get { return _valueSet; } set { _valueSet = value; } }
        private List<string> _keyValueSet = new List<string>();
        public List<string> KeyValueSet { get { return _keyValueSet; } set { _keyValueSet = value; } }

        public string AppPath { get { return _path; } }
        public string AppName { get { return _appName; } }
        public string IniName { get { return _iniName; } }
        public string FullPathName { get { return _fullPathName; } }

        //public string Value { get; set; }

        public AppSetting() 
        { 
            _iniName = _appName + ".ini";
            _fullPathName = Path.Combine(_path, _iniName);
            loadSettingFile();
        }

        private void addEmptyKey(string localLines)
        {
            _keySet.Add("");
            _valueSet.Add("");
            _keyValueSet.Add(localLines);
        }

        //Baca File Ini
        // [v] 1. Rubah tanda : menjadi = 
        // [v] 2. Tambahkan Simbol ; sebagai komentar, dan tidak di load ke memory
        public void loadSettingFile()
        {
            StreamReader Reader;
            string localLines;
            string[] Row;
            bool fileExist = File.Exists(_fullPathName);
            if (fileExist)
            {
                _keySet.Clear();
                _valueSet.Clear();
                _keyValueSet.Clear();
                Reader = new StreamReader(_fullPathName);
                while (!Reader.EndOfStream)
                {
                    localLines = Reader.ReadLine().Trim();
                    // tanda ";" sebagai komentar, lewati gak usah di baca
                    if (localLines.Length >0)
                    {
                        if (localLines.Substring(0, 1) == ";")
                        {
                            addEmptyKey(localLines);
                        }
                        else
                        {
                            if (localLines.Contains(Op))
                            {
                                Row = localLines.Split(Op);
                                _keySet.Add(Row[0].Trim());
                                _valueSet.Add(Row[1].Trim());
                                _keyValueSet.Add(Row[0].Trim() + " " + Op + " " + Row[1].Trim());

                            }
                            else
                            {
                                addEmptyKey(localLines);
                            }
                        }
                    }
                    else
                    {
                        addEmptyKey(localLines);
                    }

                }
                Reader.Close();
            }

        }

        private void reSetKeyValue()
        {
            List<string> _Temp_keyValueSet = new List<string>(_keyValueSet);
            //_Temp_keyValueSet = _keyValueSet;

            _keyValueSet.Clear();
            for (int i = 0; i < _keySet.Count(); i++) 
            {
                if (_keySet[i] == "")
                {
                    _keyValueSet.Add(_Temp_keyValueSet[i]);
                }
                else
                { 
                _keyValueSet.Add(_keySet[i] + " " + Op + " " + _valueSet[i]);
                }
            }
        }

        private void reset()
        {
            List<string> _Temp_keyValueSet = new List<string>(_keyValueSet);
            //_Temp_keyValueSet = _keyValueSet;

            _keyValueSet.Clear();
            for (int i = 0; i < _keySet.Count(); i++)
            {
                if (_keySet[i] == "")
                {
                    _keyValueSet.Add(_Temp_keyValueSet[i]);
                }
                else
                {
                    _keyValueSet.Add(_keySet[i] + " " + Op + " " + _valueSet[i]);
                }
            }
        }

        public void writeSettingFile() 
        {
            StreamWriter Writer;
            Writer = new StreamWriter (_fullPathName);
            reSetKeyValue();
            for (int i = 0; i < KeyValueSet.Count(); i++) 
            {
                Writer.WriteLine(KeyValueSet[i]);
            }
            Writer.Close();
        }


        public string getVal(string key)
        {
            string r = "";
            for (int i = 0; i < _keySet.Count(); i++)
            {
                if (key == _keySet[i])
                {
                    r = _valueSet[i];
                    break;
                }
            }
            return r;
        }

        public void setVal(string key, string value)
        {
            bool ada = false;
            for (int i = 0; i < _keySet.Count(); i++)
            {
                if (key == _keySet[i]) { ada = true; _valueSet[i] = value; break; }
            }
            if (ada) { writeSettingFile(); }
        }

    }
}
