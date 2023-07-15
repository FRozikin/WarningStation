using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using System.IO.Ports;
using System.Net;
using System.Net.Sockets;

//Modbus library
using Modbus.Device;
using Modbus.Data;

//Media
using System.Media;
using System.Reflection;
using System.IO;
using System.Runtime.InteropServices;

namespace WS
{

    class WarningSystem
    {
        public AppSetting Cfg = new AppSetting();
#region PROPERTIES
        //Property sesuai .ini file
        public int Port { get; set; }
        public byte UnitID { get; set; }

        public ConfigWS[] cfgWS = { new ConfigWS(), new ConfigWS(), new ConfigWS(), new ConfigWS(), new ConfigWS(), 
                                    new ConfigWS(), new ConfigWS(), new ConfigWS(), new ConfigWS(), new ConfigWS() };
        //
        public string Pin1 { get; set; }
        public string Pin2 { get; set; }
        public bool StartTCP { get; set; }
        public bool StopTCP { get; set; }
        public bool Running { get; set; }
        public bool ConfigChangedTCP { get; set; }
        public ushort SetActiveConfigTCP { get; set; }
        public bool ConfigParamChange { get; set; }
        public bool ConfigDurationChange { get; set; }
        public int CurActiveConfig { get; set; }//config terpilih saat ini
        public bool RequestUpdateUI = false;
        public int MaxDuration { get; set; }
        public int Num_Voices { get; set; }
        public int MaxModeList { get; set; }
        public int Length_Siren { get; set; }
        public int Length_Voice { get; set; }
        public int Length_Buzzer { get; set; }
        public bool Remote { get; set; }
        public ushort LastSelected { get; set; }
        private List<string> _voice = new List<string>();
        public List<string> Voice { get { return _voice; } set { _voice = value; } }

#endregion PROPERTIES

        //Init property, baca dari file .ini
        //DO address : 0x, DI address : 1x, AI address : 3x, AO address : 4x
        #region ReadConfig.ini

