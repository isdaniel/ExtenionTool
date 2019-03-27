using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using AutoMapper;

namespace ThirdPartEyxtension
{
    public static class AutoMapperExtension
    {
        public static TDestination Map<TDestination>(
            this IMapper mapper, params object[] sources)
            where TDestination : class, new()
        {
            TDestination dest = new TDestination();
            if (!sources.Any())
            {
                return default(TDestination);
            }

            foreach (var source in sources)
            {
                dest = mapper.Map(source, dest);
            }

            return dest;
        }

        public static void AddAutoMapperProfileFromAssembly(this ContainerBuilder builder, params Assembly[] assemblies)
        {
            //register all profile classes in the calling assembly
            builder.RegisterAssemblyTypes(assemblies).As<Profile>();

            builder.Register(context => new MapperConfiguration(cfg =>
            {
                foreach (var profile in context.Resolve<IEnumerable<Profile>>())
                {
                    cfg.AddProfile(profile);
                }

                cfg.ValidateInlineMaps = false;

            })).AsSelf();

            builder.Register(c => c.Resolve<MapperConfiguration>().CreateMapper(c.Resolve))
                .As<IMapper>()
                .InstancePerLifetimeScope();
        }
    }
}
