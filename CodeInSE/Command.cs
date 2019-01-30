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
        public class Command : Serializable, IRunnable
        {
            // TODO: CONVERT TO ENUM
            public const string SETUP = "setup";
            public const string NONE = "none";
            public const string RESET = "reset";
            public const string HELLO = "hello";

            public string _command;
            public string[][] _commandArguments;

            public string[] _possibleCommands;

            private Program _program;
            
            private string[] _allArgs;

            public Command()
            {
                _program = Program.program;
                _possibleCommands = new string[] {
                    SETUP,
                    NONE,
                    RESET,
                    HELLO
                };

                _command = SETUP;
                _commandArguments = new string[][] { };
                _allArgs = new string[] { };
                _program._logger.SysLog<Command>("initialized.");
            }

            public void Run()
            {
                _program._logger.SysLog<Command>("running.");
                if (!_program.isSelfUpdate())
                {
                    string args = _program._args;
                    if (_program._isUsingEncryption)
                    {
                        _program._logger.SysLog<Command>("decrypting program args.");
                        args = _program._encryption.decrypt(args);
                    }
                    
                    _allArgs = args.Split(':');
                    _command = _command = _allArgs[0].Trim().ToLower();
                    if (String.IsNullOrEmpty(_command) || Array.IndexOf(_possibleCommands, _command) < 0)
                    {
                        _command = "none";
                        _program._logger.AppendToLogBody(
                            "You must use one of the following arguments when running this script: " + String.Join(",", _possibleCommands),
                            "",
                            Logger.LogLevel.WARNING
                        );
                    }

                    if (_allArgs.Length > 1)
                    {
                        _program._logger.SysLog<Command>("parsing command args.");
                        _commandArguments = new string[_allArgs.Length - 1][];
                        for (int i = 1; i < _allArgs.Length; i++)
                        {
                            _commandArguments[i - 1] = _allArgs[i].Split(',').Select(argument => argument.Trim()).ToArray();
                        }
                    }
                }
            }

            /// <summary>  
            /// Saves fields to a fields variable that will be used to serialize Transform later 
            /// The following field types are supported:   
            ///     Byte[], int, boolean, float, double, string, vector and anything that extends from Serializable  
            /// </summary>  
            public override void SaveToFields()
            {
                _program._logger.SysLog<Command>("saving fields.");
                fields["_command"] = new Field(this._command);
                fields["_commandArguments"] = new Field(this._commandArguments);
                fields["_allArgs"] = new Field(this._allArgs);
            }

            /// <summary>  
            /// Loads fields from a dictionary of fields  
            /// NOTE:  
            ///     Make sure that you are not trying to load fields that are not saved or are a different type!! 
            ///     You will get the error key not found if you do try to do this 
            /// </summary>  
            /// <param name="fields">Dictionary to load from</param>  
            public override void LoadFields(Dictionary<String, Field> fields)
            {
                _program._logger.SysLog<Command>("loading fields.");
                this._command = fields["_command"].GetString();
                this._commandArguments = fields["_commandArguments"].GetStringArrayArray();
                this._allArgs = fields["_allArgs"].GetStringArray();
            }
        }
    }
}