        private void initWS()
        {
            _voice.Clear();
            //List<string> voice = new List<string>();
            //for (int i = 0; i < 10; i++) { voice.Add("Voice_" + (i + 1).ToString()); }
            //asumsi num_voice =10, maka bikin array voice 10,

            for (int i = 0; i < Cfg.KeySet.Count(); i++)
            {
                switch (Cfg.KeySet[i])
                {
                    case "Port": Port = int.Parse(Cfg.ValueSet[i]); break;
                    case "UnitID": UnitID = byte.Parse(Cfg.ValueSet[i]); break;
                    case "Pin1": Pin1 = Cfg.ValueSet[i]; break;
                    case "Pin2": Pin2 = Cfg.ValueSet[i]; break;
                    case "Remote": Remote = bool.Parse(Cfg.ValueSet[i]); break;
                    case "LastSelected": LastSelected = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "MaxDuration": MaxDuration = int.Parse(Cfg.ValueSet[i]); break;
                    case "MaxModeList": MaxModeList = int.Parse(Cfg.ValueSet[i]); break;
                    case "Num_Voices": Num_Voices = int.Parse(Cfg.ValueSet[i]); break;
                    case "Length_Siren": Length_Siren = int.Parse(Cfg.ValueSet[i]); break;
                    case "Length_Voice": Length_Voice = int.Parse(Cfg.ValueSet[i]); break;
                    case "Length_Buzzer": Length_Buzzer = int.Parse(Cfg.ValueSet[i]); break;
                    case "Voice_01": _voice.Add(Cfg.ValueSet[i]); break;
                    case "Voice_02": _voice.Add(Cfg.ValueSet[i]); break;
                    case "Voice_03": _voice.Add(Cfg.ValueSet[i]); break;
                    case "Voice_04": _voice.Add(Cfg.ValueSet[i]); break;
                    case "Voice_05": _voice.Add(Cfg.ValueSet[i]); break;
                    case "Voice_06": _voice.Add(Cfg.ValueSet[i]); break;
                    case "Voice_07": _voice.Add(Cfg.ValueSet[i]); break;
                    case "Voice_08": _voice.Add(Cfg.ValueSet[i]); break;
                    case "Voice_09": _voice.Add(Cfg.ValueSet[i]); break;
                    case "Voice_10": _voice.Add(Cfg.ValueSet[i]); break;
                    case "Config1_Name": cfgWS[0].Name = Cfg.ValueSet[i]; break;
                    case "Config1_Checked_Sirene": cfgWS[0].Checked_Sirene = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config1_Sirene_Duration": cfgWS[0].Sirene_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config1_Sirene_Mode": cfgWS[0].Sirene_Mode = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config1_Checked_Delay1": cfgWS[0].Checked_Delay1 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config1_Delay1_Duration": cfgWS[0].Delay1_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config1_Checked_Buzzer1": cfgWS[0].Checked_Buzzer1 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config1_Buzzer1_Duration": cfgWS[0].Buzzer1_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config1_Checked_Delay2": cfgWS[0].Checked_Delay2 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config1_Delay2_Duration": cfgWS[0].Delay2_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config1_Checked_Voice": cfgWS[0].Checked_Voice = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config1_Voice_Duration": cfgWS[0].Voice_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config1_Voice_File": cfgWS[0].Voice_File = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config1_Checked_Delay3": cfgWS[0].Checked_Delay3 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config1_Delay3_Duration": cfgWS[0].Delay3_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config1_Checked_Buzzer2": cfgWS[0].Checked_Buzzer2 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config1_Buzzer2_Duration": cfgWS[0].Buzzer2_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config1_Checked_Delay4": cfgWS[0].Checked_Delay4 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config1_Delay4_Duration": cfgWS[0].Delay4_Duration = ushort.Parse(Cfg.ValueSet[i]); break;

                    case "Config2_Name": cfgWS[1].Name = Cfg.ValueSet[i]; break;
                    case "Config2_Checked_Sirene": cfgWS[1].Checked_Sirene = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config2_Sirene_Duration": cfgWS[1].Sirene_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config2_Sirene_Mode": cfgWS[1].Sirene_Mode = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config2_Checked_Delay1": cfgWS[1].Checked_Delay1 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config2_Delay1_Duration": cfgWS[1].Delay1_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config2_Checked_Buzzer1": cfgWS[1].Checked_Buzzer1 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config2_Buzzer1_Duration": cfgWS[1].Buzzer1_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config2_Checked_Delay2": cfgWS[1].Checked_Delay2 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config2_Delay2_Duration": cfgWS[1].Delay2_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config2_Checked_Voice": cfgWS[1].Checked_Voice = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config2_Voice_Duration": cfgWS[1].Voice_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config2_Voice_File": cfgWS[1].Voice_File = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config2_Checked_Delay3": cfgWS[1].Checked_Delay3 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config2_Delay3_Duration": cfgWS[1].Delay3_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config2_Checked_Buzzer2": cfgWS[1].Checked_Buzzer2 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config2_Buzzer2_Duration": cfgWS[1].Buzzer2_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config2_Checked_Delay4": cfgWS[1].Checked_Delay4 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config2_Delay4_Duration": cfgWS[1].Delay4_Duration = ushort.Parse(Cfg.ValueSet[i]); break;

                    case "Config3_Name": cfgWS[2].Name = Cfg.ValueSet[i]; break;
                    case "Config3_Checked_Sirene": cfgWS[2].Checked_Sirene = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config3_Sirene_Duration": cfgWS[2].Sirene_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config3_Sirene_Mode": cfgWS[2].Sirene_Mode = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config3_Checked_Delay1": cfgWS[2].Checked_Delay1 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config3_Delay1_Duration": cfgWS[2].Delay1_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config3_Checked_Buzzer1": cfgWS[2].Checked_Buzzer1 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config3_Buzzer1_Duration": cfgWS[2].Buzzer1_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config3_Checked_Delay2": cfgWS[2].Checked_Delay2 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config3_Delay2_Duration": cfgWS[2].Delay2_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config3_Checked_Voice": cfgWS[2].Checked_Voice = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config3_Voice_Duration": cfgWS[2].Voice_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config3_Voice_File": cfgWS[2].Voice_File = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config3_Checked_Delay3": cfgWS[2].Checked_Delay3 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config3_Delay3_Duration": cfgWS[2].Delay3_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config3_Checked_Buzzer2": cfgWS[2].Checked_Buzzer2 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config3_Buzzer2_Duration": cfgWS[2].Buzzer2_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config3_Checked_Delay4": cfgWS[2].Checked_Delay4 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config3_Delay4_Duration": cfgWS[2].Delay4_Duration = ushort.Parse(Cfg.ValueSet[i]); break;

                    case "Config4_Name": cfgWS[3].Name = Cfg.ValueSet[i]; break;
                    case "Config4_Checked_Sirene": cfgWS[3].Checked_Sirene = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config4_Sirene_Duration": cfgWS[3].Sirene_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config4_Sirene_Mode": cfgWS[3].Sirene_Mode = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config4_Checked_Delay1": cfgWS[3].Checked_Delay1 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config4_Delay1_Duration": cfgWS[3].Delay1_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config4_Checked_Buzzer1": cfgWS[3].Checked_Buzzer1 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config4_Buzzer1_Duration": cfgWS[3].Buzzer1_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config4_Checked_Delay2": cfgWS[3].Checked_Delay2 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config4_Delay2_Duration": cfgWS[3].Delay2_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config4_Checked_Voice": cfgWS[3].Checked_Voice = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config4_Voice_Duration": cfgWS[3].Voice_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config4_Voice_File": cfgWS[3].Voice_File = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config4_Checked_Delay3": cfgWS[3].Checked_Delay3 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config4_Delay3_Duration": cfgWS[3].Delay3_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config4_Checked_Buzzer2": cfgWS[3].Checked_Buzzer2 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config4_Buzzer2_Duration": cfgWS[3].Buzzer2_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config4_Checked_Delay4": cfgWS[3].Checked_Delay4 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config4_Delay4_Duration": cfgWS[3].Delay4_Duration = ushort.Parse(Cfg.ValueSet[i]); break;

                    case "Config5_Name": cfgWS[4].Name = Cfg.ValueSet[i]; break;
                    case "Config5_Checked_Sirene": cfgWS[4].Checked_Sirene = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config5_Sirene_Duration": cfgWS[4].Sirene_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config5_Sirene_Mode": cfgWS[4].Sirene_Mode = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config5_Checked_Delay1": cfgWS[4].Checked_Delay1 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config5_Delay1_Duration": cfgWS[4].Delay1_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config5_Checked_Buzzer1": cfgWS[4].Checked_Buzzer1 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config5_Buzzer1_Duration": cfgWS[4].Buzzer1_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config5_Checked_Delay2": cfgWS[4].Checked_Delay2 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config5_Delay2_Duration": cfgWS[4].Delay2_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config5_Checked_Voice": cfgWS[4].Checked_Voice = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config5_Voice_Duration": cfgWS[4].Voice_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config5_Voice_File": cfgWS[4].Voice_File = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config5_Checked_Delay3": cfgWS[4].Checked_Delay3 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config5_Delay3_Duration": cfgWS[4].Delay3_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config5_Checked_Buzzer2": cfgWS[4].Checked_Buzzer2 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config5_Buzzer2_Duration": cfgWS[4].Buzzer2_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config5_Checked_Delay4": cfgWS[4].Checked_Delay4 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config5_Delay4_Duration": cfgWS[4].Delay4_Duration = ushort.Parse(Cfg.ValueSet[i]); break;

                    case "Config6_Name": cfgWS[5].Name = Cfg.ValueSet[i]; break;
                    case "Config6_Checked_Sirene": cfgWS[5].Checked_Sirene = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config6_Sirene_Duration": cfgWS[5].Sirene_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config6_Sirene_Mode": cfgWS[5].Sirene_Mode = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config6_Checked_Delay1": cfgWS[5].Checked_Delay1 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config6_Delay1_Duration": cfgWS[5].Delay1_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config6_Checked_Buzzer1": cfgWS[5].Checked_Buzzer1 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config6_Buzzer1_Duration": cfgWS[5].Buzzer1_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config6_Checked_Delay2": cfgWS[5].Checked_Delay2 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config6_Delay2_Duration": cfgWS[5].Delay2_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config6_Checked_Voice": cfgWS[5].Checked_Voice = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config6_Voice_Duration": cfgWS[5].Voice_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config6_Voice_File": cfgWS[5].Voice_File = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config6_Checked_Delay3": cfgWS[5].Checked_Delay3 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config6_Delay3_Duration": cfgWS[5].Delay3_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config6_Checked_Buzzer2": cfgWS[5].Checked_Buzzer2 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config6_Buzzer2_Duration": cfgWS[5].Buzzer2_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config6_Checked_Delay4": cfgWS[5].Checked_Delay4 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config6_Delay4_Duration": cfgWS[5].Delay4_Duration = ushort.Parse(Cfg.ValueSet[i]); break;

                    case "Config7_Name": cfgWS[6].Name = Cfg.ValueSet[i]; break;
                    case "Config7_Checked_Sirene": cfgWS[6].Checked_Sirene = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config7_Sirene_Duration": cfgWS[6].Sirene_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config7_Sirene_Mode": cfgWS[6].Sirene_Mode = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config7_Checked_Delay1": cfgWS[6].Checked_Delay1 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config7_Delay1_Duration": cfgWS[6].Delay1_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config7_Checked_Buzzer1": cfgWS[6].Checked_Buzzer1 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config7_Buzzer1_Duration": cfgWS[6].Buzzer1_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config7_Checked_Delay2": cfgWS[6].Checked_Delay2 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config7_Delay2_Duration": cfgWS[6].Delay2_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config7_Checked_Voice": cfgWS[6].Checked_Voice = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config7_Voice_Duration": cfgWS[6].Voice_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config7_Voice_File": cfgWS[6].Voice_File = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config7_Checked_Delay3": cfgWS[6].Checked_Delay3 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config7_Delay3_Duration": cfgWS[6].Delay3_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config7_Checked_Buzzer2": cfgWS[6].Checked_Buzzer2 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config7_Buzzer2_Duration": cfgWS[6].Buzzer2_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config7_Checked_Delay4": cfgWS[6].Checked_Delay4 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config7_Delay4_Duration": cfgWS[6].Delay4_Duration = ushort.Parse(Cfg.ValueSet[i]); break;

                    case "Config8_Name": cfgWS[7].Name = Cfg.ValueSet[i]; break;
                    case "Config8_Checked_Sirene": cfgWS[7].Checked_Sirene = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config8_Sirene_Duration": cfgWS[7].Sirene_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config8_Sirene_Mode": cfgWS[7].Sirene_Mode = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config8_Checked_Delay1": cfgWS[7].Checked_Delay1 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config8_Delay1_Duration": cfgWS[7].Delay1_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config8_Checked_Buzzer1": cfgWS[7].Checked_Buzzer1 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config8_Buzzer1_Duration": cfgWS[7].Buzzer1_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config8_Checked_Delay2": cfgWS[7].Checked_Delay2 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config8_Delay2_Duration": cfgWS[7].Delay2_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config8_Checked_Voice": cfgWS[7].Checked_Voice = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config8_Voice_Duration": cfgWS[7].Voice_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config8_Voice_File": cfgWS[7].Voice_File = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config8_Checked_Delay3": cfgWS[7].Checked_Delay3 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config8_Delay3_Duration": cfgWS[7].Delay3_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config8_Checked_Buzzer2": cfgWS[7].Checked_Buzzer2 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config8_Buzzer2_Duration": cfgWS[7].Buzzer2_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config8_Checked_Delay4": cfgWS[7].Checked_Delay4 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config8_Delay4_Duration": cfgWS[7].Delay4_Duration = ushort.Parse(Cfg.ValueSet[i]); break;

                    case "Config9_Name": cfgWS[8].Name = Cfg.ValueSet[i]; break;
                    case "Config9_Checked_Sirene": cfgWS[8].Checked_Sirene = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config9_Sirene_Duration": cfgWS[8].Sirene_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config9_Sirene_Mode": cfgWS[8].Sirene_Mode = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config9_Checked_Delay1": cfgWS[8].Checked_Delay1 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config9_Delay1_Duration": cfgWS[8].Delay1_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config9_Checked_Buzzer1": cfgWS[8].Checked_Buzzer1 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config9_Buzzer1_Duration": cfgWS[8].Buzzer1_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config9_Checked_Delay2": cfgWS[8].Checked_Delay2 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config9_Delay2_Duration": cfgWS[8].Delay2_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config9_Checked_Voice": cfgWS[8].Checked_Voice = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config9_Voice_Duration": cfgWS[8].Voice_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config9_Voice_File": cfgWS[8].Voice_File = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config9_Checked_Delay3": cfgWS[8].Checked_Delay3 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config9_Delay3_Duration": cfgWS[8].Delay3_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config9_Checked_Buzzer2": cfgWS[8].Checked_Buzzer2 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config9_Buzzer2_Duration": cfgWS[8].Buzzer2_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config9_Checked_Delay4": cfgWS[8].Checked_Delay4 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config9_Delay4_Duration": cfgWS[8].Delay4_Duration = ushort.Parse(Cfg.ValueSet[i]); break;

                    case "Config10_Name": cfgWS[9].Name = Cfg.ValueSet[i]; break;
                    case "Config10_Checked_Sirene": cfgWS[9].Checked_Sirene = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config10_Sirene_Duration": cfgWS[9].Sirene_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config10_Sirene_Mode": cfgWS[9].Sirene_Mode = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config10_Checked_Delay1": cfgWS[9].Checked_Delay1 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config10_Delay1_Duration": cfgWS[9].Delay1_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config10_Checked_Buzzer1": cfgWS[9].Checked_Buzzer1 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config10_Buzzer1_Duration": cfgWS[9].Buzzer1_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config10_Checked_Delay2": cfgWS[9].Checked_Delay2 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config10_Delay2_Duration": cfgWS[9].Delay2_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config10_Checked_Voice": cfgWS[9].Checked_Voice = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config10_Voice_Duration": cfgWS[9].Voice_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config10_Voice_File": cfgWS[9].Voice_File = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config10_Checked_Delay3": cfgWS[9].Checked_Delay3 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config10_Delay3_Duration": cfgWS[9].Delay3_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config10_Checked_Buzzer2": cfgWS[9].Checked_Buzzer2 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config10_Buzzer2_Duration": cfgWS[9].Buzzer2_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                    case "Config10_Checked_Delay4": cfgWS[9].Checked_Delay4 = bool.Parse(Cfg.ValueSet[i]); break;
                    case "Config10_Delay4_Duration": cfgWS[9].Delay4_Duration = ushort.Parse(Cfg.ValueSet[i]); break;
                }
            }
        }

