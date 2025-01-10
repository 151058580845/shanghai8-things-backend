using Hgzn.Mes.Domain.Entities.System.Base;

namespace Hgzn.Mes.Domain.Entities.System.Account
{
    public class Captcha : UniversalEntity
    {
        public CaptchaType Type { get; set; }

        public (int, int) Pixel { get; set; }

        public byte[]? Image { get; set; }

        public string Answer { get; set; } = null!;

        public override string ToString()
        {
            var base64 = Image is null ?
                string.Empty :
                Convert.ToBase64String(Image);
            return "data:image/png;base64," + base64;
        }
    }

    public enum CaptchaType
    {
        Question,
        Han,
        Character
    }
}