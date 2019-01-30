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
        /**   
            README:  
            Any object that extends from Serializable can be converted to a string and back  
            using obj.Serialize and Serializable.DeSerialize<Type>().  
            This allows you to store objects in the storage variable or send objects to other grids.  
            The following field types are supported:    
                Byte[], int, boolean, float, double, string, vector and anything that extends from Serializable  
  
            The first part of this is script is an example of how you can share data between two grids using this method.  
            However this script can do much more than that.  
  
            You are free to use this in your scripts and upload them to the workshop   
            but please give credit and link to the original workshop page in the description  
            If you have any problems leave them in the comments of the workshop page  
            Author: Tentacola  
        */

        /**  
            STOP EDITING HERE! THE REST IS USED TO PARSE, SERIALIZE AND DESERIALIZE OBJECTS  
            Read the comments to get a better understanding or edit if you know what you are doing ;)  
            You are free to use this in your scripts and upload them to the workshop   
            but please give credit and link to the original workshop page in the description  
            If you have any problems leave them in the comments of the workshop page  
            Author: Tentacola

            Link: https://steamcommunity.com/sharedfiles/filedetails/?id=1212934715&insideModal=0
        */

        /// <summary>  
        /// Abstract class that allows classes that extend from it to serialize and deserialize their fields  
        /// Usefull for storing data or sending data to other grids  
        /// </summary>  
        public abstract class Serializable
        {
            protected Dictionary<String, Field> fields = new Dictionary<String, Field>();
            /// <summary>  
            /// Stores all fields that need to be serialized in the fields dictionary  
            /// </summary>  
            public abstract void SaveToFields();

            /// <summary>  
            /// Applies all fields stored in a dictionary  
            /// Remember fields that are not saved can not be loaded!!!  
            /// </summary>  
            /// <param name="fields">Dictionary to take load fields from</param>  
            public abstract void LoadFields(Dictionary<String, Field> fields);

            /// <summary>  
            /// Serializes this object and its fields to a string  
            /// </summary>  
            /// <returns></returns>  
            public string Serialize()
            {
                SaveToFields();
                return Field.DicToString(fields);
            }

            /// <summary>  
            /// Gets the protected fields variable after the SaveToFields function is called  
            /// </summary>  
            /// <returns>Dictionary of fields</returns>  
            public Dictionary<String, Field> GetFields()
            {
                SaveToFields();
                return fields;
            }
        }

        /// <summary>  
        /// Deserializes a class T from a string  
        /// </summary>  
        /// <param name="obj">The string to deserialize</param>  
        /// <returns>Object of type T with deserialized fields</returns>  
        public T DeSerialize<T>(string obj) where T : Serializable, new()
        {
            T result = new T();
            Dictionary<String, Field> fieldsFromString = StringToFields(obj);
            result.LoadFields(fieldsFromString);
            return result;
        }

        /// <summary>  
        /// Parses multiple fields to a dictionary of fields  
        /// </summary>  
        /// <param name="fields">The string of fields</param>  
        /// <returns>The resulting dictionary</returns>  
        private static Dictionary<String, Field> StringToFields(string fields)
        {
            Dictionary<String, Field> result = new Dictionary<String, Field>();
            string fieldName = "";
            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i] == '}')
                {
                    return result;
                }
                else if (fields[i] == ':' && fields[i + 1] == '{')
                {
                    string subField = fields.Substring(i + 2);
                    int subEnd = ClosingBracket(subField);
                    subField = subField.Substring(0, subEnd);
                    result.Add(fieldName, ParseField(subField));
                    i += 2 + subEnd;
                    fieldName = "";
                }
                else
                {
                    fieldName += fields[i];
                }
            }
            return result;
        }

        /// <summary>  
        /// Finds the matching closing bracket  
        /// </summary>  
        /// <param name="s"></param>  
        /// <returns></returns>  
        public static int ClosingBracket(string s)
        {
            int nextOpening = s.IndexOf('{');
            int nextClosing = s.IndexOf('}');
            while (nextOpening != -1 && nextOpening < nextClosing)
            {
                nextOpening = s.IndexOf('{', nextOpening + 1);
                nextClosing = s.IndexOf('}', nextClosing + 1);
            }
            return nextClosing;
        }

        /// <summary>  
        /// Converts a string to a byte array  
        /// </summary>  
        /// <param name="byteString"> The string to convert</param>  
        /// <returns>Byte array from string</returns>  
        private static Byte[] StringToBytes(string byteString)
        {
            int NumberChars = byteString.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(byteString.Substring(i, 2), 16);
            }
            return bytes;
        }

        /// <summary>  
        /// Parses a string to a field  
        /// </summary>  
        /// <param name="field">The string to parse</param>  
        /// <returns>The resulting field from the string</returns>  
        public static Field ParseField(string field)
        {
            string value = "";
            if (field.IndexOf(':') != -1)
            {
                return new Field(StringToFields(field));
            }
            for (int i = 0; i < field.Length; i++)
            {
                value += field[i];
            }
            return new Field(StringToBytes(value));
        }
    }
}