        #endregion ReadConfig.ini

        public WarningSystem() 
        {
            initWS();
            //initSound();
            //MBTCP_Mapping();
            MBTCP_Start();
            InitMBSerial();
            //init_WS_TCP_Status();

            initWS_TCPVal(); // Config = 1;
        }

        ~WarningSystem()
        { 
            //kosongkan modbus
            MBTCP_Stop();
            DisposeMBSerial();
            DisposeSound();
        }

//Modbus TCP Address Mapping

#region MBTCP

        TcpListener MBTCP_slaveTcpListener;
        public Modbus.Device.ModbusSlave MBTCP_slave;
        private byte MBTCP_slaveID;// = 1;
        private int MBTCP_port;// = 502;
        public IPAddress IPAddress = IPAddress.Any;
        public string MBTCP_strIP = "";
        public string MBTCP_ErrMessage = "";
        public string ErrMessage { get; set; }

        //data MBTCP
        public bool[] MBTCP_DI = new bool[20];  
        public bool[] MBTCP_DO = new bool[20];
        public UInt16[] MBTCP_AI = new UInt16[125];
        public UInt16[] MBTCP_AO = new UInt16[125];

        public bool SecuenceIsRun { get; set; }
 

        private string GetIPAddress()
        {
            string IP = string.Empty;
            IPAddress[] MBTCP_IP;
            IPHostEntry ipEntry = Dns.GetHostEntry(Dns.GetHostName());
            MBTCP_IP = ipEntry.AddressList;
            try
            {
                //IPAddress[] ips = Dns.GetHostEntry(hostname).AddressList;
                for (int i = 0; i < MBTCP_IP.Length; i++)
                {
                    if (MBTCP_IP[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        IP = MBTCP_IP[i].ToString();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MBTCP_ErrMessage = ex.Message;
                ErrMessage = ex.Message;
            }
            return IP;
        }

        public void MBTCP_Start()
        {
            ////IPHostEntry ipEntry = Dns.GetHostEntry(Dns.GetHostName());
            ////MBTCP_IP = ipEntry.AddressList;
            MBTCP_slaveID = UnitID;
            MBTCP_port = Port;
            MBTCP_strIP = GetIPAddress(); //MBTCP_IP[0].ToString();


            //aktifkan listener IPAddress
            //MBTCP_slaveTcpListener = new TcpListener(MBTCP_IP[0], MBTCP_port);
            MBTCP_slaveTcpListener = new TcpListener(IPAddress, MBTCP_port);
            MBTCP_slaveTcpListener.Start();

            MBTCP_slave = Modbus.Device.ModbusTcpSlave.CreateTcp(MBTCP_slaveID, MBTCP_slaveTcpListener);
            MBTCP_slave.ModbusSlaveRequestReceived += new EventHandler<ModbusSlaveRequestEventArgs>(MBTCP_Modbus_Request_Event);
            MBTCP_slave.DataStore = Modbus.Data.DataStoreFactory.CreateDefaultDataStore();
            MBTCP_slave.DataStore.DataStoreWrittenTo += new EventHandler<DataStoreEventArgs>(MBTCP_Modbus_DataStoreWriteTo);
            MBTCP_slave.Listen();
        }

        public void MBTCP_Stop()
        {
            MBTCP_slaveTcpListener.Stop();
            MBTCP_slaveTcpListener = null;
            //MBTCP_slave.Stop();
            MBTCP_slave.Dispose();
        }

        private void MBTCP_Modbus_Request_Event(object sender, Modbus.Device.ModbusSlaveRequestEventArgs e)
        {
            try
            {
                //request from master//disassemble packet from master
                byte fc = e.Message.FunctionCode;
                byte[] data = e.Message.MessageFrame;
                byte[] byteStartAddress = new byte[] { data[3], data[2] };
                byte[] byteNum = new byte[] { data[5], data[4] };
                Int16 StartAddress = BitConverter.ToInt16(byteStartAddress, 0);
                Int16 NumOfPoint = BitConverter.ToInt16(byteNum, 0);
            }
            catch (Exception ex)
            {
                MBTCP_ErrMessage = ex.Message;
                ErrMessage = ex.Message;
            }
            //Console.WriteLine(fc.ToString() + "," + StartAddress.ToString() + "," + NumOfPoint.ToString());
        }

        private void MBTCP_Modbus_DataStoreWriteTo(object sender, Modbus.Data.DataStoreEventArgs e)
        {
            try
            {
                //this.Text = "DataType=" + e.ModbusDataType.ToString() + "  StartAdress=" + e.StartAddress;
                int iAddress = e.StartAddress;//e.StartAddress;

                switch (e.ModbusDataType)
                {
                    case ModbusDataType.HoldingRegister:

                        for (int i = 0; i < e.Data.B.Count; i++)
                        {
                            //Set AO                 

                            //e.Data.B[i] already write to slave.DataStore.HoldingRegisters[e.StartAddress + i + 1]
                            //e.StartAddress starts from 0
                            //You can set AO value to hardware here

                            MBTCP_DoAOUpdate(iAddress, e.Data.B[i].ToString());
                            iAddress++;
                        }
                        break;

                    case ModbusDataType.Coil:
                        for (int i = 0; i < e.Data.A.Count; i++)
                        {
                            //Set DO
                            //e.Data.A[i] already write to slave.DataStore.CoilDiscretes[e.StartAddress + i + 1]
                            //e.StartAddress starts from 0
                            //You can set DO value to hardware here

                            MBTCP_DoDOUpdate(iAddress, e.Data.A[i]);
                            iAddress++;
                            if (e.Data.A.Count == 1)
                            {
                                break;
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                MBTCP_ErrMessage = ex.Message;
                ErrMessage = ex.Message;
            }
        }

        #region "Set AO"
        private delegate void MBTCP_UpdateAOStatusDelegate(int index, String message);
        private void MBTCP_DoAOUpdate(int index, String message)
        {
            try
            {
                MBTCP_AO[index] = UInt16.Parse(message);
                DetectAO(index, ushort.Parse(message));

            }
            catch (Exception ex)
            {
                MBTCP_ErrMessage = ex.Message;
                ErrMessage = ex.Message;
            }
        }
        #endregion


        #region "Set DO"
        private delegate void MBTCP_UpdateDOStatusDelegate(int index, bool value);
        private void MBTCP_DoDOUpdate(int index, bool value)
        {
            try
            {
                //MBTCP_DO[index] = value;
                DetectDO(index, value);
            }
            catch (Exception ex)
            {
                MBTCP_ErrMessage = ex.Message;
                ErrMessage = ex.Message;
            }
        }


        #endregion


#endregion


#region MBSerial
        //Modbus Serial
        ModbusSerialMaster _modbusMaster;
        SerialPort _serialPort;
        byte _slaveID=1;
        public bool[] DO = { false, false, false, false }; //MSB DO Value
        public ushort[] ChDO = { 32, 33, 34, 35 }; //MSB DO Channel Output
        //public bool[] DO_Enable = { false, false, false, false }; //MSB DO Enable Value
        public bool[] DI = { false, false, false, false, false, false, false, false }; //MSB DO Value
        public ushort[] ChDI = { 8, 9, 10, 11, 12, 13, 14, 15 }; //MSB DO Channel Output


        private void InitMBSerial()
        {
            _serialPort = new SerialPort("COM2", 115200);
            _serialPort.Open();

            _modbusMaster = ModbusSerialMaster.CreateRtu(_serialPort);
            _modbusMaster.Transport.ReadTimeout = 500;
            _modbusMaster.Transport.WriteTimeout = 500;
            _modbusMaster.Transport.Retries = 0;
        }

        private void DisposeMBSerial()
        {
            _modbusMaster.Dispose();
            _modbusMaster = null;

            _serialPort.Close();
            _serialPort.Dispose();
            _serialPort = null;
        }

        public void ReadDigitalInput(ushort Ch)
        {
            try
            {
                bool[] result = _modbusMaster.ReadInputs(_slaveID, Ch, 8);
                DI = result;
            }
            catch (Exception ex)
            {
                MBTCP_ErrMessage = ex.Message;
                ErrMessage = ex.Message;
            }
        }

        public void ReadDigitalOutput(ushort Ch)
        {
            try
            {
                bool[] result = _modbusMaster.ReadCoils(_slaveID, Ch, 1);
                //DO[Ch - 32] = result[0];
            }
            catch (Exception ex)
            {
                MBTCP_ErrMessage = ex.Message;
                ErrMessage = ex.Message;
            }
        }

        public void WriteDigitalOutput(ushort Ch, bool on)
        {
            try
            {
                _modbusMaster.WriteSingleCoil(_slaveID, Ch, on);
            }
            catch (Exception ex)
            {
                MBTCP_ErrMessage = ex.Message;
                ErrMessage = ex.Message;
            }
        }


#endregion

#region SOUND_PLAYER
        
        public SoundPlayer soundPlayer = new SoundPlayer();
        public string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
        string[] Sound = new string[10]; //{ "00001.wav", "00002.wav", "00003.wav", "00004.wav" };


        public void playSoundDependDO(int idx)
        {
            if (DO[idx])
            {
                soundPlayer.SoundLocation = appPath + @"\Media" + @"\" + Sound[idx] + ".wav";
                soundPlayer.PlayLooping();
                //soundPlayer.Play();
            }
            else
            {
                soundPlayer.Stop();
            }
        }

        public void playSound(int idx, bool Played)
        {
            if (Played)
            {
                soundPlayer.SoundLocation = appPath + @"\Media" + @"\" + Sound[idx] + ".wav";
                soundPlayer.PlayLooping();
                //soundPlayer.Play();
            }
            else
            {
                soundPlayer.Stop();
            }
        }

        private void DisposeSound()
        {
            try
            {
            soundPlayer = null;
            soundPlayer.Dispose();

            }
            catch (Exception ex)
            {
                MBTCP_ErrMessage = ex.Message;
                ErrMessage = ex.Message;
            }
        }

        public void PlaySiren()
        {
            //Play Sound Buzzer 
            soundPlayer.SoundLocation = appPath + @"\Media" + @"\" + "S00020s.wav";
            soundPlayer.PlayLooping();
        }

        public void PlayBuzzer()
        {
            //Play Sound Buzzer 
            soundPlayer.SoundLocation = appPath + @"\Media" + @"\" + "buzzer_1s.wav";
            soundPlayer.PlayLooping();
        }
        //public IsRun {get, set}

        public void Stop()
        {
            bool[] b ={false,false,false};
            DO[0] = false; //Ch1.ON
            DO[1] = false; //Ch1.ON
            DO[2] = false; //Ch1.ON
            
            WriteDigitalOutput(ChDO[0], DO[0]);
            WriteDigitalOutput(ChDO[1], DO[1]);
            WriteDigitalOutput(ChDO[2], DO[2]);
            soundPlayer.Stop();
            // set status MBTCP
            SetDataDO((int)addressDO.Status_Run, false);
            SetDataDO((int)addressDO.Status_DO1_MSB_Flashing , false);
            SetDataDO((int)addressDO.Status_DO2_MSB_Sirene , false);
            SetDataDO((int)addressDO.Status_DO3_MSB_PA , false); 

            //SetDataAO(1, 0); // seq.
            SetDataAO(2, 0); // seq.
        }


#endregion

        #region CFG
        private void initCfgWS()
        {
            string[] tag = { "Name", "Checked_Sirene", "Sirene_Duration", "Sirene_Mode", "Checked_Delay1", "Delay1_Duration", 
                             "Checked_Buzzer1","Buzzer1_Duration","Checked_Delay2","Delay2_Duration","Checked_Voice","Voice_Duration","Voice_File",
                             "Checked_Delay3","Delay3_Duration","Checked_Buzzer2","Buzzer2_Duration","Checked_Delay4","Delay4_Duration"
                           };
            string[] head = { "Config1_", "Config2_", "Config3_", "Config4_", "Config5_", "Config6_", "Config7_", "Config8_", "Config9_", "Config10_" };
            for (int i = 0; i < 10; i++)
            {
                cfgWS[i].Name = Cfg.getVal(head[i] + tag[0]);
                cfgWS[i].Checked_Sirene = bool.Parse(Cfg.getVal(head[i] + tag[1]));
                cfgWS[i].Sirene_Duration = ushort.Parse(Cfg.getVal(head[i] + tag[2]));
                cfgWS[i].Sirene_Mode = ushort.Parse(Cfg.getVal(head[i] + tag[3]));
                cfgWS[i].Checked_Delay1 = bool.Parse(Cfg.getVal(head[i] + tag[4]));
                cfgWS[i].Delay1_Duration = ushort.Parse(Cfg.getVal(head[i] + tag[5]));
                cfgWS[i].Checked_Buzzer1 = bool.Parse(Cfg.getVal(head[i] + tag[6]));
                cfgWS[i].Buzzer1_Duration = ushort.Parse(Cfg.getVal(head[i] + tag[7]));
                cfgWS[i].Checked_Delay2 = bool.Parse(Cfg.getVal(head[i] + tag[8]));
                cfgWS[i].Delay2_Duration = ushort.Parse(Cfg.getVal(head[i] + tag[9]));
                cfgWS[i].Checked_Voice = bool.Parse(Cfg.getVal(head[i] + tag[10]));
                cfgWS[i].Voice_Duration = ushort.Parse(Cfg.getVal(head[i] + tag[11]));
                cfgWS[i].Voice_File = ushort.Parse(Cfg.getVal(head[i] + tag[12]));
                cfgWS[i].Checked_Delay3 = bool.Parse(Cfg.getVal(head[i] + tag[13]));
                cfgWS[i].Delay3_Duration = ushort.Parse(Cfg.getVal(head[i] + tag[14]));
                cfgWS[i].Checked_Buzzer2 = bool.Parse(Cfg.getVal(head[i] + tag[15]));
                cfgWS[i].Buzzer2_Duration = ushort.Parse(Cfg.getVal(head[i] + tag[16]));
                cfgWS[i].Checked_Delay4 = bool.Parse(Cfg.getVal(head[i] + tag[17]));
                cfgWS[i].Delay4_Duration = ushort.Parse(Cfg.getVal(head[i] + tag[18]));
            }
        }

        //bit to ushort
        private ushort bToS(bool[] b)
        {
            ushort t = 0;
            for (int i = 0; i < b.Length; i++)
            {
                if (b[i]) { t |= (byte)(1 << i); };
            }
            return t;
        }
        //ushort to bit
        private static bool[] bToSInvert(ushort Value)
        {
            bool[] b={false,false,false,false,false,false,false,false };
            string s = Convert.ToString(Value, 2).PadLeft(8, '0');
            b[7] = s.Substring(0, 1) == "1" ? true : false;
            b[6] = s.Substring(1, 1) == "1" ? true : false;
            b[5] = s.Substring(2, 1) == "1" ? true : false;
            b[4] = s.Substring(3, 1) == "1" ? true : false;
            b[3] = s.Substring(4, 1) == "1" ? true : false;
            b[2] = s.Substring(5, 1) == "1" ? true : false;
            b[1] = s.Substring(6, 1) == "1" ? true : false;
            b[0] = s.Substring(7, 1) == "1" ? true : false;
            return b;
        }
        // Update reg 3x. Config terpilih
        public void UpdateRegCB(ushort SelectedConfigWS)
        {
            int i = SelectedConfigWS-1; //index mulai dari 0 di ws.cfgWS
            bool[] b = { cfgWS[i].Checked_Sirene, cfgWS[i].Checked_Delay1, cfgWS[i].Checked_Buzzer1, cfgWS[i].Checked_Delay2, cfgWS[i].Checked_Voice, cfgWS[i].Checked_Delay3, cfgWS[i].Checked_Buzzer2, cfgWS[i].Checked_Delay4 };
            ushort regCh = bToS(b);
            int idxReg = 10; // mulai dari register 3x0011 Enable seq, semua config
            SetDataAO(idxReg + i, regCh);
        }


        //ini update dari TCP
        public void UpdateRegPeriod(ushort SelectedConfigWS, int IndexReg, ushort Value)
        {
            if ((SelectedConfigWS > 10) || (SelectedConfigWS < 1)) return;

            int i = SelectedConfigWS - 1; //index mulai dari 0 di ws.cfgWS
            int idxReg = 10; // mulai dari register 3x0011 Enable seq, semua config
            idxReg = idxReg + (SelectedConfigWS * 10) + 1;


        }

        //Mapping Konfigurasi ke Modbus TCP
        private void initWS_TCPVal()
        {
            //index UI dari 1, disimulasi semua posisi dari 1-10 utk inisialisasi
            for (int i = 1; i < 11; i++)
            {
                SetSequent(i); //konfig UI dari idx 1, cfg ws dari 0
            }
        }

        public void SetDataDI(int IndexAddress, bool DigitalValue)
        {
            MBTCP_DI[IndexAddress] = DigitalValue;
            MBTCP_slave.DataStore.InputDiscretes[IndexAddress + 1] = DigitalValue;
        }

        public void SetDataDO(int IndexAddress, bool DigitalValue)
        {
            MBTCP_slave.DataStore.CoilDiscretes[IndexAddress + 1] = DigitalValue;
            MBTCP_DO[IndexAddress] = DigitalValue;
        }

        //public void SetDataAI(int IndexAddress, UInt16 DataAnalog)
        //{
        //    MBTCP_AI[IndexAddress] = DataAnalog;
        //    MBTCP_slave.DataStore.InputRegisters[IndexAddress + 1] = MBTCP_AI[IndexAddress]; 
        //}

        public void SetDataAO(int IndexAddress, UInt16 DataAnalog)
        {
            MBTCP_AO[IndexAddress] = DataAnalog;
            MBTCP_slave.DataStore.HoldingRegisters[IndexAddress + 1] = DataAnalog;
        }

        //ini update dari UI
        public void SetSequent(int IndexConfig)
        {
            //Ini karena Index Modbus Config mulai dari 0, sementara form mulai dari 1
            //IndexConfig = IndexConfig - 1;
            if ((IndexConfig > 10) || (IndexConfig < 1)) return;

            int baseAddr = 10;
            //ushort NilaiRegister;
            //int RegisterAddress;
            int idxReg = 10; // mulai dari register 3x0011 Enable seq, semua config
            int i = IndexConfig - 1; //index cfgWS mulai dari 0
            bool[] b = { cfgWS[i].Checked_Sirene, cfgWS[i].Checked_Delay1, cfgWS[i].Checked_Buzzer1, cfgWS[i].Checked_Delay2, cfgWS[i].Checked_Voice, cfgWS[i].Checked_Delay3, cfgWS[i].Checked_Buzzer2, cfgWS[i].Checked_Delay4 };
            ushort regCh = bToS(b);
            SetDataAO(idxReg + i, regCh);


            // Utk nilai Checkbox sudah
            // sekarang nilai index combobox; 
            idxReg = (baseAddr * IndexConfig) + idxReg; //Address awal sesuai index

                         SetDataAO(idxReg, cfgWS[i].Sirene_Duration);
            idxReg += 1; SetDataAO(idxReg, cfgWS[i].Delay1_Duration);
            idxReg += 1; SetDataAO(idxReg, cfgWS[i].Buzzer1_Duration);
            idxReg += 1; SetDataAO(idxReg, cfgWS[i].Delay2_Duration);
            idxReg += 1; SetDataAO(idxReg, cfgWS[i].Voice_Duration);
            idxReg += 1; SetDataAO(idxReg, cfgWS[i].Delay3_Duration);
            idxReg += 1; SetDataAO(idxReg, cfgWS[i].Buzzer2_Duration);
            idxReg += 1; SetDataAO(idxReg, cfgWS[i].Delay4_Duration);
            idxReg += 1; SetDataAO(idxReg, cfgWS[i].Voice_File);
            idxReg += 1; SetDataAO(idxReg, cfgWS[i].Sirene_Mode);
        }
        #endregion

        #region DetectTCP_Setting_Command
        private void DetectDO(int index, bool value)
        {
            switch (index)
            { 
                case 0://StartTCP
                    StartTCP = value;
                    MBTCP_DO[0] = value;

                       break;
                case 1:
                       StopTCP = value;
                       MBTCP_DO[1] = value;
                       break;
            }
        }

        //index di parameter ini mulai dari 0, atau 1? 
        private void DetectAO(int index, ushort message)
        {
            ushort LocalIndex = 0;
            string sIdx = string.Empty;
            int LIdx=0;
            if(index ==0)
            {
                if (message > 0 && message < 11)
                {
                    //index 0, address 40001, rubah pilihan config
                    SetActiveConfigTCP = message;
                    ConfigChangedTCP = true;
                }
            }
            //index 11..20, parameter enable config 1..10
            else if((index > 9) && (index < 20))
            {
                //extrax integer to binary
                if (message > 255) { message = 255; }
                bool[] B = bToSInvert(message);//{ false, false, false, false, false, false, false, false };
                int i = index - 10;

                //bool[] b = { cfgWS[i].Checked_Sirene, cfgWS[i].Checked_Delay1, cfgWS[i].Checked_Buzzer1, cfgWS[i].Checked_Delay2, cfgWS[i].Checked_Voice, cfgWS[i].Checked_Delay3, cfgWS[i].Checked_Buzzer2, cfgWS[i].Checked_Delay4 };
                cfgWS[i].Checked_Sirene = B[0];
                cfgWS[i].Checked_Delay1 = B[1];
                cfgWS[i].Checked_Buzzer1 = B[2];
                cfgWS[i].Checked_Delay2 = B[3];
                cfgWS[i].Checked_Voice = B[4];
                cfgWS[i].Checked_Delay3 = B[5];
                cfgWS[i].Checked_Buzzer2 = B[6];
                cfgWS[i].Checked_Delay4 = B[7];
                //SetSequent(LIdx);
                SetDataAO(index, message);

                RequestUpdateUI = CurActiveConfig == i ? true : false;
                ConfigParamChange = true;
            }

            else if((index >19) && (index < 120))
            {
                if((index > 19) && (index < 30))          { LocalIndex = 0; }
                else if((index > 29) && (index < 40))     {LocalIndex=1;}
                else if((index > 39) && (index < 50))     {LocalIndex=2;}
                else if((index > 49) && (index < 60))     {LocalIndex=3;}
                else if((index > 59) && (index < 70))     {LocalIndex=4;}
                else if((index > 69) && (index < 80))     {LocalIndex=5;}
                else if((index > 79) && (index < 90))     {LocalIndex=6;}
                else if((index > 89) && (index < 100))    {LocalIndex=7;}
                else if((index > 99) && (index < 110))   {LocalIndex=8;}
                else if ((index > 109) && (index < 120))  {LocalIndex=9;}
                
                sIdx = index.ToString().Substring(index.ToString().Length-1,1);
                LIdx=int.Parse(sIdx);
                switch (LIdx)
                {
                    case 0:
                        if (message > MaxDuration) { message = (ushort)MaxDuration; }  
                        cfgWS[LocalIndex].Sirene_Duration = message;

                        break;
                    case 1:
                        if (message > MaxDuration) { message = (ushort)MaxDuration; }
                        cfgWS[LocalIndex].Delay1_Duration = message;
                        break;
                    case 2:
                        if (message > MaxDuration) { message = (ushort)MaxDuration; }
                        cfgWS[LocalIndex].Buzzer1_Duration = message;
                        break;
                    case 3:
                        if (message > MaxDuration) { message = (ushort)MaxDuration; }
                        cfgWS[LocalIndex].Delay2_Duration = message;
                        break;
                    case 4:
                        cfgWS[LocalIndex].Voice_Duration = message;
                        break;
                    case 5:
                        if (message > MaxDuration) { message = (ushort)MaxDuration; }
                        cfgWS[LocalIndex].Delay3_Duration = message;
                        break;
                    case 6:
                        if (message > MaxDuration) { message = (ushort)MaxDuration; }
                        cfgWS[LocalIndex].Buzzer2_Duration = message;
                        break;
                    case 7:
                        if (message > MaxDuration) { message = (ushort)MaxDuration; }
                        cfgWS[LocalIndex].Delay4_Duration = message;
                        break;
                    case 8:
                        if (message > Num_Voices) { message = (ushort)Num_Voices; }
                        cfgWS[LocalIndex].Voice_File = message;
                        break;
                    case 9:
                        if (message > MaxModeList) { message = (ushort)MaxModeList; }
                        cfgWS[LocalIndex].Sirene_Mode = message;
                        break;
                }

                SetDataAO(index, message);
                RequestUpdateUI = CurActiveConfig == LIdx ? true : false;
                //SetSequent(LIdx);
                ConfigDurationChange = true;

                //SetSequent(CurActiveConfig);
                //UpdateRegPeriod(CurActiveConfig);
            }

        }
        #endregion
    }
    #region TEST



    public static class SoundInfo
    {
        [DllImport("winmm.dll")]
        private static extern uint mciSendString(
            string command,
            StringBuilder returnValue,
            int returnLength,
            IntPtr winHandle);

        public static int GetSoundLength(string fileName)
        {
            StringBuilder lengthBuf = new StringBuilder(32);

            mciSendString(string.Format("open \"{0}\" type waveaudio alias wave", fileName), null, 0, IntPtr.Zero);
            mciSendString("status wave length", lengthBuf, lengthBuf.Capacity, IntPtr.Zero);
            mciSendString("close wave", null, 0, IntPtr.Zero);

            int length = 0;
            //int.TryParse(lengthBuf.ToString(), out length);

            if (!MyTryParse(lengthBuf.ToString(), out length))
            {
                //MessageBox.Show("That was not an int! Consider this a lint-like hint!");
            }

            return length;
        }

        public static bool MyTryParse(object parameter, out int value)
        {
            value = 0;
            try
            {
                value = Convert.ToInt32(parameter);
                return true;
            }
            catch (Exception ex)
            {
                //MBTCP_ErrMessage = ex.Message;
                //ErrMessage = ex.Message;
                return false;
            }
        }

    }

    #endregion


    class ConfigWS
    {
        public string Name { get; set; }
        public bool Checked_Sirene { get; set; }
        public ushort Sirene_Duration { get; set; } //1
        public ushort Sirene_Mode { get; set; }     //10
        public bool Checked_Delay1 { get; set; } 
        public ushort Delay1_Duration { get; set; } //2
        public bool Checked_Buzzer1 { get; set; }
        public ushort Buzzer1_Duration { get; set; } //3
        public bool Checked_Delay2 { get; set; }
        public ushort Delay2_Duration { get; set; }  //4
        public bool Checked_Voice { get; set; }
        public ushort Voice_Duration { get; set; }  //5
        public ushort Voice_File { get; set; }      //9
        public bool Checked_Delay3 { get; set; }
        public ushort Delay3_Duration { get; set; }  //6
        public bool Checked_Buzzer2 { get; set; }
        public ushort Buzzer2_Duration { get; set; } //7
        public bool Checked_Delay4 { get; set; }
        public ushort Delay4_Duration { get; set; } //8
    }
}
