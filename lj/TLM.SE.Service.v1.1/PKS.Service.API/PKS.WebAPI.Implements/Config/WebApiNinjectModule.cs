using Ninject.Modules;

namespace PKS.WebAPI.Services
{
    /// <summary>注入模块</summary>
    public class WebApiNinjectModule : NinjectModule
    {
        /// <summary>加载注入</summary>
        public override void Load()
        {
            //暂时取消Mongodb
            //Bind(typeof(IMongoCollection<>)).ToMethod(context => Bootstrapper.ProviderGet(context, typeof(MongoColletionProvider<>))).InSingletonScope();
        }
    }
}
