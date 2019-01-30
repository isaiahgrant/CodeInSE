using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        public class Configuration : IIO
        {
            private Program _program;
            private MyIni _ini;

            public Configuration()
            {
                _program = Program.program;
                _ini = new MyIni();
                _program._logger.SysLog<Configuration>("initialized.");
            }

            public void Read()
            {
                _program._logger.SysLog<Configuration>("reading configuration.");
                // Parse configuration.
                _ini = new MyIni();
                MyIniParseResult result;
                if (!_ini.TryParse(_program.Me.CustomData, out result))
                    throw new Exception(result.ToString());
            }

            public void Write()
            {
                // Initialize the configuration if it does not exist.
                if (String.IsNullOrEmpty(program.Me.CustomData))
                {
                    _program._logger.SysLog<Configuration>("writing default configuration.");
                    _program.Me.CustomData = _ini.ToString();
                }
            }

            /// <summary>
            /// Retrieve a configuration item as a string.
            /// </summary>
            /// <param name="section"></param>
            /// <param name="key"></param>
            /// <returns></returns>
            public string ReadItem(string section, string key)
            {
                return _ini.Get(section, key).ToString();

            }

            /// <summary>
            /// Utilizing the MyIni interface, this function with set a configuration item as well as it's description.
            /// </summary>
            /// <param name="section"></param>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <param name="description"></param>
            public void WriteItem(string section, string key, string value, string description = " -- Description coming soon...")
            {
                _ini.Set(section, key, value);
                _ini.SetComment(section, key, $"\n{description}\n");
            }
        }
    }
}
