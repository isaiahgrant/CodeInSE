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

        public class Disk : IIO
        {
            private Program _program;
            private MyIni _ini;

            public Disk()
            {
                _program = Program.program;
                _ini = new MyIni();
                _program._logger.SysLog<Disk>("initialized.");
            }

            public void Read()
            {
                _program._logger.SysLog<Disk>("reading storage.");
                _ini = new MyIni();
                MyIniParseResult result;
                if (!_ini.TryParse(_program.Storage, out result))
                    throw new Exception(result.ToString());
            }

            public void Write()
            {
                _program._logger.SysLog<Disk>("writing storage.");
                _program.Storage = _ini.ToString();
            }

            public string ReadItem(string section, string key)
            {
                return _ini.Get(section, key).ToString();
            }

            public void WriteItem(string section, string key, string value, string description = "")
            {
                _ini.Set(section, key, value);
            }

            /// <summary>
            /// Attempts to Deserialize a class from Storage.saved_modules.
            /// NOTE: When calling this method be sure to pass the *derived* type rather than the base type.
            ///       Ex: ParentClass parent = _disk.ReadClass<ChildClass>();
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <returns></returns>
            public T ReadClass<T>() where T : Serializable, new()
            {
                try
                {
                    Type typeOfClass = typeof(T);
                    return _program.DeSerialize<T>(this.ReadItem("saved_modules", $"{typeOfClass.Name}"));
                }
                catch (Exception ex)
                {
                    string ignoreWarning = ex.Message;
                }

                return null;
            }

            public void WriteClass(Serializable obj)
            {
                this.WriteItem("saved_modules", $"{obj.GetType()}", obj.Serialize());
            }
        }
    }
}
