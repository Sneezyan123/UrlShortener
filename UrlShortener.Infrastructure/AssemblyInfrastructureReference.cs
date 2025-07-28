using System.Reflection;

namespace UrlShortener.Infrastructure;

public static class AssemblyInfrastructureReference
{
    public static Assembly Assembly = typeof(AssemblyInfrastructureReference).Assembly;
}