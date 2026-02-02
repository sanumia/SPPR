using System;
using System.Collections.Generic;
using Lab1.Domain.Entities;
using Lab1.Domain.Models;
using Lab1.UI.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Lab1.UI.Services.CartService
{
    public class SessionCart : Cart
    {
        private const string CartSessionKey = "cart";
        private readonly ISession _session;

        private SessionCart(ISession session)
        {
            _session = session;
            var storedItems = session.Get<Dictionary<int, CartItem>>(CartSessionKey);
            if (storedItems != null)
            {
                CartItems = storedItems;
            }
        }

        public static SessionCart GetCart(IServiceProvider services)
        {
            var contextAccessor = services.GetRequiredService<IHttpContextAccessor>();
            var session = contextAccessor.HttpContext?.Session
                ?? throw new InvalidOperationException("Session is not available.");

            return new SessionCart(session);
        }

        public override void AddToCart(Sweet sweet)
        {
            base.AddToCart(sweet);
            Commit();
        }

        public override void RemoveItems(int id)
        {
            base.RemoveItems(id);
            Commit();
        }

        public override void ClearAll()
        {
            base.ClearAll();
            Commit();
        }

        private void Commit()
        {
            _session.Set(CartSessionKey, CartItems);
        }
    }
}

