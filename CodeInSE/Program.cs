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
    partial class Program : MyGridProgram
    {
        /// <summary>
        /// The arguments provided to the program.
        /// </summary>
        public string _args;
        /// <summary>
        /// The update type of the program.
        /// </summary>
        public UpdateType _updateType;
        /// <summary>
        /// If set to true Commands will be decrypted before processed.
        /// WARNING: If commands cannot be decrypted they will be ignored.
        /// </summary>
        public bool _isUsingEncryption = false;

        #region Modules
        /// <summary>
        /// Modules make up the base functionality that comes with the framework.
        /// </summary>


        /// <summary>
        /// The grid module.
        /// </summary>
        public Grid _grid;
        /// <summary>
        /// The command module.
        /// </summary>
        public Command _command;
        /// <summary>
        /// The configuration module.
        /// </summary>
        public Configuration _configuration;
        /// <summary>
        /// The disk module.
        /// </summary>
        public Disk _disk;
        /// <summary>
        /// The logger module.
        /// </summary>
        public Logger _logger;
        /// <summary>
        /// The Timer module.
        /// </summary>
        public Timer _timer;
        /// <summary>
        /// The Encryption module.
        /// </summary>
        public Encryption _encryption;
        #endregion


        // Your Custom Program!
        public CustomProgram _customProgram;

        /// <summary>
        /// This variable is a self-referencing singleton.
        /// In short this allows global access to the Program instance.
        /// </summary>
        public static Program program;

        public Program()
        {
            program = this;
            try
            {
                // Initialize Modules.
                _grid = new Grid();
                _logger = new Logger(Logger.LogLevel.INFO, HelloProgram.TAG, false);
                _disk = new Disk();
                _configuration = new Configuration();
                _command = new Command();
                _timer = new Timer();
                _encryption = new Encryption();

                // Writing a custom encyption algorithm? Test it here!
                //throw new Exception($"Encyption/Decryption algorithm successful?: {_encryption.test("awesome!")}");

                // Initialize custom program(s).
                _customProgram = new HelloProgram();

                // Manage configurations.
                this.WriteDefaultConfigurations();
                Load();
            } catch (Exception ex)
            {
                Program.program.Echo($"**INIT ERROR**\n{ex}");
            }
        }

        public void Main(string args, UpdateType updateType)
        {
            try
            {
                Program.program._logger.SysLog<Program>("entering Main loop.");
                // Update program variables.
                Program.program._args = args;
                Program.program._updateType = updateType;

                // Run module updates.
                Program.program._command.Run();
                Program.program._timer.Run();
                // Read configuration.
                this.ReadConfigurations();

                if (Program.program._timer.canRun())
                {
                    Program.program._logger.SysLog<Program>("timer says custom program can run this loop.");
                    // Run custom program(s).
                    Program.program._customProgram.Run();
                }
            }
            catch(Exception ex)
            {
                Program.program._logger.AppendToLogBody($"**RUNTIME ERROR**\n{ex}", "", Logger.LogLevel.ERROR);
                Runtime.UpdateFrequency = UpdateFrequency.None;
            }
            finally
            {
                Program.program._logger.SysLog<Program>("exiting Main loop.");
                Program.program._logger.Log();
            }
        }

        /// <summary>
        /// Attempts to load any saved modules. If loading fails, they will simply default to new instances.
        /// </summary>
        public void Load()
        {
            try
            {
                Command command = Program.program._disk.ReadClass<Command>();
                if (command != null)
                    Program.program._command = command;

                CustomProgram program = Program.program._disk.ReadClass<HelloProgram>();
                if (program != null)
                    Program.program._customProgram = program;
            }
            catch (Exception ex)
            {
                Program.program.Echo($"**LOAD ERROR**\n{ex}");
            }
        }

        /// <summary>
        /// Saves modules so they can be loaded the next time the program executes.
        /// </summary>
        public void Save()
        {
            try
            {
                // Save modules to disk module.
                Program.program._disk.WriteClass(_command);
                Program.program._disk.WriteClass(_customProgram);
                // Write to disk.
                Program.program._disk.Write();
            }
            catch (Exception ex)
            {
                Program.program.Echo($"**SAVE ERROR**\n{ex}");
            }
        }

        /// <summary>
        /// Returns true if the program is self-looping. Otherwise returns false (if ran by user or radio
        /// transmission).
        /// </summary>
        /// <returns></returns>
        public bool isSelfUpdate()
        {
            return (_updateType & (UpdateType.Update1 | UpdateType.Update10 | UpdateType.Update100)) != 0;
        }

        /// <summary>
        /// Write default configurations (if no configurations exist).
        /// </summary>
        private void WriteDefaultConfigurations()
        {
            _timer.SetDefaultConfiguration();
            _encryption.SetDefaultConfiguration();
            _customProgram.SetDefaultConfiguration();

            _configuration.Write();
        }

        /// <summary>
        /// Load Configurations.
        /// </summary>
        private void ReadConfigurations()
        {
            _configuration.Read();

            _timer.ReadConfiguration();
            _encryption.ReadConfiguration();
            _customProgram.ReadConfiguration();
        }
    }
}