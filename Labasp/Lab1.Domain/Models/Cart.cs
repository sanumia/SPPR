using System.Collections.Generic;
using System.Linq;
using Lab1.Domain.Entities;

namespace Lab1.Domain.Models
{
    public class Cart
    {
        public virtual Dictionary<int, CartItem> CartItems { get; protected set; } = new();

        public IEnumerable<CartItem> Items => CartItems.Values;

        public virtual void AddToCart(Sweet sweet)
        {
            if (sweet == null)
            {
                return;
            }

            if (CartItems.TryGetValue(sweet.Id, out var cartItem))
            {
                cartItem.Count++;
            }
            else
            {
                CartItems[sweet.Id] = new CartItem
                {
                    SweetId = sweet.Id,
                    Name = sweet.Name,
                    Price = sweet.Price,
                    Image = sweet.Image,
                    Count = 1
                };
            }
        }

        public virtual void RemoveItems(int id)
        {
            CartItems.Remove(id);
        }

        public virtual void ClearAll()
        {
            CartItems.Clear();
        }

        public int Count => CartItems.Sum(item => item.Value.Count);

        public decimal TotalPrice => CartItems.Sum(item => item.Value.TotalPrice);
    }
}

