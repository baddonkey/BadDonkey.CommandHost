using Autofac;

namespace BadDonkey.CommandHost
{
    public static class AutoFacContainerProvider
    {
        public static ILifetimeScope Container { get; set; }
    }
}