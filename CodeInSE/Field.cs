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
        /// Representation of a field  
        /// A field can contain the following value:  
        /// Byte[], int, boolean, float, double, string, vector  
        /// </summary>  
        public class Field
        {
            private Byte[] value;

            internal Dictionary<string, Field> Children { get; set; } = new Dictionary<string, Field>();

            /// <summary>  
            /// Creates a field with value of a simple byte array  
            /// </summary>  
            /// <param name="value">Value of the field</param>  
            public Field(Byte[] value)
            {
                this.value = value;
            }

            /// <summary>  
            /// Creates a field with children from a serializable object  
            /// </summary>  
            /// <param name="sObject">Object to store</param>  
            public Field(Serializable sObject)
            {
                sObject.SaveToFields();
                Children = sObject.GetFields();
            }

            /// <summary>  
            /// Creates a field with children from a vector  
            /// </summary>  
            /// <param name="value"></param>  
            public Field(Vector3 value)
            {
                Children["x"] = new Field(value.X);
                Children["y"] = new Field(value.Y);
                Children["z"] = new Field(value.Z);
            }

            /// <summary>  
            /// Creates a field with value converted from a float  
            /// </summary>  
            /// <param name="value"></param>  
            public Field(float value)
            {
                this.value = BitConverter.GetBytes(value);
            }

            /// <summary>  
            /// Creates a field with value converted from a double  
            /// </summary>  
            /// <param name="value"></param>  
            public Field(double value)
            {
                this.value = BitConverter.GetBytes(value);
            }

            /// <summary>  
            /// Creates a field with value converted from a integer  
            /// </summary>  
            /// <param name="value"></param>  
            public Field(int value)
            {
                this.value = BitConverter.GetBytes(value);
            }

            /// <summary>  
            /// Creates a field with value converted from a boolean  
            /// </summary>  
            /// <param name="value"></param>  
            public Field(bool value)
            {
                this.value = BitConverter.GetBytes(value);
            }

            /// <summary>  
            /// Creates a field with value converted from a string  
            /// </summary>  
            /// <param name="value"></param>  
            public Field(string value)
            {
                this.value = System.Text.ASCIIEncoding.ASCII.GetBytes(value);
            }

            /// <summary>  
            /// Sets the children of a field  
            /// Used for storing for storage when type is unknown; try to use serializable objects for anything else  
            /// </summary>  
            /// <param name="children">Children fields</param>  
            public Field(Dictionary<string, Field> children)
            {
                this.Children = children;
            }

            /// <summary>
            /// Sets the children of a field from a String[].
            /// </summary>
            /// <param name="values"></param>
            public Field(string[] values) : this(values.ToDictionary(key => key, value => new Field(value)))
            {

            }

            /// <summary>
            /// Sets the children of a field from a String[][].
            /// </summary>
            /// <param name="values"></param>
            public Field(string[][] values)
            {
                this.Children = new Dictionary<string, Field>();
                foreach (string[] value in values)
                {
                    this.Children.Add($"{Array.IndexOf(values, value)}", new Field(value));
                }
            }

            /**  
            Functions that convert the byte value to a usable object  
            Make sure that the correct get function is used!!  
                */

            /// <summary>
            /// Gets the children as a string[].
            /// </summary>
            /// <returns></returns>
            public string[] GetStringArray()
            {
                return this.Children.Select(value => value.Value.GetString()).ToArray();
            }

            /// <summary>
            /// Gets the children as a string[][].
            /// </summary>
            /// <returns></returns>
            public string[][] GetStringArrayArray()
            {
                return this.Children.Select(value => value.Value.GetStringArray()).ToArray();
            }

            /// <summary>  
            /// Gets the byte value as a vector  
            /// </summary>  
            /// <returns></returns>  
            public Vector3 GetVector3()
            {
                Vector3 vector = new Vector3();
                vector.X = Children["x"].GetFloat();
                vector.Y = Children["y"].GetFloat();
                vector.Z = Children["z"].GetFloat();
                return vector;
            }
            /// <summary>  
            /// Gets the byte value as a float  
            /// </summary>  
            /// <returns></returns>  
            public float GetFloat()
            {
                return BitConverter.ToSingle(value, 0);
            }

            /// <summary>  
            /// Gets the byte value as a double  
            /// </summary>  
            /// <returns></returns>  
            public double GetDouble()
            {
                return BitConverter.ToDouble(value, 0);
            }

            /// <summary>  
            /// Gets the byte value as an integer  
            /// </summary>  
            /// <returns></returns>  
            public int GetInt()
            {
                return BitConverter.ToInt32(value, 0);
            }

            /// <summary>  
            /// Gets the byte value as an integer  
            /// </summary>  
            /// <returns>Value as bool</returns>  
            public bool GetBool()
            {
                return BitConverter.ToBoolean(value, 0);
            }

            /// <summary>  
            /// Gets the byte value as a string  
            /// </summary>  
            /// <returns>Value as string</returns>  
            public string GetString()
            {
                return System.Text.ASCIIEncoding.ASCII.GetString(value);
            }

            /// <summary>  
            /// Gets the byte value as an object of type T  
            /// </summary>  
            /// <returns>Object of type T</returns>  
            public T GetObject<T>() where T : Serializable, new()
            {
                T obj = new T();
                obj.LoadFields(Children);
                return obj;
            }

            /// <summary>  
            /// Gets the value  
            /// </summary>  
            /// <returns>Byte array</returns>  
            public Byte[] GetBytes()
            {
                return value;
            }

            /// <summary>  
            /// Coverts a dictionary of fields to a string  
            /// </summary>  
            /// <param name="fields"></param>  
            /// <returns></returns>  
            public static string DicToString(Dictionary<string, Field> fields)
            {
                string result = "";
                foreach (KeyValuePair<string, Field> child in fields)
                {
                    result += child.Key + ":" + child.Value.ToString();
                };
                return result;
            }

            /// <summary>  
            /// Converts a Field to a string representation  
            /// </summary>  
            /// <returns></returns>  
            public override string ToString()
            {
                string result = "{";
                if (value != null)
                {
                    result += BytesToString(value);
                }
                else
                {
                    result += DicToString(Children);
                }
                result += "}";
                return result;
            }

            /// <summary>  
            /// Converts a byte array to a string  
            /// </summary>  
            /// <param name="ba"> The byte array to convert</param>  
            /// <returns>String represantation of byte array</returns>  
            public static string BytesToString(byte[] ba)
            {
                string hex = BitConverter.ToString(ba);

                return hex.Replace("-", "");
            }
        }
    }
}
