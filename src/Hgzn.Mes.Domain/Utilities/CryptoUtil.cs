using Hgzn.Mes.Domain.Entities.System.Account;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;

namespace Hgzn.Mes.Domain.Utilities
{
    public static class CryptoUtil
    {
        //public static ECDsa PublicECDsa { get; private set; } = ECDsa.Create();
        //public static ECDsa PrivateECDsa { get; private set; } = ECDsa.Create();
        public static string KeyId = "EC_KEY_2024";
        public static ECDsaSecurityKey PrivateECDsaSecurityKey { get; private set; } = null!;
        public static ECDsaSecurityKey PublicECDsaSecurityKey { get; private set; } = null!;
        public static void Initialize(string keyFolder)
        {
            //if (SettingUtil.IsDevelopment)
            //{
            //    //var password = "dev@2024";
            //    //var password = "admin@2024";
            //    var password = "super@2024";
            //    var secret = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            //    var base64 = Convert.ToBase64String(secret);
            //    //------ to backend -------//
            //    var pass = Salt(secret, out var salt);
            //    var pass64 = Convert.ToBase64String(pass);
            //    var salt64 = Convert.ToBase64String(salt);
            //    var isPass = User.DevUser.Verify(secret);
            //}

            var privateKeyPath = Path.Combine(keyFolder, "private-key.pem");
            var publicKeyPath = Path.Combine(keyFolder, "public-key.pem");

            if (!File.Exists(privateKeyPath) || !File.Exists(publicKeyPath))
            {
                var ecdsa = ECDsa.Create();
                ecdsa.GenerateKey(ECCurve.NamedCurves.nistP256);

                var privatePem = ecdsa.ExportECPrivateKeyPem();
                var publicPem = ecdsa.ExportSubjectPublicKeyInfoPem();
                if (!Directory.Exists(keyFolder))
                {
                    Directory.CreateDirectory(keyFolder);
                }
                File.WriteAllText(privateKeyPath, privatePem);
                File.WriteAllText(publicKeyPath, publicPem);
                // 导入密钥
                var privateEcdsa = ECDsa.Create();
                privateEcdsa.ImportFromPem(privatePem);
                var publicEcdsa = ECDsa.Create();
                publicEcdsa.ImportFromPem(publicPem);
                // 创建 SecurityKey 并设置 KeyId
                PrivateECDsaSecurityKey = new ECDsaSecurityKey(privateEcdsa) { KeyId = KeyId };
                PublicECDsaSecurityKey = new ECDsaSecurityKey(publicEcdsa) { KeyId = KeyId };
            }
            else
            {
                // 加载现有密钥
                var privatePem = File.ReadAllText(privateKeyPath);
                var publicPem = File.ReadAllText(publicKeyPath);

                // 导入密钥
                var privateEcdsa = ECDsa.Create();
                privateEcdsa.ImportFromPem(privatePem);
                var publicEcdsa = ECDsa.Create();
                publicEcdsa.ImportFromPem(publicPem);
                PrivateECDsaSecurityKey = new ECDsaSecurityKey(privateEcdsa) { KeyId = KeyId };
                PublicECDsaSecurityKey = new ECDsaSecurityKey(publicEcdsa) { KeyId = KeyId };

                // PrivateECDsa.ImportFromPem(privatePem);
                // PublicECDsa.ImportFromPem(publicPem);
            }
        }

        private static string ParseKeyIdFromPem(string pem)
        {
            var match = Regex.Match(pem, @"KeyId: (\w+)");
            return match.Success ? match.Groups[1].Value : "default-kid";
        }

        /// <summary>
        ///     对指定内容进行加盐
        /// </summary>
        /// <param name="secret">待加盐内容</param>
        /// <param name="salt">此次加盐使用的盐</param>
        /// <returns>已加盐的密文</returns>
        public static byte[] Salt(byte[] secret, out byte[] salt)
        {
            salt = CreateSalt();
            var combine = secret.Concat(salt).ToArray();
            var salted = SHA256.HashData(combine);
            return salted;
        }

        /// <summary>
        ///     通过用户携带的盐验证提供的密码
        /// </summary>
        /// <param name="user"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public static bool Verify(this User user, byte[] secret)
        {
            var salt = Convert.FromBase64String(user.Salt);
            var combine = secret.Concat(salt).ToArray();
            var salted = SHA256.HashData(combine);
            return Convert.FromBase64String(user.Passphrase)
                .SequenceEqual(salted);
        }

        /// <summary>
        ///     产生一个随机长度的盐
        /// </summary>
        /// <returns></returns>
        public static byte[] CreateSalt()
        {
            var guid = Guid.NewGuid().ToByteArray()!;
            var length = Random.Shared.Next(guid.Length / 4, guid.Length / 2);
            var start = Random.Shared.Next(0, guid.Length / 2 - 1);
            return SHA256.HashData(guid[start..(length + start)]);
        }

        /// <summary>
        ///     对字符串内容进行hash
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string Sha256(string content)
        {
            var secret = SHA256.HashData(Encoding.UTF8.GetBytes(content));
            var base64 = Convert.ToBase64String(secret);
            return base64;
        }
    }
}