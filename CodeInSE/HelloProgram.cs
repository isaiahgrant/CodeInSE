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
        public class HelloProgram : CustomProgram, IHello
        {
            public const string TAG = "[hello]";

            protected string _customName;
            protected string _lastTimeRan;

            public HelloProgram()
            {
                // This program only runs once before terminating (does not loop).
                // See Base class (CustomProgram) for more information.
                _program.Runtime.UpdateFrequency = UpdateFrequency.None;

                // Declare how your program will handle commands (map a function to a command).
                _commandHandlers = new Dictionary<string, System.Action>();
                _commandHandlers[Command.SETUP] = Setup;
                _commandHandlers[Command.NONE] = None;
                _commandHandlers[Command.RESET] = Reset;
                _commandHandlers[Command.HELLO] = Hello;

                _program._logger.SysLog<HelloProgram>("initialized.");
            }

            public void Setup()
            {
                // Do some setup stuff.
                _program._command._command = "none";
                _program._logger.AppendToLogBody("Setup Complete");
                _program._logger.AppendToLogBody("Well done, the script is ready!");
            }

            public void None()
            {
                // Handle the default command (happens when an invalid command is passed).
                _program._logger.AppendToLogBody("Look in your PB's Custom Data for configuration options.");
                _program._logger.AppendToLogBody("Refer to the top of the script for documentation.");
            }

            public void Reset()
            {
                // Do reset stuff.
                _program._logger.AppendToLogBody("Reset Complete");
            }

            public void Hello()
            {
                // Write a message from your program.
                _program._logger.SetLogHeader("CodeInSE is easy\n================");
                _program._logger.AppendToLogBody("Hello Space!");
                // Utilize saved data.
                if (!String.IsNullOrEmpty(_lastTimeRan))
                {
                    _program._logger.AppendToLogBody($"Detected a save. Last time program was ran was: {_lastTimeRan}");
                }
                // Utilize configured data.
                _program.Me.CustomName = _customName;
                _program._logger.AppendToLogBody($"Set PB's custom name to: {_customName}");
            }

            public override void SetDefaultConfiguration()
            {
                // This will be used to populate the PB's CustomData if it is empty.
                base.SetDefaultConfiguration();
                _program._configuration.WriteItem("hello_section", "custom_name", "Hello Programmable Block",
                    "This is an example configuration variable.\n" +
                    "Configuration allows your script to behave differently without changing code!\n"
                );
            }

            public override void ReadConfiguration()
            {
                // Read from configuration.
                base.ReadConfiguration();
                _customName = _program._configuration.ReadItem("hello_section", "custom_name");
            }

            public override void LoadFields(Dictionary<string, Field> fields)
            {
                // Attempt to load any fields (class members) that may have been saved.
                base.LoadFields(fields);
                this._lastTimeRan = fields["_lastTimeRan"].GetString();
            }

            public override void SaveToFields()
            {
                // Save any fields (class members) that should be loaded later.
                base.SaveToFields();
                fields["_lastTimeRan"] = new Field(DateTime.Now.ToString("HH:mm:ss.fff"));
            }
        }
    }
}
