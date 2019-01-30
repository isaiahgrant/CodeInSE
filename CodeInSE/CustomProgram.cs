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
        public class CustomProgram : Serializable, IRunnable, IConfigurable
        {
            protected Program _program;

            protected Dictionary<string, System.Action> _commandHandlers;
            protected string[][] _programArguments;

            public CustomProgram()
            {
                this._program = Program.program;
                // For your custom program to run itself (like a Timer block) simply set this value to something like: UpdateFrequency.Update100.
                // That value will force the script run every 100 frames (game cycles), which may not be consistent and is not bound by time.
                // In some cases you may need more precise timing, for this see the Timer class (and still update this value)!
                // *Exercise caution* when making the UpdateFrequency/Timer too frequent as you will sacrifice game performance.
                // For performance reasons, by default the Timer limits runs to once a second but can be modified.
                //_program.Runtime.UpdateFrequency = UpdateFrequency.None;
                _program._logger.SysLog<CustomProgram>("initialized.");
            }

            public virtual void Run()
            {
                // Program logic goes here. Depending on your UpdateFrequency this may run once per command or may loop.
                _program._logger.SysLog<CustomProgram>("running.");
                string command = _program._command._command;
                // Store command arguments.
                _programArguments = _program._command._commandArguments;
                // Attempt to handle the provided command.
                if (_commandHandlers != null && _commandHandlers.ContainsKey(command))
                {
                    _commandHandlers[command]();
                }
                else
                {
                    _program._logger.AppendToLogBody($"Command: {command} could not be processed.", "", Logger.LogLevel.WARNING);
                }
            }

            public override void LoadFields(Dictionary<string, Field> fields)
            {
                // Load any fields (class members) that the CustomProgram utilizes.
                _program._logger.SysLog<CustomProgram>("loading fields.");
            }

            public override void SaveToFields()
            {
                // Save any fields (class members) that the CustomProgram utilizes.
                _program._logger.SysLog<CustomProgram>("saving fields.");
            }

            public virtual void SetDefaultConfiguration()
            {
                _program._logger.SysLog<CustomProgram>("setting default configuration.");
            }

            public virtual void ReadConfiguration()
            {
                _program._logger.SysLog<CustomProgram>("reading configuration.");
            }
        }

    }
}
