﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entities.Entities
{
    public class ProductItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string BrandName { get; set; }
        public string ProductModel { get; set; }
        public string TypeOfFootwear { get; set; }
        public string Recipient { get; set; }
        public string ProductSize { get; set; }
        public string ProductColor { get; set; }
        public int ProductStock { get; set; }
        public double ProductPrice { get; set; }

        // Навигационное свойство для связи с заказами
        [JsonIgnore]
        public ICollection<OrderProduct> OrderProduct { get; set; }

        // Навигационное свойство для связи с изображениями
        [JsonIgnore]
        public ICollection<ImageItem> ImageItem { get; set; }

        public ProductItem()
        {
            // Инициализация коллекции
            ImageItem = new List<ImageItem>();
            OrderProduct = new List<OrderProduct>();
        }
    }

}
