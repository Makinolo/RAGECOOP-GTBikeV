using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace RageCoop.Core.Scripting
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class CustomEventReceivedArgs : EventArgs
    {
        /// <summary>
        /// The event hash
        /// </summary>
        public int Hash { get; set; }

        /// <summary>
        /// Supported types: byte, short, ushort, int, uint, long, ulong, float, bool, string, Vector3, Quaternion, Vector2, byte[] and int[]
        /// </summary>
        public object[] Args { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class CustomEvents
    {
        private static readonly MD5 Hasher = MD5.Create();
        private static readonly Dictionary<int, string> Hashed = new Dictionary<int, string>();
        internal static readonly int OnPlayerDied = Hash("RageCoop.OnPlayerDied");
        internal static readonly int SetWeather = Hash("RageCoop.SetWeather");
        internal static readonly int OnPedDeleted = Hash("RageCoop.OnPedDeleted");
        internal static readonly int OnVehicleDeleted = Hash("RageCoop.OnVehicleDeleted");
        internal static readonly int SetAutoRespawn = Hash("RageCoop.SetAutoRespawn");
        internal static readonly int SetDisplayNameTag = Hash("RageCoop.SetDisplayNameTag");
        internal static readonly int NativeCall = Hash("RageCoop.NativeCall");
        internal static readonly int NativeResponse = Hash("RageCoop.NativeResponse");
        internal static readonly int AllResourcesSent = Hash("RageCoop.AllResourcesSent");
        internal static readonly int ServerPropSync = Hash("RageCoop.ServerPropSync");
        internal static readonly int ServerBlipSync = Hash("RageCoop.ServerBlipSync");
        internal static readonly int SetEntity = Hash("RageCoop.SetEntity");
        internal static readonly int DeleteServerProp = Hash("RageCoop.DeleteServerProp");
        internal static readonly int UpdatePedBlip = Hash("RageCoop.UpdatePedBlip");
        internal static readonly int DeleteEntity = Hash("RageCoop.DeleteEntity");
        internal static readonly int DeleteServerBlip = Hash("RageCoop.DeleteServerBlip");
        internal static readonly int CreateVehicle = Hash("RageCoop.CreateVehicle");
        internal static readonly int WeatherTimeSync = Hash("RageCoop.WeatherTimeSync");
        internal static readonly int IsHost = Hash("RageCoop.IsHost");
        /// <summary>
        /// Get a Int32 hash of a string.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">The exception is thrown when the name did not match a previously computed one and the hash was the same.</exception>
        public static int Hash(string s)
        {
            var hash = BitConverter.ToInt32(Hasher.ComputeHash(Encoding.UTF8.GetBytes(s)), 0);
            lock (Hashed)
            {
                if (Hashed.TryGetValue(hash, out string name))
                {
                    if (name != s)
                    {
                        throw new ArgumentException($"Hashed value has collision with another name:{name}, hashed value:{hash}");
                    }

                    return hash;
                }

                Hashed.Add(hash, s);
                return hash;
            }
        }
    }

    /// <summary>
    /// Decorator attribute to exclude properties in the message class from being serialized
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class NoSerialize : Attribute { }

    /// <summary>
    /// CustomEventMessage is a base abstract class that Custom event messages
    /// can inherit from to automate the serialization of their properties and
    /// fields using introspection.
    /// The properties and fields not intended to be serialized in the message
    /// can be decorated with NoSerialize.
    /// The serialization can't be tailored based on flags like for internal
    /// packets.
    /// </summary>
    public abstract class CustomEventMessage
    {
        /// <summary>
        /// Constructor that populates the event message with the contents of custom 
        /// received arguments coming from a packet
        /// </summary>
        public CustomEventMessage(CustomEventReceivedArgs cera)
        {
            if (cera.Hash == GetMessageHash())
            {
                FromCustomEventArgs(cera.Args);
            }
            else
            {
                throw new InvalidOperationException($"Trying to create a new CustomEventMessage ({this.GetType().Name}) with the wrong message hash ({cera.Hash})");
            }
        }

        /// <summary>
        /// Constructor for an empty message
        /// </summary>
        public CustomEventMessage()
        {
        }

        /// <summary>
        /// Converts the message in an object array that is ready to be sent as a 
        /// custom event
        /// </summary>
        public object[] ToCustomEventArgs()
        {
            List<object> args = new List<object>();

            var myClassType = this.GetType();
            var members = myClassType.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                            .Where(m =>
                                    (m.MemberType == MemberTypes.Property || m.MemberType == MemberTypes.Field) &&
                                    !m.GetCustomAttributes(typeof(NoSerialize), false).Any()
                                );

            foreach (var member in members)
            {
                object value = null;

                if (member.MemberType == MemberTypes.Property)
                {
                    value = ((PropertyInfo)member).GetValue(this);
                }
                else if (member.MemberType == MemberTypes.Field)
                {
                    value = ((FieldInfo)member).GetValue(this);
                }

                if (value != null)
                {
                    args.Add(value);
                }
            }

            return args.ToArray();
        }

        /// <summary>
        /// Gets the object array received in a custom event packet and populates the 
        /// message class properties and fields.
        /// </summary>
        public void FromCustomEventArgs(object[] args)
        {
            var myClassType = this.GetType();
            var members = myClassType.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                            .Where(m =>
                                    (m.MemberType == MemberTypes.Property || m.MemberType == MemberTypes.Field) &&
                                    !m.GetCustomAttributes(typeof(NoSerialize), false).Any()
                                );
            int counter = 0;
            foreach (var member in members)
            {
                object value = args[counter];
                if (member.MemberType == MemberTypes.Property)
                {
                    ((PropertyInfo)member).SetValue(this, value);
                }
                else if (member.MemberType == MemberTypes.Field)
                {
                    ((FieldInfo)member).SetValue(this, value);
                }
                counter++;
            }
        }

        /// <summary>
        /// Creates a Hash for the message based on the type name
        /// </summary>
        public int GetMessageHash()
        {
            return CustomEvents.Hash(this.GetType().FullName);
        }
    }
}
