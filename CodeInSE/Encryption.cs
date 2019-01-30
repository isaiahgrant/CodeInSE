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
        public class Encryption : IConfigurable
        {
            Program _program;

            char _encryptionSeparator;
            string _salt;
            int _salty;

            public Encryption()
            {
                _program = Program.program;
                _program._logger.SysLog<Encryption>("initialized.");
            }

            public void SetDefaultConfiguration()
            {
                _program._logger.SysLog<Encryption>("setting default configuration.");
                _program._configuration.WriteItem("encryption", "salt", "DTI",
                    "A security feature that allows you to add an encryption layer to messages that helps protect receivers from unknown transmitters (and hackers).\n" +
                    "It cannot be manipulated in transit by any external forces (by script) and can be securly created by you when configuring your radio system.\n" +
                    "This salt *MUST* match the on the receiver and transmitter. Receivers will ignore transmitter commands that do not contain the same salt.\n" +
                    "This allows for many radio systems running simultaneously within radar proximities of one another.\n"
);
                _program._configuration.WriteItem("encryption", "encryption_separator", "-");
            }

            public void ReadConfiguration()
            {
                _program._logger.SysLog<Encryption>("reading configuration.");
                _encryptionSeparator = _program._configuration.ReadItem("encryption", "encryption_separator").ToCharArray()[0];
                _salt = _program._configuration.ReadItem("encryption", "salt");
                _salty = GetSalty(_salt);
            }

            /// <summary>
            /// Will encrypt the string parameter utilizing configuration.
            /// </summary>
            /// <param name="messageToEncrypt"></param>
            /// <returns>The encrypted string.</returns>
            public string encrypt(string messageToEncrypt)
            {
                _program._logger.SysLog<Encryption>("encrypting message.");
                string result = "";
                
                string split = "";
                foreach (char s in messageToEncrypt)
                {
                    result += split + (s + _salty);
                    split = "" + _encryptionSeparator;
                }
                split = "";
                string prepend = "";
                foreach (char s in _salt)
                {
                    prepend += split + (s + _salty);
                    split = "" + _encryptionSeparator;
                }
                result = prepend + _encryptionSeparator + result.Trim();

                return result;
            }

            /// <summary>
            /// Will decrypt the string parameter utilizing configuration.
            /// </summary>
            /// <param name="encryptedMessage"></param>
            /// <returns>Decrypted message or the string parameter if message could not be decrypted.</returns>
            public string decrypt(string encryptedMessage)
            {
                _program._logger.SysLog<Encryption>("decrypting message.");
                _program._logger.AppendToLogBody("decrypting message: "+encryptedMessage);
                string result = "";

                if (String.IsNullOrEmpty(encryptedMessage) || encryptedMessage.IndexOf(_encryptionSeparator) < 0)
                {
                    result = encryptedMessage;
                }
                else
                {
                    foreach (string s in encryptedMessage.Split(_encryptionSeparator))
                    {
                        if (s.All(char.IsDigit)) {
                            int chunk = int.Parse(s);
                            result += (char)(chunk - _salty);
                        }
                    }
                    if (!result.Contains(_salt))
                    {
                        result = encryptedMessage;
                    }
                    else
                    {
                        result = result.Remove(result.IndexOf(_salt), _salt.Length);
                    }
                }

                return result;
            }

            public int GetSalty(string salt)
            {
                int saltToThrow = 0;
                foreach (char s in salt)
                {
                    saltToThrow += (int)s;
                }

                return saltToThrow;
            }

            public bool test(string testMessage)
            {
                return testMessage == decrypt(encrypt(testMessage));
            }
        }
    }
}
