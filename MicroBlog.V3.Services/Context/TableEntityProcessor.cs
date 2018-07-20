using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroBlog.V3.Services.Context
{
    class TableEntityProcessor<T> : ITableEntity where T : class
    {
        public string PartitionKey { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string RowKey { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public DateTimeOffset Timestamp { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string ETag { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        private T objectToSend { get; set; }

        internal TableEntityProcessor(T objectBeingSent)
        {
            this.objectToSend = objectBeingSent;
        }
        public void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            // Get all da props
            var props = objectToSend.GetType().GetProperties();
            
            props.ToList().ForEach(prop =>
            {
                
            });
            throw new NotImplementedException();
        }

        public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
         
            Dictionary<string, EntityProperty> EBids = new Dictionary<string, EntityProperty>();
            var props = objectToSend.GetType().GetProperties();
            props.ToList().ForEach(prop =>
            {/*EBids.Add(prop.Name, new EntityProperty()*/

                
            });
            throw new NotImplementedException();
        }
    }
}
