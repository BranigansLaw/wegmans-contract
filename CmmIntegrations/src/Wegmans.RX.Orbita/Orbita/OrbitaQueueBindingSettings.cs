using System;
using System.Collections.Generic;
using System.Text;

namespace Wegmans.RX.Orbita.Orbita
{
    public static class OrbitaQueueBindingSettings
    {
        public const string PatientsToProcessQueueName = "%" + nameof(PatientsToProcessQueueName) + "%";
        public const string StorageAccountConnection = nameof(StorageAccountConnection);
    }
}
