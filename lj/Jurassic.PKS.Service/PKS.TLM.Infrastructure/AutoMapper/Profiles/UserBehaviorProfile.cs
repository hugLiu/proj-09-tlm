using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using PKS.TLM.DbServices.Semantics;
using Jurassic.PKS.Service.Semantics;
using PKS.WebAPI.Models.UserBehavior.REST;
using PKS.WebAPI.Models.UserBehavior;

namespace PKS.TLM.Infrastructure.AutoMapper.Profiles
{
    /// <summary>自定义对象映射配置</summary>
    public sealed class UserBehaviorProfile : Profile
    {
        /// <summary>构造函数</summary>
        public UserBehaviorProfile()
        {
            CreateMap<dataHotTopic, AnaResult4HotTopic>()
                .ReverseMap();
            CreateMap<anaHotTopicsResult, AnaHotTopicsResult>().ForMember(dest => dest.AnaResult, opt => opt.MapFrom(src => src.data))
                .ReverseMap();
            CreateMap<anaYourLikesResult, AnaYourLikesResult>().ForMember(dest => dest.AnaResult, opt => opt.MapFrom(src => src.data))
                .ReverseMap();
        }
    }
}
