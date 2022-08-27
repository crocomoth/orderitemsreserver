using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace OrderItemsReserver
{
    public class OrderItem : BaseEntity
    {
        public OrderItem(int Id)
        {
            this.Id = Id;
            PartitionKey = Id.ToString();
        }

        [JsonProperty]
        public override int Id { get; protected set; }
        [JsonProperty("id")]
        public string PartitionKey { get; set; }
        public string BuyerId { get; set; }
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;
        public Address ShipToAddress { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
