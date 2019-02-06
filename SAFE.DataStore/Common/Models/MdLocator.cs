﻿using Newtonsoft.Json;

namespace SAFE.DataStore
{
    public class MdLocator
    {
#pragma warning disable SA1502 // Element should not be on a single line
        [JsonConstructor]
        MdLocator() { }
#pragma warning restore SA1502 // Element should not be on a single line

        public MdLocator(byte[] xorName, ulong typeTag, byte[] secEncKey, byte[] nonce)
        {
            XORName = xorName;
            TypeTag = typeTag;
            SecEncKey = secEncKey;
            Nonce = nonce;
        }

        /// <summary>
        /// The address of the Md this points at.
        /// </summary>
        public byte[] XORName { get; set; }

        /// <summary>
        /// Md type tag / protocol
        /// </summary>
        public ulong TypeTag { get; set; }

        /// <summary>
        /// Secret encryption key
        /// </summary>
        public byte[] SecEncKey { get; set; }

        public byte[] Nonce { get; set; }
    }
}