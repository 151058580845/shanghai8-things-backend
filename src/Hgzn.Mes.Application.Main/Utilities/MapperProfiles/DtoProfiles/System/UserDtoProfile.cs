using AutoMapper;
using Hgzn.Mes.Application.Main.Dtos;
using Hgzn.Mes.Domain.Entities.System.Account;
using Hgzn.Mes.Domain.Utilities;
using Hgzn.Mes.Domain.ValueObjects.UserValue;

namespace Hgzn.Mes.Application.Main.Utilities.MapperProfiles.DtoProfiles.System
{
    public class UserDtoProfile : Profile
    {
        public UserDtoProfile()
        {
            CreateMap<User, UserReadDto>();
            CreateMap<User, UserScopeReadDto>()
                .ForMember(dest => dest.RoleCodes, opts => opts.MapFrom(src => src.Roles.Select(r => r.Code)))
                .ForMember(dest => dest.ScopeCodes, opts => opts.MapFrom(
                    src => src.Roles.Where(r => r.Menus != null).SelectMany(r => r.Menus!.Select(m => m.ScopeCode))));
            CreateMap<UserRegisterDto, User>()
                .AfterMap((src, dest) =>
                {
                    var bytes = Convert.FromBase64String(src.Password);
                    dest.Passphrase = Convert.ToBase64String(CryptoUtil.Salt(bytes, out var salt));
                    dest.Salt = Convert.ToBase64String(salt);
                    dest.Roles = [Role.MemberRole];
                });
            CreateMap<Setting, UserSettingReadDto>();
            CreateMap<Setting, UserDetailReadDto>();
            CreateMap<Captcha, CaptchaReadDto>()
                .ForMember(dest => dest.Image, opts => opts.MapFrom(src => src.Image == null ? string.Empty : src.ToString()))
                .ForMember(dest => dest.Type, opts => opts.MapFrom(src => src.Type.ToString("F")))
                .ForMember(dest => dest.Pixel, opts => opts.MapFrom(src => new int[] { src.Pixel.Item1, src.Pixel.Item2 }));
            CreateMap<CaptchaAnswerDto, Captcha>()
                .ForMember(dest => dest.Image, opts => opts.Ignore())
                .ForMember(dest => dest.Pixel, opts => opts.Ignore());
        }
    }
}