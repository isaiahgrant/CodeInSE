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
        public class Timer : IConfigurable
        {
            private Program _program;
            private TimeSpan _userDefinedTick;
            private TimeSpan _timeSinceLastRun;
            private int _millisecondsSinceLastTick;


            public Timer()
            {
                _program = Program.program;
                _millisecondsSinceLastTick = 0;
                _program._logger.SysLog<Timer>("initialized.");
            }

            public void SetDefaultConfiguration()
            {
                _program._logger.SysLog<Timer>("setting default configuration.");
                _program._configuration.WriteItem("time", "seconds_per_tick", "1");
                _program._configuration.WriteItem("time", "milliseconds_per_tick", "0");
            }

            public void ReadConfiguration()
            {
                _program._logger.SysLog<Timer>("reading configuration.");
                int secondsPerTick = int.Parse(
                    _program._configuration.ReadItem("time", "seconds_per_tick")
                );
                int millisecondsPerTick = int.Parse(
                    _program._configuration.ReadItem("time", "milliseconds_per_tick")
                );
                _userDefinedTick = new TimeSpan(0, 0, 0, secondsPerTick, millisecondsPerTick);
            }

            public bool canRun()
            {
                return _timeSinceLastRun >= _userDefinedTick;
            }

            public void Run()
            {
                _program._logger.SysLog<Timer>("running.");
                if (_program.isSelfUpdate())
                {
                    // We must aggregate values here because Runtime.TimeSinceLastRun only records the time between loops,
                    // not the amount of time since a Command was ran.
                    _timeSinceLastRun = _program.Runtime.TimeSinceLastRun.Add(new TimeSpan(0, 0, 0, 0, _millisecondsSinceLastTick));
                    // Update aggregate.
                    _millisecondsSinceLastTick = (this.canRun()) ? 0 : this.TimeSpanToMilliseconds(_timeSinceLastRun);
                }
                else
                {
                    _timeSinceLastRun = _userDefinedTick;
                }
            }

            public int TimeSpanToMilliseconds(TimeSpan timeSpan)
            {
                return (int)(timeSpan.Ticks / 10000);
            }
        }
    }
}
