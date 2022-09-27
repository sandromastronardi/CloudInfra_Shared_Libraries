using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyName.Shared.Events
{
    public static class EventTypes
    {
        public const string SystemEventIdentity = "System";
        public static class Users
        {
            public const string UserCreated = nameof(UserCreated);
            public const string UserDeleted = nameof(UserDeleted);
            public const string UserUpdated = nameof(UserUpdated);
            public const string UserDisabled = nameof(UserDisabled);
            public const string UserEnabled = nameof(UserEnabled);
            public const string UserLoggedIn = nameof(UserLoggedIn);
        }

        public static class Tenants
        {
            public const string TenantCreated = nameof(TenantCreated);
            public const string TenantDeleted = nameof(TenantDeleted);
            public const string TenantUpdated = nameof(TenantUpdated);
            public const string TenantDisabled = nameof(TenantDisabled);
            public const string TenantAbandoned = nameof(TenantAbandoned);
        }
        
        public static class Storage
        {
            // https://docs.microsoft.com/en-us/azure/storage/blobs/storage-blob-event-overview
            // filter https://docs.microsoft.com/en-us/azure/storage/blobs/storage-blob-event-overview
            public const string StorageFileCreated = nameof(StorageFileCreated);
            public const string StorageFileDeleted = nameof(StorageFileDeleted);
            public const string StorageFileUpdated = nameof(StorageFileUpdated);

            public const string StorageJobFileCreated = nameof(StorageJobFileCreated);
            public const string StorageJobFileDeleted = nameof(StorageJobFileDeleted);
            public const string StorageJobFileUpdated = nameof(StorageJobFileUpdated);
        }
        public static class Device
        {
            public const string DeviceCreated = nameof(DeviceCreated);
            public const string DeviceUpdated = nameof(DeviceUpdated);
            public const string DeviceDeleted = nameof(DeviceDeleted);
            public const string DeviceDisconnected = nameof(DeviceDisconnected);
            public const string DeviceConnected = nameof(DeviceConnected);
            public const string DeviceCommandFailed = nameof(DeviceCommandFailed);
        }

        public static class Microsoft
        {
            public static class Storage
            {
                public const string BlobCreated = "Microsoft.Storage.BlobCreated";
                public const string BlobDeleted = "Microsoft.Storage.BlobDeleted";
                // more events https://docs.microsoft.com/en-us/azure/event-grid/event-schema-blob-storage
            }
            public static class KeyVault
            {
                public const string SecretNewVersionCreated  = "Microsoft.KeyVault.SecretNewVersionCreated";
                public const string SecretNearExpiry         = "Microsoft.KeyVault.SecretNearExpiry";
                public const string SecretExpired            = "Microsoft.KeyVault.SecretExpired";
                public const string VaultAccessPolicyChanged = "Microsoft.KeyVault.VaultAccessPolicyChanged";
                // more events https://docs.microsoft.com/en-us/azure/event-grid/event-schema-key-vault

            }
            public static class Devices
            {
                public const string DeviceCreated = "Microsoft.Devices.DeviceCreated";
                public const string DeviceDeleted = "Microsoft.Devices.DeviceDeleted";
                public const string DeviceConnected = "Microsoft.Devices.DeviceConnected";
                public const string DeviceDisconnected = "Microsoft.Devices.DeviceDisconnected";
                public const string DeviceTelemetry = "Microsoft.Devices.DeviceTelemetry";
            }
        }
    }
}
