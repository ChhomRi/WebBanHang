using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebBanHang.Models;

namespace WebBanHang.Areas.Customer.Models
{
    public class CartItem
    {
        public Product Product { get; set; } 
        public int Quantity { get; set; }
    }
    public class Cart
    {
        private List<CartItem> _Items;
        //contructor
        public Cart()
        {
            _Items = new List<CartItem>();
        }
        public List<CartItem> Items { get { return _Items; } }
        public void Add(Product p, int qty)
        {
            var item = _Items .FirstOrDefault(x => x.Product.Id == p.Id);
            if (item == null) 
            {
                _Items.Add(new CartItem { Product = p, Quantity = qty });
            }
            else
            {
                item.Quantity += qty;
            }
        }
        // cập nhật số lượng
        public void Update(int productId, int qty)
        {
            var item = _Items.FirstOrDefault(x => x.Product.Id == productId);
            if (item != null)//tồn tại
            {
                if (qty > 0)
                {
                    item.Quantity = qty;
                }
                else
                {
                    _Items.Remove(item);
                }
            }
        }
        //phuong thuc cập nhật số lượng
        public void Remove(int productId)
        {
            var item = _Items.FirstOrDefault(x => x.Product.Id == productId);
            if (item != null)
            {
                _Items.Remove(item);
            }
        }
        //tính tổng thành tiền
        public double Total
        {
            get
            {
                double total = _Items.Sum(x => x.Quantity * x.Product.Price);

                return total;
            }
        }
        //tính tổng số lượng sản phẩm
        public double Quantity
        {
            get
            {
                double total = _Items.Sum(x => x.Quantity);
                return total;
            }
        }
    }
}

    

