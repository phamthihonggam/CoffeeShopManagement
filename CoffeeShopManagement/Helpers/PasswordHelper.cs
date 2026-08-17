using BCrypt.Net;
using CoffeeShopManagement.Models;
using Microsoft.AspNetCore.Identity;

namespace CoffeeShopManagement.Helpers
{
    public static class PasswordHelper
    {
        // =====================================================
        // HASH MẬT KHẨU KHÁCH HÀNG
        // =====================================================

        public static string Hash(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException(
                    "Mật khẩu không được để trống.",
                    nameof(password)
                );
            }

            return BCrypt.Net.BCrypt.HashPassword(password);
        }


        // =====================================================
        // VERIFY CHO KHÁCH HÀNG - BCrypt
        // =====================================================

        public static bool Verify(
            string password,
            string hash)
        {
            if (string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(hash))
            {
                return false;
            }

            try
            {
                // BCrypt của khách hàng
                if (IsBcryptHash(hash))
                {
                    return BCrypt.Net.BCrypt.Verify(
                        password,
                        hash
                    );
                }

                // ASP.NET Identity hash của Admin / Nhân viên
                if (IsIdentityHash(hash))
                {
                    var passwordHasher =
                        new PasswordHasher<TaiKhoan>();

                    var dummyAccount =
                        new TaiKhoan
                        {
                            TenDangNhap = "",
                            MatKhau = "",
                            HoTen = "",
                            MaVaiTro = 0
                        };

                    var result =
                        passwordHasher.VerifyHashedPassword(
                            dummyAccount,
                            hash,
                            password
                        );

                    return result !=
                        PasswordVerificationResult.Failed;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }


        // =====================================================
        // VERIFY RIÊNG CHO TÀI KHOẢN ADMIN / NHÂN VIÊN
        // =====================================================

        public static bool VerifyAccount(
            TaiKhoan account,
            string password)
        {
            if (account == null ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(account.MatKhau))
            {
                return false;
            }

            try
            {
                // Hash ASP.NET Identity
                if (IsIdentityHash(account.MatKhau))
                {
                    var passwordHasher =
                        new PasswordHasher<TaiKhoan>();

                    var result =
                        passwordHasher.VerifyHashedPassword(
                            account,
                            account.MatKhau,
                            password
                        );

                    return result !=
                        PasswordVerificationResult.Failed;
                }

                // Nếu sau này tài khoản Admin dùng BCrypt
                if (IsBcryptHash(account.MatKhau))
                {
                    return BCrypt.Net.BCrypt.Verify(
                        password,
                        account.MatKhau
                    );
                }

                return false;
            }
            catch
            {
                return false;
            }
        }


        // =====================================================
        // CHECK BCrypt
        // =====================================================

        private static bool IsBcryptHash(
            string hash)
        {
            return
                hash.StartsWith("$2a$") ||
                hash.StartsWith("$2b$") ||
                hash.StartsWith("$2y$");
        }


        // =====================================================
        // CHECK ASP.NET IDENTITY HASH
        // =====================================================

        private static bool IsIdentityHash(
            string hash)
        {
            return
                hash.StartsWith("AQAAAA");
        }
    }
}