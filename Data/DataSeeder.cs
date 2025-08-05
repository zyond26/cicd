using System.Collections.Generic;
using System.Linq;
using WebRestaurant.Models;

namespace WebRestaurant.Data
{
    public static class DataSeeder
    {
        public static void SeedData(AppDbContext context)
        {
            // 1. Kiểm tra xem đã có tài khoản admin hay chưa
            var adminUser = context.Users.FirstOrDefault(u => u.Email == "admin123@gmail.com");
            if (adminUser == null)
            {
                // Thực tế nên hash mật khẩu (ví dụ BCrypt)
                adminUser = new User
                {
                    Name = "Administrator",
                    Email = "admin123@gmail.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("123456"),
                    Phone = "0999999999",
                    Address = "Việt Nam",
                    Role = "admin"        // Đặt role = admin
                };
                context.Users.Add(adminUser);
                context.SaveChanges();
            }
            // Nếu chưa có dữ liệu món ăn nào thì seed sample data
            if (!context.MenuItems.Any())
            {
                var items = new List<MenuItem>
                {
                    new MenuItem { Name = "Phở bò", Description = "Phở bò truyền thống Việt Nam", Price = 50000, ImageUrl = "PhoBo.jpg", Category = "Món chính" },
                    new MenuItem { Name = "Bún chả", Description = "Bún chả Hà Nội đặc sắc", Price = 40000, ImageUrl = "BunCha.jpg", Category = "Món chính" },
                    new MenuItem { Name = "Gỏi cuốn", Description = "Gỏi cuốn tươi ngon, đầy đủ rau sống", Price = 30000, ImageUrl = "GoiCuon.jpg", Category = "Khai vị" },
                    new MenuItem { Name = "Bánh mì", Description = "Bánh mì Việt Nam với nhân đa dạng", Price = 15000, ImageUrl = "BanhMi.jpg", Category = "Đồ ăn nhanh" },
                    new MenuItem { Name = "Cà phê sữa đá", Description = "Cà phê sữa đá mát lạnh", Price = 25000, ImageUrl = "CafeSuaDa.jpg", Category = "Đồ uống" },
                    new MenuItem { Name = "Sinh tố bơ", Description = "Sinh tố bơ tươi ngon", Price = 30000, ImageUrl = "SinhToBo.jpg", Category = "Đồ uống" },
                    new MenuItem { Name = "Trà sữa", Description = "Trà sữa truyền thống với topping trân châu", Price = 35000, ImageUrl = "TraSua.jpg", Category = "Đồ uống" },
                    new MenuItem { Name = "Chè ba màu", Description = "Chè ba màu thơm ngon, mát lạnh", Price = 20000, ImageUrl = "CheBaMau.jpg", Category = "Tráng miệng" },
                    new MenuItem { Name = "Bánh flan", Description = "Bánh flan mềm mịn, béo ngậy", Price = 22000, ImageUrl = "BanhFlan.jpg", Category = "Tráng miệng" },
                    new MenuItem { Name = "Kem socola", Description = "Kem socola đậm đà, mát lạnh", Price = 18000, ImageUrl = "KemChocolate.jpg", Category = "Tráng miệng" },
                    new MenuItem { Name = "Bánh xèo", Description = "Bánh xèo giòn rụm, nhân tôm thịt", Price = 35000, ImageUrl = "BanhXeo.jpg", Category = "Món chính" },
                    new MenuItem { Name = "Hủ tiếu", Description = "Hủ tiếu Nam Vang đặc sản", Price = 40000, ImageUrl = "HuTieu.jpg", Category = "Món chính" },
                    new MenuItem { Name = "Bánh cuốn", Description = "Bánh cuốn nóng hổi, nhân thịt nấm", Price = 28000, ImageUrl = "BanhCuon.jpg", Category = "Món chính" },
                    new MenuItem { Name = "Cháo lòng", Description = "Cháo lòng đặc sản, bổ dưỡng", Price = 30000, ImageUrl = "ChaoLong.jpg", Category = "Món chính" },
                    new MenuItem { Name = "Súp lơ", Description = "Súp lơ xanh, tươi ngon", Price = 32000, ImageUrl = "SupLo.jpg", Category = "Khai vị" },
                
                    // Món chính bổ sung
                    new MenuItem { Name = "Lẩu thái", Description = "Lẩu thái chua cay, tươi ngon", Price = 60000, ImageUrl = "https://i-giadinh.vnecdn.net/2022/12/17/Thanh-pham-1-1-5372-1671269525.jpg", Category = "Món chính" },
                    new MenuItem { Name = "Bò lúc lắc", Description = "Bò lúc lắc sốt tiêu đen", Price = 55000, ImageUrl = "https://tiki.vn/blog/wp-content/uploads/2023/08/cach-lam-bo-luc-lac-sot-tieu-den-1024x683.jpg", Category = "Món chính" },
                    new MenuItem { Name = "Mì Quảng", Description = "Mì Quảng đặc sản miền Trung", Price = 45000, ImageUrl = "https://hapinut.com/wp-content/uploads/2022/03/mi-quang-quang-nam.jpg", Category = "Món chính" },
                    new MenuItem { Name = "Bún bò Huế", Description = "Bún bò Huế cay nồng đậm đà", Price = 48000, ImageUrl = "https://hoasenfoods.vn/wp-content/uploads/2024/01/bun-bo-hue.jpg", Category = "Món chính" },

                    // Đồ uống bổ sung
                    new MenuItem { Name = "Nước cam ép", Description = "Nước cam tươi ép nguyên chất", Price = 30000, ImageUrl = "https://suckhoedoisong.qltns.mediacdn.vn/324455921873985536/2022/2/18/nuoc-cam-16451723509001548204332-1645190499535-16451904996541594829836.jpg", Category = "Đồ uống" },
                    new MenuItem { Name = "Nước dừa", Description = "Nước dừa tươi mát lành", Price = 25000, ImageUrl = "https://storage-vnportal.vnpt.vn/ndh-ubnd/5893/1223/uong-nuoc-dua.jpg", Category = "Đồ uống" },
                    new MenuItem { Name = "Sữa đậu nành", Description = "Sữa đậu nành nóng hoặc lạnh", Price = 20000, ImageUrl = "https://file.hstatic.net/1000394081/article/nuoc-dau-nanh_1a2d577c17a24711952e2324850cc592.jpg", Category = "Đồ uống" },
                    new MenuItem { Name = "Cocktail trái cây", Description = "Cocktail tươi mát, đầy vitamin", Price = 50000, ImageUrl = "https://www.huongnghiepaau.com/wp-content/uploads/2018/06/cocktail-trai-cay.jpg", Category = "Đồ uống" },

                    // Tráng miệng bổ sung
                    new MenuItem { Name = "Chè khúc bạch", Description = "Chè khúc bạch thanh mát", Price = 25000, ImageUrl = "https://cdn.buffetposeidon.com/app/media/Kham-pha-am-thuc/04.2024/190424-lam-che-khuc-bach-buffet-poseidon-04.jpg", Category = "Tráng miệng" },
                    new MenuItem { Name = "Bánh su kem", Description = "Bánh su kem béo mịn, thơm ngon", Price = 20000, ImageUrl = "https://onesteak.vn/wp-content/uploads/2023/07/tong-hop-cac-tiem-banh-su-kem-ngon-ha-noi-noi-tieng-2105-5.jpg", Category = "Tráng miệng" },
                    new MenuItem { Name = "Kem dừa", Description = "Kem dừa tươi, béo ngậy", Price = 28000, ImageUrl = "https://bizweb.dktcdn.net/thumb/grande/100/424/895/products/z2433596499383-dbd25f1960c846db61e562ef6602e384.jpg?v=1620205274247", Category = "Tráng miệng" },
                    new MenuItem { Name = "Bánh mochi", Description = "Bánh mochi Nhật Bản mềm dẻo", Price = 30000, ImageUrl = "https://www.fujimarket.vn/images_upload/nhung-dieu-thu-vi-ve-banh-mochi-nhat-ban.jpg", Category = "Tráng miệng" },

                    // Khai vị bổ sung
                    new MenuItem { Name = "Súp hải sản", Description = "Súp hải sản tươi ngon", Price = 35000, ImageUrl = "https://monngonmoingay.com/wp-content/uploads/2020/03/sup-nam-hai-san-chua-cay.webp", Category = "Khai vị" },
                    new MenuItem { Name = "Chả giò", Description = "Chả giò giòn rụm, nhân thịt", Price = 30000, ImageUrl = "https://www.disneycooking.com/wp-content/uploads/2019/08/c%C3%A1ch-l%C3%A0m-ch%E1%BA%A3-gi%C3%B2-t%C3%B4m-th%E1%BB%8Bt-khoai-m%C3%B4n.jpg", Category = "Khai vị" },
                    new MenuItem { Name = "Nộm bò khô", Description = "Nộm bò khô thơm ngon, đậm vị", Price = 32000, ImageUrl = "https://media.loveitopcdn.com/25228/nom-bo-kho.jpg", Category = "Khai vị" },
                    new MenuItem { Name = "Bánh bột lọc", Description = "Bánh bột lọc Huế truyền thống", Price = 28000, ImageUrl = "https://statics.vinpearl.com/banh-bot-loc-hue-anh-thumb_1628662396.png", Category = "Khai vị" },

                    // Đồ ăn nhanh bổ sung
                    new MenuItem { Name = "Pizza hải sản", Description = "Pizza hải sản phô mai béo ngậy", Price = 90000, ImageUrl = "https://file.hstatic.net/200000700229/article/lam_pizza_hai_san_pho_mai_bang_noi_chien_khong_dau_523f886dece8480a91fef62e20dc6630.jpg", Category = "Đồ ăn nhanh" },
                    new MenuItem { Name = "Hamburger bò", Description = "Hamburger bò thơm ngon", Price = 45000, ImageUrl = "https://cdn.tgdd.vn/Files/2020/07/22/1272718/cach-lam-hamburger-bo-kieu-my-ngon-nhu-ngoai-hang-202111121636377572.jpg", Category = "Đồ ăn nhanh" },
                    new MenuItem { Name = "Khoai tây chiên", Description = "Khoai tây chiên giòn rụm", Price = 20000, ImageUrl = "https://lamsonfood.com/wp-content/uploads/2022/04/cach-lam-khoai-tay-chien-1.jpg", Category = "Đồ ăn nhanh" },
                    new MenuItem { Name = "Xúc xích nướng", Description = "Xúc xích nướng thơm ngon", Price = 30000, ImageUrl = "https://i.pinimg.com/564x/f7/95/4e/f7954e27586306e4ceb2eefefb0815e5.jpg", Category = "Đồ ăn nhanh" }




                };

                context.MenuItems.AddRange(items);
                context.SaveChanges();
            }
        }
    }
}
