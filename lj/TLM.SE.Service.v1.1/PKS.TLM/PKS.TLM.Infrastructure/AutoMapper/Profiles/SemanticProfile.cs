using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using PKS.TLM.DbServices.Semantics;
using Jurassic.PKS.Service.Semantics;

namespace PKS.TLM.Infrastructure.AutoMapper.Profiles
{
    /// <summary>自定义对象映射配置</summary>
    public sealed class SemanticProfile : Profile
    {
        /// <summary>构造函数</summary>
        public SemanticProfile()
        {
            CreateMap<SD_ConceptClass, ConceptClass>()
                .ReverseMap();
            CreateMap<SD_SemanticsType, SemanticsType>()
                .ReverseMap();
            CreateMap<SD_CCTerm, TermInfo>()
                .ReverseMap();
        }
    }
}
