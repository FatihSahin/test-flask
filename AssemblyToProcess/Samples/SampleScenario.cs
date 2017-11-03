/********************************************************************
 *                                                                  *
 *  This file contains some dummy shopping cart                     *
 *  business logic and entities                                     *
 *  to use in TestFlask.Aspects.Tests                               *
 *                                                                  *
 * *****************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestFlask.Aspects;
using TestFlask.Aspects.Identifiers;

namespace AssemblyToProcess.Samples
{
    #region Models

    public enum ErrorCodes
    {
        CustomerNotFound,
        ProductNotFound,
        StockNotAvailable,
        CustomerNotAuthenticated,
    }

    public class Customer
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public bool IsAuthenticated { get; set; }
    }

    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }

    public class ShoppingCart
    {
        public Guid CartId { get; set; }
        public Customer CartOwner { get; set; }

        private List<Product> items = new List<Product>();

        public List<Product> Items
        {
            get
            {
                return items;
            }
            set
            {
                items = value;
            }
        }
    }

    #endregion

    #region Biz

    public class CustomerBiz
    {
        private static List<Customer> customers = new List<Customer>
        {
             new Customer { CustomerId = 1, Name = "Fatih", IsAuthenticated = true },
             new Customer { CustomerId = 2, Name = "Ipek", IsAuthenticated = true },
             new Customer { CustomerId = 3, Name = "Uzay", IsAuthenticated = true },
             new Customer { CustomerId = 4, Name = "Caner", IsAuthenticated = false },
             new Customer { CustomerId = 5, Name = "Emos", IsAuthenticated = true },
        };

        [Playback(typeof(CustomerIdIdentifier), typeof(CustomerResponseIdentifier))]
        public Customer GetCustomer(int customerId)
        {
            return customers.SingleOrDefault(c => c.CustomerId == customerId);
        }

        public bool LogIn(int customerId)
        {
            return customers.SingleOrDefault(c => c.CustomerId == customerId).IsAuthenticated = true;
        }

        public bool LogOut(int customerId)
        {
            return !(customers.SingleOrDefault(c => c.CustomerId == customerId).IsAuthenticated = false);
        }
    }

    public class ProductBiz
    {
        private static List<Product> products = new List<Product>
        {
           new Product { ProductId = 1, Brand = "Apple", Name = "MacBook", Price = 1099, Stock = 10 },
           new Product { ProductId = 2, Brand = "Apple", Name = "iPhone", Price = 499, Stock = 5 },
           new Product { ProductId = 3, Brand = "Microsoft", Name = "XBox One", Price = 599, Stock = 10 },
           new Product { ProductId = 4, Brand = "Microsoft", Name = "Surface Book", Price = 1199, Stock = 0 },
           new Product { ProductId = 5, Brand = "Google", Name = "Pixel", Price = 399, Stock = 10 },
        };

        [Playback(typeof(ProductIdIdentifier))]
        public Product GetProduct(int productId)
        {
            return products.SingleOrDefault(p => p.ProductId == productId);
        }

        [Playback]
        public void DecrementStock(Product product)
        {
            if (product.Stock > 0)
            {
                product.Stock--;
            }
            else
            {
                throw new ApplicationException(ErrorCodes.StockNotAvailable.ToString());
            }
        }
    }

    public class ShoppingCartBiz
    {
        private CustomerBiz customerBiz;
        private ProductBiz productBiz;

        public ShoppingCartBiz()
        {
            customerBiz = new CustomerBiz();
            productBiz = new ProductBiz();
        }

        [Playback(typeof(AddToCartIdentifier))]
        public decimal AddToCart(ShoppingCart cart, int productId)
        {
            var product = productBiz.GetProduct(productId);

            if (product == null)
            {
                throw new ApplicationException(ErrorCodes.ProductNotFound.ToString());
            }

            productBiz.DecrementStock(product);

            cart.Items.Add(product);

            return 0;
        }

        [Playback(typeof(CustomerIdIdentifier))]
        public ShoppingCart CreateCart(int customerId)
        {
            var customer = customerBiz.GetCustomer(customerId);

            if (customer == null)
            {
                throw new ApplicationException(ErrorCodes.CustomerNotFound.ToString());
            }

            if (!customer.IsAuthenticated)
            {
                throw new ApplicationException(ErrorCodes.CustomerNotAuthenticated.ToString());
            }

            return new ShoppingCart
            {
                CartId = Guid.NewGuid(),
                CartOwner = customer,
                Items = new List<Product>()
            };
        }
    }

    #endregion

    #region TestFlask Identifiers

    public class CustomerIdIdentifier : IRequestIdentifier<int>
    {
        public string ResolveDisplayInfo(int req)
        {
            return $"CustomerID: {req}";
        }

        public string ResolveIdentifierKey(int req)
        {
            return req.ToString();
        }
    }

    public class ProductIdIdentifier : IRequestIdentifier<int>
    {
        public string ResolveDisplayInfo(int req)
        {
            return $"ProductID: {req}";
        }

        public string ResolveIdentifierKey(int req)
        {
            return req.ToString();
        }
    }

    public class AddToCartIdentifier : IRequestIdentifier<ShoppingCart, int>
    {
        public string ResolveDisplayInfo(ShoppingCart arg0, int arg1)
        {
            return $"AddToCart for CartId: {arg0.CartId}, ProductId: {arg1}";
        }

        public string ResolveIdentifierKey(ShoppingCart arg0, int arg1)
        {
            return $"{arg0}!{arg1}";
        }
    }

    public class CustomerResponseIdentifier : IResponseIdentifier<Customer>
    {
        public string ResolveDisplayInfo(Customer res)
        {
            return $"Customer: {res.Name}";
        }
    }

    #endregion
}
