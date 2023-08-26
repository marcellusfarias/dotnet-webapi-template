using AutoMapper;
using MyBuyingList.Application.Common.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyBuyingList.Application.Tests
{
    internal static class MapperHelper
    {
        public static IMapper GetMapper()
        {
            MapperConfiguration mapperConfiguration = new MapperConfiguration(cfg =>
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                var assembly = assemblies.Single(x => x.ManifestModule.Name.Equals("MyBuyingList.Application.dll"));

                var types = assembly.GetTypes()
                        .Where(type => type.GetCustomAttributes(typeof(AutoMapperMappingAttribute), true).Any());

                foreach (var mappingType in types)
                {
                    var instance = Activator.CreateInstance(mappingType);
                    var method = mappingType.GetMethod("ConfigureMappings"); 
                    method?.Invoke(instance, new object[] { cfg });
                }
            });

            return mapperConfiguration.CreateMapper();
        }        
    }
}
