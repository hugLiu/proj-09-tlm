using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKS.TLM.Infrastructure.AutoMapper
{
    public class Configuration
    {
        public static void Configure()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<Profiles.SemanticProfile>();
                cfg.AddProfile<Profiles.UserBehaviorProfile>();
            });
        }
    }
}
