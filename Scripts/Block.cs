using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Dotnetblockchainimplementation
{
    public interface IBlock
    {
        DateTime TimeStamp { get; }
        byte[] Data { get; set; }
        byte[] Hash { get; set; }
        byte[] PreviousHash { get; set; }
        int Nonce { get; set; }
    }

    public class Block : IBlock
    {
        public DateTime TimeStamp { get; }
        public byte[] Data { get; set; }
        public byte[] Hash { get; set; }
        public byte[] PreviousHash { get; set; }
        public int Nonce { get; set; }

        public Block(byte[] data)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
            Nonce = 0;
            PreviousHash = new byte[] { 0x00 };
            TimeStamp = DateTime.Now;
        }

        public override string ToString()
        {
            return $"{BitConverter.ToString(Hash).Replace("-", "")}:\n{System.BitConverter.ToString(PreviousHash).Replace("-", "")}\n{Nonce} {TimeStamp}";
        }

    }
}