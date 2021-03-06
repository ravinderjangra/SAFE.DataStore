﻿using System.Collections.Generic;
using System.Threading.Tasks;
using SafeApp;
using SafeApp.Utilities;

namespace SAFE.DataStore.Network
{
    internal interface INetworkDataOps
    {
        Session Session { get; }

        Task<MDataInfo> CreateEmptyMd(ulong typeTag);

        Task<List<byte>> CreateEmptyMdSerialized(ulong typeTag);

        Task<MDataInfo> CreateEmptyRandomPrivateMd(NativeHandle permissionsHandle, ulong protocol);

        Task<MDataInfo> CreateRandomPrivateMd(NativeHandle permissionsHandle, NativeHandle dataEntries, ulong protocol);

        Task<(byte[], byte[])> GenerateRandomKeyPair();

        PermissionSet GetFullPermissions();

        Task<Result<MDataInfo>> LocatePrivateMd(byte[] xor, ulong protocol, byte[] secEncKey, byte[] nonce);

        Task<Result<MDataInfo>> LocatePublicMd(byte[] xor, ulong protocol);

        Task<byte[]> StoreImmutableData(byte[] payload);
    }
}